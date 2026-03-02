using FavoriteRates.SharedLibrary.Swagger;
using FavoriteRates.UsersService.Application;
using FavoriteRates.UsersService.Endpoints;
using FavoriteRates.UsersService.Infrastructure;

const string serviceName = "UsersService";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddCustomSwagger(serviceName);

var app = builder.Build();

app.UseCustomSwagger(serviceName, behindProxy: true);

app.UseHttpsRedirection();

app.MapUsersEndpoints();

app.Run();