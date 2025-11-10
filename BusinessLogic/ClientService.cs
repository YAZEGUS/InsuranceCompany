using Persistence;
using Domain;
using System.Collections.Generic;

namespace BusinessLogic;

/// <summary>
/// Implements the IClientService interface.
/// Contains business logic for managing clients.
/// </summary>
public class ClientService : IClientService
{
    private readonly IRepository<Client> _clientRepository;
    private readonly IRepository<Policy> _policyRepository; // For future use (e.g., client stats)

    /// <summary>
    /// Initializes a new instance of the ClientService.
    /// </summary>
    /// <param name="clientRepository">The client data repository.</param>
    /// <param name="policyRepository">The policy data repository.</param>
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