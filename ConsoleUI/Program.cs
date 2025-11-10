using BusinessLogic;
using Domain;
using Persistence;
using System;
using System.Collections.Generic;

namespace ConsoleUI;

/// <summary>
/// Main entry point for the console application.
/// Handles user input, dependency injection, and calls business logic services.
/// </summary>
public class Program
{
    // --- Service fields for Dependency Injection ---
    private static readonly IClientService _clientService;
    private static readonly IPolicyService _policyService;
    private static readonly IClaimService _claimService;

    /// <summary>
    /// Static constructor to set up repositories and services (Dependency Injection).
    /// </summary>
    static Program()
    {
        // Initialize Repositories
        IRepository<Client> clientRepository = new JsonRepository<Client>("clients.json");
        IRepository<Policy> policyRepository = new JsonRepository<Policy>("policies.json");
        IRepository<Claim> claimRepository = new JsonRepository<Claim>("claims.json");
        
        // Initialize Services
        _clientService = new ClientService(clientRepository, policyRepository);
        _policyService = new PolicyService(policyRepository, clientRepository);
        _claimService = new ClaimService(claimRepository, policyRepository); 
    }

    /// <summary>
    /// The main entry point for the application.
    /// Runs the main menu loop.
    /// </summary>
    static void Main(string[] args)
    {
        Console.WriteLine("--- Insurance Management System ---");
        
        while (true)
        {
            Console.WriteLine("\n--- MAIN MENU ---");
            Console.WriteLine("Press 1 to manage clients");
            Console.WriteLine("Press 2 to manage policies");
            Console.WriteLine("Press 3 to create a claim");
            Console.WriteLine("Press 0 to exit");
            Console.Write("Your choice: ");
            
            // Using int.Parse as per your style
            int choice = int.Parse(Console.ReadLine());
            
            switch (choice)
            {
                case 1:
                    ManageClients(_clientService);
                    break;
                case 2:
                    ManagePolicies(_policyService);
                    break;
                case 3:
                    CreateNewClaim(_claimService); 
                    break;
                case 0:
                    return; // Exit the loop and program
            }
        }
    }
    
    /// <summary>
    /// Handles the Client Management sub-menu.
    /// </summary>
    /// <param name="clientService">The injected client service.</param>
    private static void ManageClients(IClientService clientService)
    {
        Console.WriteLine("\n--- Client Management ---");
        Console.WriteLine("Press 1 to add client");
        Console.WriteLine("Press 2 to see list of clients");
        Console.Write("Your choice: ");
        int choice = int.Parse(Console.ReadLine());
        
        switch (choice)
        {
         case 1:
             Console.WriteLine("Add client");
             Console.WriteLine("Enter client FullName:");
             string fullName = Console.ReadLine();
             
             Console.WriteLine("Enter client email:");
             string email = Console.ReadLine();
             
             Console.WriteLine("Enter client type (1 = Individual, 2 = Company):");
             int typeChoice = int.Parse(Console.ReadLine());
             ClientTypes clientType = (typeChoice == 2) ? ClientTypes.Company : ClientTypes.Individual;

             var newClient = clientService.CreateClient(fullName, email, clientType);
             Console.WriteLine($"Client added! Id: {newClient.Id}");
             break;
             
         case 2:
             Console.WriteLine("--- List of clients ---");
             var clients = clientService.GetAllClients();
             if (clients.Count == 0)
             {
                 Console.WriteLine("No clients found.");
                 break;
             }
             
             foreach (var client in clients)
             {
                 Console.WriteLine($"Id: {client.Id}, Name: {client.FullName}, Email: {client.Email}, Type: {client.ClientType}");
             }
             break;
        }
    }

    /// <summary>
    /// Handles the Policy Management sub-menu.
    /// </summary>
    /// <param name="policyService">The injected policy service.</param>
    private static void ManagePolicies(IPolicyService policyService)
    {
        Console.WriteLine("\n--- Policy Management ---");
        Console.WriteLine("Press 1 to add policy");
        Console.WriteLine("Press 2 to see list of policies");
        Console.Write("Your choice: ");
        int choice = int.Parse(Console.ReadLine());
        
        switch (choice)
        {
            case 1:
                Console.WriteLine("Add policy");
                
                Console.WriteLine("Enter Client Id:");
                int clientId = int.Parse(Console.ReadLine());

                Console.WriteLine("Enter policy type (1=Car, 2=Medical, 3=Property):");
                int typeChoice = int.Parse(Console.ReadLine());
                PolicyTypes policyType = PolicyTypes.CarInsurance;
                if (typeChoice == 2) policyType = PolicyTypes.MedicalInsurance;
                if (typeChoice == 3) policyType = PolicyTypes.PropertyInsurance;

                Console.WriteLine("Enter start date (e.g., 2025-01-30):");
                DateTime startDate = DateTime.Parse(Console.ReadLine());
                
                Console.WriteLine("Enter end date (e.g., 2026-01-30):");
                DateTime endDate = DateTime.Parse(Console.ReadLine());
                
                Console.WriteLine("Enter coverage amount:");
                decimal coverageAmount = decimal.Parse(Console.ReadLine());

                var newPolicy = policyService.CreatePolicy(clientId, policyType, startDate, endDate, coverageAmount);
                
                if (newPolicy != null)
                {
                    Console.WriteLine($"Policy created! Id: {newPolicy.Id}");
                    Console.WriteLine($"Calculated Price: {newPolicy.Price}");
                }
                else
                {
                    Console.WriteLine("Error! Client with this Id might not exist.");
                }
                break;
                
            case 2:
                Console.WriteLine("--- List of policies ---");
                var policies = policyService.GetAllPolicies();
                if (policies.Count == 0)
                {
                    Console.WriteLine("No policies found.");
                    break;
                }
                
                foreach (var policy in policies)
                {
                    Console.WriteLine($"Id: {policy.Id}, ClientId: {policy.ClientId}, Type: {policy.PolicyType}, Price: {policy.Price}, Status: {policy.Status}");
                }
                break;
        }
    }
    
    /// <summary>
    /// Handles the creation of a new claim.
    /// </summary>
    /// <param name="claimService">The injected claim service.</param>
    private static void CreateNewClaim(IClaimService claimService)
    {
        Console.WriteLine("\n--- Create New Claim ---");
        
        Console.WriteLine("Enter Policy Id:");
        int policyId = int.Parse(Console.ReadLine());
        
        Console.WriteLine("Enter event description:");
        string description = Console.ReadLine();
        
        Console.WriteLine("Enter payout amount:");
        decimal payoutAmount = decimal.Parse(Console.ReadLine());

        DateTime date = DateTime.Now;

        var newClaim = claimService.CreateClaim(policyId, date, description, payoutAmount);
        
        Console.WriteLine($"Claim registered! Id: {newClaim.Id} for policy {newClaim.PolicyId}");
    }
}