using Domain;
using System.Collections.Generic;

namespace BusinessLogic;

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
    
    /// <summary>
    /// Creates a new client.
    /// </summary>
    /// <param name="fullName">The client's full name.</param>
    /// <param name="email">The client's email.</param>
    /// <param name="clientType">The type of client.</param>
    /// <returns>The newly created client with its generated Id.</returns>
    Client CreateClient(string fullName, string email, ClientTypes clientType);
}