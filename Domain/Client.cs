namespace Domain;

/// <summary>
/// Represents a client of the insurance company (either an individual or a company).
/// </summary>
public class Client : BaseEntity
{
    /// <summary>
    /// The full name of the individual or the name of the company.
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    /// Contact email address for the client.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// The type of client (Individual or Company).
    /// </summary>
    public ClientTypes ClientType { get; set; }
    
    // ЕТАП 2
    /// <summary>
    /// The total number of policies the client currently has.
    /// </summary>
    public int PolicyCount { get; set; } = 0;

    /// <summary>
    /// The cumulative sum of all payouts received by the client.
    /// </summary>
    public decimal TotalPayouts { get; set; } = 0m;
}