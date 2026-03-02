using FavoriteRates.FinanceService.Application.Currencies.SetFavorites;
using FavoriteRates.SharedLibrary.ResultPattern;

namespace FavoriteRates.FinanceService.Endpoints;

public static class CurrenciesEndpoints
{
    public static void MapCurrenciesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/currencies")
            .RequireAuthorization();

        group.MapGet("/rates", async () => await Task.FromResult(new { }));
        
        group.MapPost("/favorites", 
            async (SetFavoritesCommand command, SetFavoritesCommandHandler handler, CancellationToken cancellationToken) => 
                (await handler.Handle(command, cancellationToken)).ToProblemDetails());
    }
}