namespace Domain;

public class Client : BaseEntity
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public ClientTypes ClientType { get; set; }
}