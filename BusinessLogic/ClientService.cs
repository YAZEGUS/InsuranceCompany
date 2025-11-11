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

    // ЕТАП 2
    /// <summary>
    /// Updates the statistical fields (PolicyCount, TotalPayouts) for a client.
    /// </summary>
    /// <param name="clientId">The Id of the client to update.</param>
    /// <param name="policyChange">Delta for policy count (+1 or -1).</param>
    /// <param name="payoutChange">Delta for total payouts.</param>
    public void UpdateClientStats(int clientId, int policyChange = 0, decimal payoutChange = 0m)
    {
        var client = _clientRepository.GetById(clientId);
        if (client == null)
        {
            System.Console.WriteLine($"Error: Client with Id={clientId} not found for stats update.");
            return;
        }

        if (policyChange != 0)
        {
            client.PolicyCount += policyChange;
            // Ensure count is not negative
            if (client.PolicyCount < 0) client.PolicyCount = 0; 
        }

        if (payoutChange != 0m)
        {
            client.TotalPayouts += payoutChange;
        }

        // Викликаємо оновлення в репозиторії, що забезпечить збереження в JSON
        _clientRepository.Update(client);
    }
}