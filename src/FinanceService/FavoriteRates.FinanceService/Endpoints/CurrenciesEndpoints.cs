namespace FavoriteRates.FinanceService.Endpoints;

public static class CurrenciesEndpoints
{
    public static void MapCurrenciesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/currencies")
            .RequireAuthorization();

        group.MapGet("/rates", async () => await Task.FromResult(new { }));
        
        group.MapGet("/favorites", async () => await Task.FromResult(new { }));
        
        group.MapPost("/favorites", async () => await Task.FromResult(new { }));
    }
}