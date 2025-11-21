
namespace BusinessLogic.Interfaces;

/// <summary>
/// Контракт для Сервісу курсів валют (інтеграція з зовнішнім API).
/// </summary>
public interface ICurrencyRateService
{
    /// <summary>
    /// Асинхронно отримує обмінний курс з однієї валюти в іншу.
    /// </summary>
    Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency);
}