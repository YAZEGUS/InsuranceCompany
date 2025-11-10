using Domain;
namespace BusinessLogic;

public interface IClaimService
{
    public Claim CreateClaim(int policyId, DateTime date, string description, decimal payoutAmount);
}