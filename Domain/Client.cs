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
}