using FavoriteRates.FinanceService.Application.Currencies.GetFavoritesRates;
using FavoriteRates.FinanceService.Application.Currencies.SetFavorites;
using FavoriteRates.SharedLibrary.Application;
using FavoriteRates.SharedLibrary.ResultPattern;
using Microsoft.AspNetCore.Mvc;

namespace FavoriteRates.FinanceService.Endpoints;

public static class CurrenciesEndpoints
{
    public static void MapCurrenciesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/currencies")
            .RequireAuthorization();

        group.MapGet("/favorites/rates", 
            async ([FromServices] IHandler<GetFavoritesRatesQuery, Result<IEnumerable<CurrencyRateDto>>> handler, CancellationToken cancellationToken) => 
                (await handler.Handle(new GetFavoritesRatesQuery(), cancellationToken)).ToProblemDetails());
        
        group.MapPost("/favorites", 
            async (SetFavoritesCommand command, IHandler<SetFavoritesCommand, Result> handler, CancellationToken cancellationToken) => 
                (await handler.Handle(command, cancellationToken)).ToProblemDetails());
    }
}