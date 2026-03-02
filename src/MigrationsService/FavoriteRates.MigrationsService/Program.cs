using FavoriteRates.FinanceService.Infrastructure.Persistence;
using FavoriteRates.UsersService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddScoped<UsersDbContext>(sp => new UsersDbContext(
    builder.Configuration.GetConnectionString("UsersServiceConnection")!,
    sp.GetRequiredService<ILoggerFactory>()));
builder.Services.AddScoped<FinanceDbContext>(sp => new FinanceDbContext(
    builder.Configuration.GetConnectionString("FinanceServiceConnection")!,
    sp.GetRequiredService<ILoggerFactory>()));
    
var app = builder.Build();

using var scope = app.Services.CreateScope();

var usersDb = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
var financeDb = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();

await Task.WhenAll(usersDb.Database.MigrateAsync(), financeDb.Database.MigrateAsync());