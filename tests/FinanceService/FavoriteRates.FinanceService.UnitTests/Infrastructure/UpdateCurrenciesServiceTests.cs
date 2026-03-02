using System.Net;
using System.Text;
using FavoriteRates.FinanceService.Domain.Entities;
using FavoriteRates.FinanceService.Domain.Repositories;
using FavoriteRates.FinanceService.Infrastructure.BatchProcessing.UpdateCurrencies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FavoriteRates.FinanceService.UnitTests.Infrastructure;

public class UpdateCurrenciesServiceTests
{
    private readonly UpdateCurrenciesService _sut;
    private readonly Mock<IHttpClientFactory> _httpClientFactory = new();
    private readonly Mock<ILogger<UpdateCurrenciesService>> _logger = new();
    private readonly Mock<IOptions<UpdateCurrenciesOptions>> _options = new();
    private readonly Mock<ICurrenciesRepository> _currenciesRepository = new();
    private readonly Mock<HttpMessageHandler> _httpMessageHandler = new(MockBehavior.Strict);

    private const string BaseUrl = "https://example.com";
    private const string Path = "/api/rates";
    private const int Windows1251CodePage = 1251;

    public UpdateCurrenciesServiceTests()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        _options.Setup(x => x.Value).Returns(new UpdateCurrenciesOptions
        {
            ClientBaseUrl = BaseUrl,
            ClientPath = Path
        });

        var httpClient = new HttpClient(_httpMessageHandler.Object)
        {
            BaseAddress = new Uri(BaseUrl)
        };

        _httpClientFactory.Setup(x => x.CreateClient(UpdateCurrenciesService.ClientName))
            .Returns(httpClient);

        _sut = new UpdateCurrenciesService(
            _httpClientFactory.Object,
            _logger.Object,
            _options.Object,
            _currenciesRepository.Object);
    }

    [Fact]
    public async Task UpdateAsync_HttpRequestFails_LogsErrorAndThrows()
    {
        _httpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));
        var ct = CancellationToken.None;

        await Assert.ThrowsAsync<HttpRequestException>(() => _sut.UpdateAsync(ct));

        _logger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("CBR request failed.")),
                It.IsAny<HttpRequestException>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_HappyPath_AddsNewCurrencies()
    {
        const string xml = """
                           <?xml version="1.0" encoding="windows-1251"?>
                           <ValCurs Date="03.03.2026" name="Foreign Currency Market">
                               <Valute ID="R01235">
                                   <NumCode>840</NumCode>
                                   <CharCode>USD</CharCode>
                                   <Nominal>1</Nominal>
                                   <Name>Доллар США</Name>
                                   <Value>70,5000</Value>
                                   <VunitRate>70,5000</VunitRate>
                               </Valute>
                           </ValCurs>
                           """;
        
        var encoding = Encoding.GetEncoding(Windows1251CodePage);
        _httpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.PathAndQuery == Path),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(xml, encoding, "application/xml")
            });

        _currenciesRepository.Setup(x => x.FindByCodeAsync("USD", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Currency?)null);
        var ct = CancellationToken.None;

        await _sut.UpdateAsync(ct);

        _currenciesRepository.Verify(x => x.AddAsync(
            It.Is<Currency>(c => c.Id == "USD" && c.Name == "Доллар США" && c.Rate == 70.50m), 
            ct), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_HappyPath_UpdatesExistingCurrencies()
    {
        const string xml = """
                           <?xml version="1.0" encoding="windows-1251"?>
                           <ValCurs Date="03.03.2026" name="Foreign Currency Market">
                               <Valute ID="R01235">
                                   <NumCode>840</NumCode>
                                   <CharCode>USD</CharCode>
                                   <Nominal>1</Nominal>
                                   <Name>Доллар США Updated</Name>
                                   <Value>71,5000</Value>
                                   <VunitRate>71,5000</VunitRate>
                               </Valute>
                           </ValCurs>
                           """;

        var encoding = Encoding.GetEncoding(Windows1251CodePage);
        _httpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(xml, encoding, "application/xml")
            });

        var existing = new Currency { Id = "USD", Name = "Доллар США", Rate = 70.50m };
        _currenciesRepository.Setup(x => x.FindByCodeAsync("USD", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        var ct = CancellationToken.None;

        await _sut.UpdateAsync(ct);

        _currenciesRepository.Verify(x => x.UpdateAsync(
            It.Is<Currency>(c => c.Id == "USD" && c.Name == "Доллар США Updated" && c.Rate == 71.50m), 
            ct), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_OneCurrencyFails_ContinuesWithOthersAndLogsError()
    {
        const string xml = """
                           <?xml version="1.0" encoding="windows-1251"?>
                           <ValCurs Date="03.03.2026" name="Foreign Currency Market">
                               <Valute ID="R01235">
                                   <NumCode>840</NumCode>
                                   <CharCode>USD</CharCode>
                                   <Nominal>1</Nominal>
                                   <Name>Доллар США</Name>
                                   <Value>70,5000</Value>
                                   <VunitRate>70,5000</VunitRate>
                               </Valute>
                               <Valute ID="R01239">
                                   <NumCode>978</NumCode>
                                   <CharCode>EUR</CharCode>
                                   <Nominal>1</Nominal>
                                   <Name>Евро</Name>
                                   <Value>80,5000</Value>
                                   <VunitRate>80,5000</VunitRate>
                               </Valute>
                           </ValCurs>
                           """;

        var encoding = Encoding.GetEncoding(Windows1251CodePage);
        _httpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(xml, encoding, "application/xml")
            });

        _currenciesRepository.Setup(x => x.FindByCodeAsync("USD", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB failure for USD"));
        
        _currenciesRepository.Setup(x => x.FindByCodeAsync("EUR", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Currency?)null);
        var ct = CancellationToken.None;

        await _sut.UpdateAsync(ct);

        _currenciesRepository.Verify(x => x.AddAsync(
            It.Is<Currency>(c => c.Id == "EUR" && c.Name == "Евро" && c.Rate == 80.50m), 
            ct), Times.Once);

        _logger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Processing currency USD failed.")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
