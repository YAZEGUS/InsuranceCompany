using BusinessLogic.Interfaces;
using Domain;
using Persistence;

namespace BusinessLogic.Services;

public class PaymentService : IPaymentService
{
    private readonly IRepository<Payment> _paymentRepository;
    private readonly IRepository<Policy> _policyRepository; // Для перевірки існування поліса
    private readonly ICurrencyRateService _currencyRateService; // Додано, Етап 5

    public PaymentService(IRepository<Payment> paymentRepository, IRepository<Policy> policyRepository, ICurrencyRateService currencyRateService)
    {
        _paymentRepository = paymentRepository;
        _policyRepository = policyRepository;
        _currencyRateService = currencyRateService;
    }

    public Payment RecordPayment(int policyId, decimal amount, PaymentType type)
    {
        return RecordPaymentAsync(policyId, amount, type, "UAH").GetAwaiter().GetResult();
    }
    public async Task<Payment> RecordPaymentAsync(int policyId, 
        decimal amount, 
        PaymentType type, 
        string paymentCurrency)
    {
        var policy = _policyRepository.GetById(policyId);
        
        if (policy == null)
        {
            throw new ArgumentException($"Policy with Id={policyId} not found.");
        }
        
        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be positive.");
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
            Console.WriteLine($"Payment converted from {paymentCurrency} {amount} to {policyCurrency} {finalAmount} using rate {rate}.");
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
    public List<Payment> GetPaymentsByPolicy(int policyId)
    {
        return _paymentRepository.GetAll()
            .Where(p => p.PolicyId == policyId)
            .ToList();
    }
}