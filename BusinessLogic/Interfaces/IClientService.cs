using Domain;

namespace BusinessLogic.Interfaces;

/// <summary>
/// Contract for the Client Service.
/// Defines business operations related to clients.
/// </summary>
public interface IClientService
{
    /// <summary>
    /// Gets a list of all clients.
    /// </summary>
    /// <returns>A list of clients.</returns>
    List<Client> GetAllClients();
    
    // !!! ДОДАНО: Для ефективної перевірки існування клієнта в PolicyService
    /// <summary>
    /// Gets a single client by its unique identifier.
    /// </summary>
    Client GetClientById(int id);
    
    /// <summary>
    /// Creates a new client.
    /// </summary>
    /// <param name="fullName">The client's full name.</param>
    /// <param name="email">The client's email.</param>
    /// <param name="clientType">The type of client.</param>
    /// <returns>The newly created client with its generated Id.</returns>
    Client CreateClient(string fullName, string email, ClientTypes clientType);

    // ЕТАП 2
    /// <summary>
    /// Updates the statistical fields (PolicyCount, TotalPayouts) for a client.
    /// </summary>
    void UpdateClientStats(int clientId, int policyChange = 0, decimal payoutChange = 0m);

    // !!! НОВЕ: Метод для видалення клієнта
    /// <summary>
    /// Deletes a client by its unique identifier.
    /// </summary>
    /// <param name="clientId">The ID of the client to delete.</param>
    /// <returns>True if deleted, false if not found or if the client has active policies.</returns>
    bool DeleteClient(int clientId);
}