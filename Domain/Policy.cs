namespace Domain;

public class Policy : BaseEntity
{
    public string PolicyNumber { get; set; }
    public PolicyTypes PolicyType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal CoverageAmount { get; set; }
    public decimal Price { get; set; }
    public StatusTypes Status { get; set; }
    public int ClientId { get; set; }
}