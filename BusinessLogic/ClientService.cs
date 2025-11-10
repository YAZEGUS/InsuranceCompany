using Persistence;
using Domain;

namespace BusinessLogic;

public class ClientService : IClientService
{
    private readonly IRepository<Client> _clientRepository;
    private readonly IRepository<Policy> _policyRepository;
    
    public ClientService(IRepository<Client> clientRepository, IRepository<Policy> policyRepository)
    {
        _clientRepository = clientRepository;
        _policyRepository = policyRepository;
    }
    
    public List<Client> GetAllClients()
    {
        return _clientRepository.GetAll();
    }

    public Client CreateClient(string fullName, string email, ClientTypes clientType)
    {
        var newClient = new Client
        {
            FullName = fullName,
            Email = email,
            ClientType = clientType
        };
        _clientRepository.Add(newClient);
        return newClient;
    }
}