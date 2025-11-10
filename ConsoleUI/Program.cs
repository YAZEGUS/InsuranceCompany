using BusinessLogic;
using Domain;
using Persistence;
namespace ConsoleUI;

public class Program
{
    static void Main(string[] args)
    {
        IRepository<Client> clientRepository = new JsonRepository<Client>("clients.json");
        IRepository<Policy> policyRepository = new JsonRepository<Policy>("policies.json");
        
        IClientService clientService = new ClientService(clientRepository, policyRepository);
        IPolicyService policyService = new PolicyService(policyRepository, clientRepository);

        while (true)
        {
            Console.WriteLine("Press 1 to manage clients");
            Console.WriteLine("Press 2 to manage policies");
            Console.WriteLine("Press 3 to create policy event");
            Console.WriteLine("Press 0 to exit");
            int choice = int.Parse(Console.ReadLine());
            switch (choice)
            {
                case 1:
                    ManageClients(clientService);
                    break;
                case 2:
                    ManagePolicies(policyService);
                    break;
                case 3:
                    //TODO: Implement policy event creation
                    break;
                case 0:
                    return;
            }
        }
    }
    
    private static void ManageClients(IClientService clientService)
    {
        Console.WriteLine("Manage clients");
        Console.WriteLine("Press 1 to add client");
        Console.WriteLine("Press 2 to see list of clients");
        int choice = int.Parse(Console.ReadLine());
        switch (choice)
        {
         case 1:
             Console.WriteLine("Add client");
             Console.WriteLine("Enter client FullName");
             string fullName = Console.ReadLine();
             Console.WriteLine("Enter client email");
             string email = Console.ReadLine();
             clientService.CreateClient(fullName, email, ClientTypes.Individual);
             
             break;
         case 2:
             Console.WriteLine("List of clients");
             var clients = clientService.GetAllClients();
             foreach (var client in clients)
             {
                 Console.WriteLine($"Id: {client.Id}, Name: {client.FullName}, Email: {client.Email}");
             }
             break;
        }
    }

    private static void ManagePolicies(IPolicyService policyService)
    {
        Console.WriteLine("Manage policies");
        Console.WriteLine("Press 1 to add policy");
        Console.WriteLine("Press 2 to see list of policies");
        int choice = int.Parse(Console.ReadLine());
        switch (choice)
        {
            case 1:
                Console.WriteLine("Add policy");
                Console.WriteLine("Enter ");

                break;
        }
        
    }
}

