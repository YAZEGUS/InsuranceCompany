using System;

namespace Domain;

/// <summary>
/// Defines the structure for an insurance policy, extending BaseEntity.
/// </summary>
public class Policy : BaseEntity
{
    /// <summary>
    /// Foreign key referencing the Client who owns this policy.
    /// </summary>
    public int ClientId { get; set; }

    // !!! НОВЕ: ID агента, який продав поліс. Може бути null (якщо невідомо або продано напряму).
    public int? AgentId { get; set; } 

    /// <summary>
    /// The type of the insurance policy (e.g., Car, Medical).
    /// </summary>
    public PolicyTypes PolicyType { get; set; }

    /// <summary>
    /// The start date of the policy's validity period.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// The end date of the policy's validity period.
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// The maximum amount the insurance will cover.
    /// </summary>
    public decimal CoverageAmount { get; set; }

    /// <summary>
    /// The cost (premium) of the policy, calculated by Business Logic.
    /// </summary>
    public decimal Price { get; set; }
    
    // ДОДАНО: Валюта поліса
    /// <summary>
    /// The currency of the policy (e.g., UAH, USD).
    /// </summary>
    public string Currency { get; set; } = "UAH";

    /// <summary>
    /// The current status of the policy (e.g., Active, Completed, Cancelled).
    /// </summary>
    public StatusTypes Status { get; set; } = StatusTypes.Active;
}