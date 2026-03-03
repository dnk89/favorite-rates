using FavoriteRates.SharedLibrary.ResultPattern;
using FluentValidation;
using System.Reflection;

namespace FavoriteRates.SharedLibrary.Application;

public class ValidationDecorator<TRequest, TResponse>(
    IHandler<TRequest, TResponse> decorated,
    IEnumerable<IValidator<TRequest>> validators) : IHandler<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(
                validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            
            var failure = validationResults
                .SelectMany(r => r.Errors)
                .FirstOrDefault(f => f != null);

            if (failure != null)
            {
                return CreateFailureResult(failure.ErrorMessage);
            }
        }

        return await decorated.Handle(request, cancellationToken);
    }

    private static TResponse CreateFailureResult(string error)
    {
        if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(error);
        }

        if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var resultType = typeof(TResponse).GetGenericArguments()[0];
            var failureMethod = typeof(Result<>).MakeGenericType(resultType)
                .GetMethod("Failure", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy, [typeof(string)]);
            
            return (TResponse)failureMethod!.Invoke(null, [error])!;
        }

        throw new InvalidOperationException($"Unsupported result type: {typeof(TResponse).Name}");
    }
}