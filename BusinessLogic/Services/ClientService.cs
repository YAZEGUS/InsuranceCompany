using BusinessLogic.Interfaces;
using Domain;
using Persistence;

// Потрібен для Any()

namespace BusinessLogic.Services;

/// <summary>
/// Implements the IClientService interface.
/// Contains business logic for managing clients.
/// </summary>
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
    
    public Client GetClientById(int id)
    {
        return _clientRepository.GetById(id);
    }

    public Client CreateClient(string fullName, string email, ClientTypes clientType)
    {
        // Додамо базову валідацію
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("Full name cannot be empty.");
        }
        
        // !!! НОВЕ: Валідація Email
        if (!email.Contains("@") || !email.Contains("."))
        {
            throw new ArgumentException("Email повинен бути валідним.");
        }
        
        var newClient = new Client
        {
            FullName = fullName,
            Email = email,
            ClientType = clientType
        };
        
        _clientRepository.Add(newClient);
        return newClient;
    }

    public void UpdateClientStats(int clientId, int policyChange = 0, decimal payoutChange = 0m)
    {
        var client = _clientRepository.GetById(clientId);
        if (client == null)
        {
            throw new ArgumentException($"Client with Id={clientId} not found for stats update.");
        }

        if (policyChange != 0)
        {
            client.PolicyCount += policyChange;
            if (client.PolicyCount < 0) client.PolicyCount = 0; 
        }

        if (payoutChange != 0m)
        {
            client.TotalPayouts += payoutChange;
        }

        _clientRepository.Update(client);
    }

    // !!! НОВА РЕАЛІЗАЦІЯ: Бізнес-логіка видалення клієнта
    // ...
    public bool DeleteClient(int clientId)
    {
        // ... (перевірка клієнта)

        // Бізнес-правило: Не можна видалити клієнта, якщо він має активні поліси.
        // Перевіряємо, чи є поліси, які не є Completed або Cancelled.
        bool hasActivePolicies = _policyRepository.GetAll()
            .Any(p => p.ClientId == clientId && p.Status == StatusTypes.Active || p.Status == StatusTypes.Paused);
        //                                                                                     ^^^^^^
        //                                                                                     ЗМІНЕНО З Suspended НА Paused

        if (hasActivePolicies)
        {
            throw new ArgumentException($"Cannot delete client {clientId}. They still have active or paused policies.");
        }
        
        // Видаляємо клієнта, оскільки він чистий
        return _clientRepository.Delete(clientId);
    }
}