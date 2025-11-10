using System;

namespace Domain;

/// <summary>
/// Represents an insurance claim filed against a policy.
/// </summary>
public class Claim : BaseEntity
{
    /// <summary>
    /// Foreign key referencing the Policy this claim is associated with.
    /// </summary>
    public int PolicyId { get; set; }

    /// <summary>
    /// The date the claim was filed.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// A description of the insurance event.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// The amount to be paid out for this claim.
    /// </summary>
    public decimal PayoutAmount { get; set; }
    
    // public ClaimStatus Status { get; set; } // Ready for Stage 2
}