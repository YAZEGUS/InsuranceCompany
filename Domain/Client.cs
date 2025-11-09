namespace Domain;

public class Client
{
    public int Id { get;}
    public string FullName { get;}
    public string Email { get;}

    public enum ClientTypes
    {
        Individual,
        Company
    };
    public ClientTypes ClientType { get; }
}