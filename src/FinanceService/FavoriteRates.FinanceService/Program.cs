using FavoriteRates.FinanceService.Application;
using FavoriteRates.FinanceService.Endpoints;
using FavoriteRates.FinanceService.Infrastructure;
using FavoriteRates.SharedLibrary.Swagger;

const string serviceName = "FinanceService";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddCustomSwagger(serviceName, bearerAuth: true);

var app = builder.Build();

app.UseCustomSwagger(serviceName, behindProxy: true);

app.UseHttpsRedirection();

app.MapCurrenciesEndpoints();

app.Run();