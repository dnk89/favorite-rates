using System.Xml.Serialization;
using FavoriteRates.FinanceService.Application.Abstractions;
using FavoriteRates.FinanceService.Domain.Entities;
using FavoriteRates.FinanceService.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FavoriteRates.FinanceService.Infrastructure.BatchProcessing.UpdateCurrencies;

public class UpdateCurrenciesService(
    IHttpClientFactory httpClientFactory,
    ILogger<UpdateCurrenciesService> logger,
    IOptions<UpdateCurrenciesOptions> options,
    ICurrenciesRepository currenciesRepository) : IUpdateCurrenciesService
{
    public const string ClientName = "UpdateCurrenciesClient";
    
    public async Task UpdateAsync(CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient(ClientName);
        try
        {
            var response = await client.GetAsync(options.Value.ClientPath, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            await using var xmlStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var valCurs = DeserializeResponse(xmlStream);

            await ProcessCurrenciesAsync(valCurs, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "CBR request failed.");
            throw;
        }
    }

    private async Task ProcessCurrenciesAsync(CurrenciesRatesDto currenciesRates, CancellationToken cancellationToken)
    {
        foreach (var rateDto in currenciesRates.Rates)
        {
            try
            {
                await ProcessCurrencyAsync(rateDto, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Processing currency {CurrencyCode} failed.", rateDto.Code);
            }
        }
    }

    private async Task ProcessCurrencyAsync(CurrencyRateDto rateDto, CancellationToken cancellationToken)
    {
        var existing = await currenciesRepository.FindByCodeAsync(rateDto.Code, cancellationToken);
        if (existing is null)
        {
            await currenciesRepository.AddAsync(new Currency
            {
                Id = rateDto.Code,
                Name = rateDto.Name,
                Rate = rateDto.GetRate()
            }, cancellationToken);
        }
        else
        {
            existing.Name = rateDto.Name;
            existing.Rate = rateDto.GetRate();
            await currenciesRepository.UpdateAsync(existing, cancellationToken);
        }
    }

    private static CurrenciesRatesDto DeserializeResponse(Stream xmlStream)
    {
        var serializer = new XmlSerializer(typeof(CurrenciesRatesDto));
        var valCurs = (CurrenciesRatesDto?) serializer.Deserialize(xmlStream);

        return valCurs ?? throw new Exception("Invalid response from CBR.");
    }
}