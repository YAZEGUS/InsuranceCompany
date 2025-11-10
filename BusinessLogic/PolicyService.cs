using Domain;
using Persistence;
using System;
using System.Collections.Generic;

namespace BusinessLogic;

/// <summary>
/// Implements the IPolicyService interface.
/// Contains business logic for managing policies, including price calculation.
/// </summary>
public class PolicyService : IPolicyService
{
    private readonly IRepository<Policy> _policyRepository;
    private readonly IRepository<Client> _clientRepository;

    /// <summary>
    /// Initializes a new instance of the PolicyService.
    /// </summary>
    /// <param name="policyRepository">The policy data repository.</param>
    /// <param name="clientRepository">The client data repository.</param>
    public PolicyService(IRepository<Policy> policyRepository, IRepository<Client> clientRepository)
    {
        _policyRepository = policyRepository;
        _clientRepository = clientRepository;
    }
    
    public List<Policy> GetAllPolicies()
    {
        return _policyRepository.GetAll();
    }
    
    public Policy CreatePolicy(int clientId, PolicyTypes type, DateTime startDate, DateTime endDate, decimal coverageAmount)
    {
        // Optional: Check if client exists
        var client = _clientRepository.GetById(clientId);
        if (client == null)
        {
            // In a real app, we'd throw an Exception.
            // For console, returning null is an option.
            Console.WriteLine($"Error: Client with Id={clientId} not found.");
            return null;
        }

        // Business Logic: Calculate the price
        decimal price = CalculatePolicyPrice(coverageAmount, type);
        
        var newPolicy = new Policy
        {
            ClientId = clientId,
            PolicyType = type,
            StartDate = startDate,
            EndDate = endDate,
            CoverageAmount = coverageAmount,
            Price = price,
            Status = StatusTypes.Active // Default status
        };
        
        _policyRepository.Add(newPolicy);
        return newPolicy;
    }

    /// <summary>
    /// Calculates the price of a policy based on its type and coverage.
    /// (This is the core business logic for Stage 1).
    /// </summary>
    /// <param name="coverageAmount">The coverage amount.</param>
    /// <param name="policyType">The type of policy.</param>
    /// <returns>The calculated price.</returns>
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
                return 0; // Or throw ArgumentException
        }
    }
}