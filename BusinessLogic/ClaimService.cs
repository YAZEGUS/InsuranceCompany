using Domain;
using Persistence;

namespace BusinessLogic;

public class ClaimService : IClaimService
{
    private readonly IRepository<Claim> _claimRepository;
    private readonly IRepository<Policy> _policyRepository;
    
    public ClaimService(IRepository<Claim> claimRepository, IRepository<Policy> policyRepository)
    {
        _claimRepository = claimRepository;
        _policyRepository = policyRepository;
    }
    
    public Claim CreateClaim(int policyId, DateTime date, string description, decimal payoutAmount)
    {
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