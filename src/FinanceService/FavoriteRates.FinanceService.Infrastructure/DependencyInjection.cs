using System.Text;
using FavoriteRates.FinanceService.Application.Abstractions;
using FavoriteRates.FinanceService.Domain.Services;
using FavoriteRates.FinanceService.Infrastructure.BatchProcessing.UpdateCurrencies;
using FavoriteRates.FinanceService.Infrastructure.Persistence;
using FavoriteRates.FinanceService.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Polly;

namespace FavoriteRates.FinanceService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication()
            .AddJwtBearer(o =>
            {
                var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!));
                
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = signingKey,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"]
                };
            });
        
        services.AddAuthorization();
        
        services.AddOptions<UpdateCurrenciesOptions>()
            .Bind(configuration.GetSection(UpdateCurrenciesOptions.Key))
            .ValidateDataAnnotations();

        services.AddHttpClient(UpdateCurrenciesService.ClientName, (sp, config) =>
        {
            var options = sp.GetRequiredService<IOptions<UpdateCurrenciesOptions>>().Value;
            config.BaseAddress = new Uri(options.ClientBaseUrl);
            config.DefaultRequestHeaders.Add("User-Agent", "FavoriteRates.FinanceService");
        })
        .AddStandardResilienceHandler(options =>
        {
            options.Retry.MaxRetryAttempts = configuration.GetValue<int>($"{UpdateCurrenciesOptions.Key}:ClientMaxRetryAttempts");
            options.Retry.Delay = TimeSpan.FromSeconds(configuration.GetValue<int>($"{UpdateCurrenciesOptions.Key}:ClientRetryDelayInSeconds"));
            options.Retry.BackoffType = DelayBackoffType.Exponential;
        });

        services.AddScoped<FinanceDbContext>(sp => new FinanceDbContext(
            configuration.GetConnectionString("DefaultConnection")!,
            sp.GetRequiredService<ILoggerFactory>()));
        services.AddTransient<ICurrenciesRepository, CurrenciesRepository>();
        services.AddTransient<IUpdateCurrenciesService, UpdateCurrenciesService>();
        services.AddHostedService<UpdateCurrenciesWorker>();
        
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        
        return services;
    }
}