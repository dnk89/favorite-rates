using FavoriteRates.FinanceService.Application;
using FavoriteRates.FinanceService.Endpoints;
using FavoriteRates.FinanceService.Extensions;
using FavoriteRates.FinanceService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddSwaggerServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapCurrenciesEndpoints();

app.Run();