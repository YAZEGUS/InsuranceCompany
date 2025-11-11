using Domain;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq; // Для LINQ-запитів

namespace BusinessLogic;

/// <summary>
/// Implements the IPolicyService interface.
/// Contains business logic for managing policies, including price calculation.
/// </summary>
public class PolicyService : IPolicyService
{
    private readonly IRepository<Policy> _policyRepository;
    private readonly IClientService _clientService; // Використовуємо IClientService для оновлення статистики

    /// <summary>
    /// Initializes a new instance of the PolicyService.
    /// </summary>
    /// <param name="policyRepository">The policy data repository.</param>
    /// <param name="clientService">The client service for updating client statistics.</param>
    public PolicyService(IRepository<Policy> policyRepository, IClientService clientService)
    {
        _policyRepository = policyRepository;
        _clientService = clientService;
    }
    
    public List<Policy> GetAllPolicies()
    {
        return _policyRepository.GetAll();
    }
    
    public Policy CreatePolicy(int clientId, PolicyTypes type, DateTime startDate, DateTime endDate, decimal coverageAmount)
    {
        // Перевірка існування клієнта не потрібна, бо це зробить ClientService при оновленні статистики.
        // Але для коректного функціонування логіки, ми перевіримо, чи можна оновити статистику:
        var client = _clientService.GetAllClients().FirstOrDefault(c => c.Id == clientId);
        if (client == null)
        {
            Console.WriteLine($"Error: Client with Id={clientId} not found.");
            return null;
        }

        // Business Logic: Calculate the price (Етап 1)
        decimal price = CalculatePolicyPrice(coverageAmount, type);
        
        var newPolicy = new Policy
        {
            // PolicyNumber буде порожнім у цій реалізації, бо це ускладнить Етап 1/2.
            // Його можна додати у наступних етапах.
            ClientId = clientId,
            PolicyType = type,
            StartDate = startDate,
            EndDate = endDate,
            CoverageAmount = coverageAmount,
            Price = price,
            Status = StatusTypes.Active // Default status
        };
        
        _policyRepository.Add(newPolicy);
        
        // ЕТАП 2
        _clientService.UpdateClientStats(clientId, policyChange: 1);

        return newPolicy;
    }

    /// <summary>
    /// Calculates the price of a policy based on its type and coverage.
    /// (This is the core business logic for Stage 1).
    /// </summary>
    
    private decimal CalculatePolicyPrice(decimal coverageAmount, PolicyTypes policyType)
    {
        switch (policyType)
        {
            case PolicyTypes.CarInsurance:
                return coverageAmount * 0.05m;
            case PolicyTypes.MedicalInsurance:
                return coverageAmount * 0.07m;
            case PolicyTypes.PropertyInsurance:
                return coverageAmount * 0.10m;
            default:
                return 0; 
        }
    }

    // ЕТАП 2

    public bool ChangePolicyStatus(int policyId, StatusTypes newStatus)
    {
        var policy = _policyRepository.GetById(policyId);
        if (policy == null)
        {
            Console.WriteLine($"Error: Policy with Id={policyId} not found.");
            return false;
        }

     
        
        policy.Status = newStatus;
        _policyRepository.Update(policy);
        Console.WriteLine($"Policy {policyId} status updated to {newStatus}.");
        return true;
    }

    public List<Policy> SearchPolicies(PolicyTypes? type = null, int? clientId = null, 
        StatusTypes? status = null, decimal? minPrice = null, decimal? maxPrice = null)
    {
        IEnumerable<Policy> query = _policyRepository.GetAll();

        if (type.HasValue)
        {
            query = query.Where(p => p.PolicyType == type.Value);
        }

        if (clientId.HasValue)
        {
            query = query.Where(p => p.ClientId == clientId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        return query.ToList();
    }
}