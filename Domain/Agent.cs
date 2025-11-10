namespace Domain;

/// <summary>
/// Represents an insurance agent who sells policies.
/// </summary>
public class Agent : BaseEntity
{
    /// <summary>
    /// The name of the agent.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The commission percentage the agent earns on policies.
    /// </summary>
    public decimal CommissionPercentage { get; set; }
}