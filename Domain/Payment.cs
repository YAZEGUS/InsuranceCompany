namespace Domain;

/// <summary>
/// Тип транзакції: Внесок від клієнта (Premium) або Виплата компанії (Payout).
/// </summary>
public enum PaymentType
{
    Contribution, // Внесок/Премія (гроші йдуть в компанію)
    Payout        // Виплата (гроші йдуть клієнту)
}

/// <summary>
/// Представляє платіжну транзакцію, пов'язану зі страховим полісом.
/// </summary>
public class Payment : BaseEntity
{
    /// <summary>
    /// Зовнішній ключ, що посилається на Поліс, з яким пов'язаний цей платіж.
    /// </summary>
    public int PolicyId { get; set; }

    /// <summary>
    /// Дата, коли відбувся платіж/транзакція.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Сума транзакції.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Тип транзакції (Contribution або Payout).
    /// </summary>
    public PaymentType Type { get; set; }
}