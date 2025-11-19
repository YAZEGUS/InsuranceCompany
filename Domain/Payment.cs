using System;

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
/// Represents a payment transaction related to an insurance policy.
/// </summary>
public class Payment : BaseEntity
{
    /// <summary>
    /// Foreign key referencing the Policy this payment is associated with.
    /// </summary>
    public int PolicyId { get; set; }

    /// <summary>
    /// The date the payment/transaction occurred.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// The amount of the transaction.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// The type of the transaction (Contribution or Payout).
    /// </summary>
    public PaymentType Type { get; set; }
}