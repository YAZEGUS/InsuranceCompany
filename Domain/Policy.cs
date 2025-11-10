using System;

namespace Domain;

/// <summary>
/// Represents an insurance policy.
/// </summary>
public class Policy : BaseEntity
{
    /// <summary>
    /// Foreign key referencing the Client who owns this policy.
    /// </summary>
    public int ClientId { get; set; }

    /// <summary>
    /// The unique human-readable number for this policy.
    /// </summary>
    public string PolicyNumber { get; set; }

    /// <summary>
    /// The type of insurance (e.g., Car, Medical).
    /// </summary>
    public PolicyTypes PolicyType { get; set; }

    /// <summary>
    /// The date when the policy coverage begins.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// The date when the policy coverage ends.
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// The maximum amount the policy will pay out.
    /// </summary>
    public decimal CoverageAmount { get; set; }

    /// <summary>
    /// The cost of the policy, calculated by the Business Logic.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// The current status of the policy (e.g., Active, Paused).
    /// </summary>
    public StatusTypes Status { get; set; }
}