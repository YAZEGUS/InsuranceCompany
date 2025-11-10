using Domain;
using System;

namespace BusinessLogic;

/// <summary>
/// Contract for the Claim Service.
/// Defines business operations related to claims.
/// </summary>
public interface IClaimService
{
    /// <summary>
    /// Creates a new claim.
    /// </summary>
    /// <param name="policyId">The Id of the policy being claimed against.</param>
    /// <param name="date">The date the claim was filed.</param>
    /// <param name="description">Description of the event.</param>
    /// <param name="payoutAmount">The requested payout amount.</param>
    /// <returns>The newly created claim with its generated Id.</returns>
    Claim CreateClaim(int policyId, DateTime date, string description, decimal payoutAmount);
}