using Domain;
using Persistence;
using System;
using System.Linq;

namespace BusinessLogic;

/// <summary>
/// Implements the IClaimService interface.
/// Contains business logic for managing claims.
/// </summary>
public class ClaimService : IClaimService
{
    private readonly IRepository<Claim> _claimRepository;
    private readonly IRepository<Policy> _policyRepository;
    private readonly IClientService _clientService; // Для оновлення статистики клієнта

    /// <summary>
    /// Initializes a new instance of the ClaimService.
    /// </summary>
    /// <param name="claimRepository">The claim data repository.</param>
    /// <param name="policyRepository">The policy data repository.</param>
    /// <param name="clientService">The client service for updating client statistics.</param>
    public ClaimService(IRepository<Claim> claimRepository, IRepository<Policy> policyRepository, IClientService clientService)
    {
        _claimRepository = claimRepository;
        _policyRepository = policyRepository;
        _clientService = clientService; // Ініціалізація нового поля
    }
    
    public Claim CreateClaim(int policyId, DateTime date, string description, decimal payoutAmount)
    {
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
            PayoutAmount = payoutAmount,
            Status = ClaimStatusTypes.New // Статус за замовчуванням (Етап 2)
        };
        
        _claimRepository.Add(newClaim);
        return newClaim;
    }

    // ЕТАП 2

    public bool ChangeClaimStatus(int claimId, ClaimStatusTypes newStatus)
    {
        var claim = _claimRepository.GetById(claimId);
        if (claim == null)
        {
            Console.WriteLine($"Error: Claim with Id={claimId} not found.");
            return false;
        }

        // Логіка оновлення статистики клієнта при виплаті (Approved/Paid)
        if (newStatus == ClaimStatusTypes.Paid && claim.Status != ClaimStatusTypes.Paid)
        {
            var policy = _policyRepository.GetById(claim.PolicyId);
            if (policy != null)
            {
                // Оновлюємо загальну суму виплат для клієнта
                _clientService.UpdateClientStats(policy.ClientId, payoutChange: claim.PayoutAmount);
                Console.WriteLine($"Client {policy.ClientId} stats updated with payout of {claim.PayoutAmount}.");
            }
        }

        claim.Status = newStatus;
        _claimRepository.Update(claim);
        Console.WriteLine($"Claim {claimId} status updated to {newStatus}.");
        return true;
    }
}