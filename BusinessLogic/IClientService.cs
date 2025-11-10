using Domain;
namespace BusinessLogic;

public interface IClientService
{
    public List<Client> GetAllClients();
    public Client CreateClient(string fullName, string email, ClientTypes clientType);
}