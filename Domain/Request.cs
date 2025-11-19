namespace Domain;

/// <summary>
/// Represents a preliminary request from a client
/// </summary>
public class Request : BaseEntity
{
    public int ClientId { get; set; }
    public PolicyTypes PolicyType { get; set; }
    public decimal DesiredCoverageAmount { get; set; }
    public DateTime CreationDate { get; set; }
    
    public int DurationInMonths { get; set; }
}