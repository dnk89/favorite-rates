using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace FavoriteRates.SharedLibrary.Swagger;

public static class SwaggerExtensions
{
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services, string serviceName, 
        string version = "v1", bool bearerAuth = false)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(version, new OpenApiInfo { Title = serviceName, Version = version });
            
            if (!bearerAuth) return;
            
            const string scheme = "Bearer";

            options.AddSecurityDefinition(scheme, new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please insert JWT token into field",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = scheme
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = scheme
                        }
                    },
                    []
                }
            });
        });
        
        return services;
    }
    
    public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app, 
        string serviceName, string version = "v1", bool behindProxy = false)
    {
        if (!behindProxy)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            return app;
        }
        
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.All
        });
        app.UseSwagger(s =>
        {
            s.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
            {
                var loggerFactory = httpReq.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("SwaggerExtensions");
                logger.LogInformation("Swagger pre-serialize filter called.");
                foreach (var httpReqHeader in httpReq.Headers)
                {
                    logger.LogInformation("Header: {HeaderName} = {HeaderValue}", httpReqHeader.Key, httpReqHeader.Value);
                }
                if (httpReq.Headers.TryGetValue(ForwardedHeadersDefaults.XForwardedPrefixHeaderName, out var value) && value.Count > 0)
                {
                    swaggerDoc.Servers = new List<OpenApiServer>
                    {
                        new() { Url = value }
                    };
                }
            });
        });
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint($"{version}/swagger.json", $"{serviceName} {version}");
        });
        
        return app;
    }
}