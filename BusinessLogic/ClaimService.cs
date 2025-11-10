using Domain;
using Persistence;
using System;

namespace BusinessLogic;

/// <summary>
/// Implements the IClaimService interface.
/// Contains business logic for managing claims.
/// </summary>
public class ClaimService : IClaimService
{
    private readonly IRepository<Claim> _claimRepository;
    private readonly IRepository<Policy> _policyRepository; // For validation (e.g., is policy active?)

    /// <summary>
    /// Initializes a new instance of the ClaimService.
    /// </summary>
    /// <param name="claimRepository">The claim data repository.</param>
    /// <param name="policyRepository">The policy data repository.</param>
    public ClaimService(IRepository<Claim> claimRepository, IRepository<Policy> policyRepository)
    {
        _claimRepository = claimRepository;
        _policyRepository = policyRepository;
    }
    
    public Claim CreateClaim(int policyId, DateTime date, string description, decimal payoutAmount)
    {
        // Optional: Check if policy exists
        var policy = _policyRepository.GetById(policyId);
        if (policy == null)
        {
            Console.WriteLine($"Error: Policy with Id={policyId} not found.");
            return null;
        }
        
        var newClaim = new Claim
        {
            PolicyId = policyId,
            Date = date,
            Description = description,
            PayoutAmount = payoutAmount
        };
        
        _claimRepository.Add(newClaim);
        return newClaim;
    }
}