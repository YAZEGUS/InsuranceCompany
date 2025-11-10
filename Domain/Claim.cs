namespace Domain;

public class Claim : BaseEntity
{
    public int PolicyId { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; }
    public decimal PayoutAmount { get; set; }
    
}