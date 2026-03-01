using System.Globalization;
using System.Xml.Serialization;

namespace FavoriteRates.FinanceService.Infrastructure.BatchProcessing.UpdateCurrencies;

[XmlRoot("ValCurs")]
public class CurrenciesRatesDto
{
    [XmlAttribute("Date")]
    public string? Date { get; set; }
    
    [XmlElement("Valute")]
    public List<CurrencyRateDto> Rates { get; set; } = [];
}

public class CurrencyRateDto
{
    [XmlElement("CharCode")]
    public string Code { get; set; } = string.Empty;
    [XmlElement("Name")]
    public string Name { get; set; } = string.Empty;
    [XmlElement("VunitRate")]
    public string VunitRateString { get; set; }  = string.Empty;
    
    public decimal GetRate() => string.IsNullOrEmpty(VunitRateString) ?
        0 :
        decimal.Parse(VunitRateString.Replace(',', '.'), CultureInfo.InvariantCulture);
}