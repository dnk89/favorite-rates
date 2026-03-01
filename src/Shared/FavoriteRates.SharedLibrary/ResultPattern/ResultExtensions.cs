using Microsoft.AspNetCore.Http;

namespace FavoriteRates.SharedLibrary.ResultPattern;

public static class ResultExtensions
{
    public static IResult ToProblemDetails<T>(this Result<T> result)
    {
        return result.IsSuccess ? Results.Ok(result.Value) : Results.Problem(result.Error);
    }
}