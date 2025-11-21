using Domain;
using System.Collections.Generic; // Потрібен для List

namespace BusinessLogic.Interfaces;

/// <summary>
/// Contract for the Policy Service.
/// Defines business operations related to insurance policies.
/// </summary>
public interface IPolicyService
{
    /// <summary>
    /// Gets a list of all policies.
    /// </summary>
    /// <returns>A list of policies.</returns>
    List<Policy> GetAllPolicies();

    /// <summary>
    /// Creates a new insurance policy and calculates its price.
    /// </summary>
    /// <param name="clientId">The Id of the client purchasing the policy.</param>
    /// <param name="agentId">The Id of the agent who sold the policy (optional).</param>
    /// <param name="type">The type of insurance.</param>
    /// <param name="startDate">The policy start date.</param>
    /// <param name="endDate">The policy end date.</param>
    /// <param name="coverageAmount">The coverage amount.</param>
    /// <returns>The newly created policy with its price and Id.</returns>
    Policy CreatePolicy(int clientId, int? agentId, PolicyTypes type, 
        DateTime startDate, DateTime endDate, decimal coverageAmount);

    /// <summary>
    /// Changes the status of an existing policy.
    /// </summary>
    /// <param name="policyId">The Id of the policy to update.</param>
    /// <param name="newStatus">The new status to set.</param>
    /// <returns>True if the status was successfully updated, false otherwise.</returns>
    bool ChangePolicyStatus(int policyId, StatusTypes newStatus);

    /// <summary>
    /// Searches for policies based on specified criteria.
    /// </summary>
    /// <param name="type">Filter by policy type (optional).</param>
    /// <param name="clientId">Filter by client Id (optional).</param>
    /// <param name="status">Filter by status (optional).</param>
    /// <param name="minPrice">Minimum price in the range (optional).</param>
    /// <param name="maxPrice">Maximum price in the range (optional).</param>
    /// <param name="agentId">Filter by agent Id (optional). </param>
    /// <returns>A list of policies matching the search criteria.</returns>
    List<Policy> SearchPolicies(PolicyTypes? type = null, int? clientId = null, 
        StatusTypes? status = null, decimal? minPrice = null, decimal? maxPrice = null,
        int? agentId = null);
}