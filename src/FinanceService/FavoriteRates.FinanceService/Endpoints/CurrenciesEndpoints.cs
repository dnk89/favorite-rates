using FavoriteRates.FinanceService.Application.Currencies.GetFavoritesRates;
using FavoriteRates.FinanceService.Application.Currencies.SetFavorites;
using FavoriteRates.SharedLibrary.ResultPattern;
using Microsoft.AspNetCore.Mvc;

namespace FavoriteRates.FinanceService.Endpoints;

public static class CurrenciesEndpoints
{
    public static void MapCurrenciesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/currencies")
            .RequireAuthorization();

        group.MapGet("/rates", 
            async ([FromServices] GetFavoritesRatesQueryHandler handler, CancellationToken cancellationToken) => 
                (await handler.Handle(new GetFavoritesRatesQuery(), cancellationToken)).ToProblemDetails());
        
        group.MapPost("/favorites", 
            async (SetFavoritesCommand command, SetFavoritesCommandHandler handler, CancellationToken cancellationToken) => 
                (await handler.Handle(command, cancellationToken)).ToProblemDetails());
    }
}