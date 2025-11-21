using BusinessLogic.Interfaces;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Клас для десеріалізації відповіді API
/// </summary>
public class ExchangeRateResponse
{
    [JsonPropertyName("result")]
    public string Result { get; set; }
        
    [JsonPropertyName("conversion_rate")]
    public decimal ConversionRate { get; set; }
}
/// <summary>
/// Реалізація сервісу для отримання курсів валют із зовнішнього API.
/// </summary>
public class CurrencyRateService : ICurrencyRateService
{
    private readonly HttpClient _httpClient;
    private const string ApiBaseUrl = "https://v6.exchangerate-api.com/v6/4e8a7654d08a49076e4bae3f/pair/"; // Замініть на свій ключ API

    public CurrencyRateService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    /// <summary>
    /// Асинхронно отримує обмінний курс з однієї валюти в іншу.
    /// </summary>
    public async Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency)
    {
        try
        {
            string url = $"{ApiBaseUrl}{fromCurrency}/{toCurrency}";
            
            var response = await _httpClient.GetFromJsonAsync<ExchangeRateResponse>(url);

            if (response == null || response.Result != "success")
            {
                throw new Exception("Не вдалося отримати курс валют від зовнішнього API.");
            }

            return response.ConversionRate;
        }
        catch (Exception ex)
        {
            // У разі помилки зв'язку чи парсингу, повертаємо 1.0m, але логуємо помилку
            Console.WriteLine($"Помилка інтеграції з API курсів валют: {ex.Message}");
            return 1.0m;
        }
    }
}
