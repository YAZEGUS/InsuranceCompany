using BusinessLogic.Interfaces;
using Domain;
using Persistence;

namespace BusinessLogic.Services;

/// <summary>
/// Реалізує логіку управління платежами, включаючи конвертацію валют.
/// </summary>
public class PaymentService : IPaymentService
{
    private readonly IRepository<Payment> _paymentRepository;
    private readonly IRepository<Policy> _policyRepository; // Для перевірки існування поліса
    private readonly ICurrencyRateService _currencyRateService; // Інтерфейс для конвертації валют (Етап 5)

    public PaymentService(IRepository<Payment> paymentRepository, IRepository<Policy> policyRepository, ICurrencyRateService currencyRateService)
    {
        _paymentRepository = paymentRepository;
        _policyRepository = policyRepository;
        _currencyRateService = currencyRateService;
    }
    
    /// <summary> Створює запис про новий платіж, конвертуючи суму, якщо валюти відрізняються. </summary>
    public async Task<Payment> RecordPaymentAsync(int policyId, 
        decimal amount, 
        PaymentType type, 
        string paymentCurrency)
    {
        var policy = _policyRepository.GetById(policyId);
        
        if (policy == null)
        {
            throw new ArgumentException($"Поліс з Id={policyId} не знайдено.");
        }
        
        if (amount <= 0)
        {
            throw new ArgumentException("Сума має бути більшою за нуль.");
        }

        // 1. Визначення валюти поліса
        string policyCurrency = policy.Currency; 
        decimal finalAmount = amount;

        // 2. Логіка конвертації
        if (!string.Equals(paymentCurrency, policyCurrency, StringComparison.OrdinalIgnoreCase))
        {
            // Виклик API через інтерфейс
            decimal rate = await _currencyRateService.GetExchangeRateAsync(paymentCurrency, policyCurrency);
            finalAmount = amount * rate;
            Console.WriteLine($"Платіж конвертовано з {paymentCurrency} {amount} на {policyCurrency} {finalAmount} курсом {rate}.");
        }

        // 3. Створення запису про платіж у валюті поліса
        var newPayment = new Payment
        {
            PolicyId = policyId,
            Date = DateTime.Now,
            Amount = finalAmount,
            Type = type
        };

        _paymentRepository.Add(newPayment);
        return newPayment;
    }
    /// <summary> Отримує список усіх платежів, пов'язаних із конкретним полісом. </summary>
    public List<Payment> GetPaymentsByPolicy(int policyId)
    {
        return _paymentRepository.GetAll()
            .Where(p => p.PolicyId == policyId)
            .ToList();
    }
}