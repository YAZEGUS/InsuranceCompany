using Domain;

namespace BusinessLogic.Interfaces;

/// <summary>
/// Контракт для Сервісу платежів. Керує фіксацією внесків та виплат з підтримкою конвертації валют.
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Асинхронно створює запис про новий платіж, конвертуючи суму через API.
    /// </summary>
    /// <param name="policyId">Ідентифікатор поліса.</param>
    /// <param name="amount">Сума платежу у вихідній валюті.</param>
    /// <param name="type">Тип платежу.</param>
    /// <param name="paymentCurrency">Валюта платежу.</param>
    /// <returns>Створений об'єкт Payment (з конвертованою сумою).</returns>
    Task<Payment> RecordPaymentAsync(int policyId, 
        decimal amount, 
        PaymentType type, 
        string paymentCurrency);
    
    /// <summary>
    /// Отримує список усіх платежів, пов'язаних із конкретним полісом.
    /// </summary>
    List<Payment> GetPaymentsByPolicy(int policyId);
}