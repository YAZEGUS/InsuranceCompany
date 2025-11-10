using Domain;
using System;
using System.Collections.Generic;

namespace BusinessLogic;

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
    /// <param name="type">The type of insurance.</param>
    /// <param name="startDate">The policy start date.</param>
    /// <param name="endDate">The policy end date.</param>
    /// <param name="coverageAmount">The coverage amount.</param>
    /// <returns>The newly created policy with its price and Id.</returns>
    Policy CreatePolicy(int clientId, PolicyTypes type, 
        DateTime startDate, DateTime endDate, decimal coverageAmount);
}