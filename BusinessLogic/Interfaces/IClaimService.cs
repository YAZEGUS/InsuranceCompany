using Domain;

namespace BusinessLogic.Interfaces;

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

    // ЕТАП 2
    /// <summary>
    /// Changes the status of an existing claim.
    /// </summary>
    /// <param name="claimId">The Id of the claim to update.</param>
    /// <param name="newStatus">The new status to set.</param>
    /// <returns>True if the status was successfully updated, false otherwise.</returns>
    bool ChangeClaimStatus(int claimId, ClaimStatusTypes newStatus);
}