namespace BusinessLogic.Interfaces;

public interface ICurrencyRateService
{
    Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency);
}