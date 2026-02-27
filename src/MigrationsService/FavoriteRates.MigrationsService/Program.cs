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
    
var app = builder.Build();

using var scope = app.Services.CreateScope();

var db = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
await db.Database.MigrateAsync();