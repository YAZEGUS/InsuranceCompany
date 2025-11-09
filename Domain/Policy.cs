namespace Domain;
public enum PolicyTypes
{
    CarInsurance,
    MedicalInsurance,
    PropertyInsurance,
}
public enum StatusTypes
{
    Active, 
    Paused, 
    Cancelled
}
public class Policy
{
    public int Id { get; }
    public string PolicyNumber { get; }
    public PolicyTypes PolicyType { get; }
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }
    public decimal CoverageAmount { get; }
    public decimal Price { get; }
    public StatusTypes Status { get;}
}