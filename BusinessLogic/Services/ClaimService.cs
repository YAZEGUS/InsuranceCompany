using BusinessLogic.Interfaces;
using Domain;
using Persistence;
using System.Collections.Generic; // Потрібен для List<Claim>

namespace BusinessLogic.Services;

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
        _clientService = clientService;
    }
    
    // !!! НОВЕ: Реалізація методу GetAll()
    public List<Claim> GetAll()
    {
        return _claimRepository.GetAll();
    }
    
    public Claim CreateClaim(int policyId, DateTime date, string description, decimal payoutAmount)
    {
        var policy = _policyRepository.GetById(policyId);
        
        // Перевірка існування поліса
        if (policy == null)
        {
            throw new ArgumentException($"Policy with Id={policyId} not found.");
        }
        
        // Перевірка, чи поліс не скасовано/завершено
        if (policy.Status == StatusTypes.Cancelled || policy.Status == StatusTypes.Completed)
        {
            throw new ArgumentException($"Policy with Id={policyId} is {policy.Status} and cannot have new claims.");
        }
        
        // Додаткова перевірка: сума виплати не може перевищувати покриття (хоча це може бути бізнес-рішенням)
        if (payoutAmount > policy.CoverageAmount)
        {
             // Це може бути попередженням, але для строгості залишимо виняток
            throw new ArgumentException($"Payout amount ({payoutAmount:0.00}) exceeds policy coverage ({policy.CoverageAmount:0.00}).");
        }

        var newClaim = new Claim
        {
            PolicyId = policyId,
            Date = date,
            Description = description,
            PayoutAmount = payoutAmount,
            Status = ClaimStatusTypes.New 
        };
        
        _claimRepository.Add(newClaim);
        return newClaim;
    }

    public bool ChangeClaimStatus(int claimId, ClaimStatusTypes newStatus)
    {
        var claim = _claimRepository.GetById(claimId);
        
        // Перевірка існування події
        if (claim == null)
        {
            throw new ArgumentException($"Claim with Id={claimId} not found.");
        }
        
        // Логіка оновлення статистики клієнта при виплаті (Paid)
        if (newStatus == ClaimStatusTypes.Paid && claim.Status != ClaimStatusTypes.Paid)
        {
            var policy = _policyRepository.GetById(claim.PolicyId);
            
            if (policy != null)
            {
                // Оновлюємо загальну суму виплат для клієнта
                _clientService.UpdateClientStats(policy.ClientId, payoutChange: claim.PayoutAmount);
            }
        }
        
        // Якщо ми скасовуємо виплату (напр., перехід з Paid на Approved),
        // ми повинні відмінити зміну статистики (це більш складна логіка, але важлива)
        if (claim.Status == ClaimStatusTypes.Paid && newStatus != ClaimStatusTypes.Paid)
        {
            var policy = _policyRepository.GetById(claim.PolicyId);
            if (policy != null)
            {
                // Відміняємо попередню виплату
                _clientService.UpdateClientStats(policy.ClientId, payoutChange: -claim.PayoutAmount);
            }
        }

        claim.Status = newStatus;
        _claimRepository.Update(claim);
        return true;
    }
}