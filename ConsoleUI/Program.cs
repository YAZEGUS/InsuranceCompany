using BusinessLogic;
using Domain;
using Persistence;
using System;
using System.Linq; 
using System.Collections.Generic;

namespace ConsoleUI;

/// <summary>
/// The main entry point and user interface for the application.
/// Manages dependency injection setup and the main application loop.
/// </summary>
public static class Program
{
    // --- Service fields for Dependency Injection ---
    private static readonly IClientService ClientService;
    private static readonly IPolicyService PolicyService;
    private static readonly IClaimService ClaimService;
    private static readonly IRepository<Agent> AgentRepository; 

    /// <summary>
    /// Static constructor to set up repositories and services (Dependency Injection).
    /// </summary>
    static Program()
    {
        // Ініціалізація Repositories (Persistence Layer)
        // Створюємо репозиторії як локальні змінні для DI
        IRepository<Client> clientRepository = new JsonRepository<Client>("clients.json");
        IRepository<Policy> policyRepository = new JsonRepository<Policy>("policies.json");
        IRepository<Claim> claimRepository = new JsonRepository<Claim>("claims.json");
        AgentRepository = new JsonRepository<Agent>("agents.json"); 
        
        // Ініціалізація залежностей (Business Logic Layer)
        // Передаємо репозиторії до сервісів
        ClientService = new ClientService(clientRepository, policyRepository);
        PolicyService = new PolicyService(policyRepository, ClientService, AgentRepository);
        ClaimService = new ClaimService(claimRepository, policyRepository, ClientService); 
    }

    /// <summary>
    /// The main entry point for the application.
    /// Runs the main menu loop.
    /// </summary>
    static void Main()
    {
        Console.WriteLine("--- Система Управління Страхуванням ---");
        
        while (true)
        {
            Console.WriteLine("\n--- ГОЛОВНЕ МЕНЮ ---");
            Console.WriteLine("Натисніть 1 для управління клієнтами (CRUD)");
            Console.WriteLine("Натисніть 2 для управління полісами (CRUD/Status)");
            Console.WriteLine("Натисніть 3 для управління подіями (Створення/Status)");
            Console.WriteLine("Натисніть 4 для пошуку полісів (Етап 2)");
            Console.WriteLine("Натисніть 5 для управління агентами (CRUD)"); 
            Console.WriteLine("Натисніть 0 для виходу");
            Console.Write("Ваш вибір: ");
            
            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Некоректне введення. Спробуйте ще раз.");
                continue;
            }
            
            try 
            {
                switch (choice)
                {
                    case 1:
                        ManageClients(ClientService);
                        break;
                    case 2:
                        ManagePolicies(PolicyService);
                        break;
                    case 3:
                        ManageClaims(ClaimService); 
                        break;
                    case 4:
                        SearchPolicies(PolicyService);
                        break;
                    case 5: 
                        ManageAgents(AgentRepository); 
                        break;
                    case 0:
                        return; 
                    default:
                        Console.WriteLine("Некоректний вибір.");
                        break;
                }
            }
            catch (Exception ex)
            {
                // Загальна обробка непередбачених помилок
                Console.WriteLine($"\nКритична помилка програми: {ex.Message}");
            }
        }
    }
    
    /// <summary>
    /// Handles the Client Management sub-menu.
    /// </summary>
    // !!! ЗМІНЕНО: Приймає лише ClientService
    private static void ManageClients(IClientService clientService)
    {
        Console.WriteLine("\n--- Управління Клієнтами ---");
        Console.WriteLine("Натисніть 1, щоб додати клієнта");
        Console.WriteLine("Натисніть 2, щоб переглянути список клієнтів");
        Console.WriteLine("Натисніть 3, щоб видалити клієнта (Delete)"); 
        Console.Write("Ваш вибір: ");
        
        if (!int.TryParse(Console.ReadLine(), out int choice))
        {
            Console.WriteLine("Некоректне введення числа.");
            return;
        }
        
        switch (choice)
        {
           case 1:
                Console.WriteLine("Додавання клієнта");
                Console.WriteLine("Введіть повне ім'я клієнта:");
                string fullName = Console.ReadLine() ?? string.Empty;
                
                Console.WriteLine("Введіть email клієнта:");
                string email = Console.ReadLine() ?? string.Empty;

                Console.WriteLine("Введіть тип клієнта (1 = Individual, 2 = Company):");
                if (!int.TryParse(Console.ReadLine(), out int typeChoice) || !Enum.IsDefined(typeof(ClientTypes), typeChoice - 1))
                {
                    Console.WriteLine("Некоректний вибір типу клієнта.");
                    return;
                }
                ClientTypes clientType = (ClientTypes)(typeChoice - 1);

                try
                {
                    var newClient = clientService.CreateClient(fullName, email, clientType);
                    Console.WriteLine($"Клієнта додано! Id: {newClient.Id}");
                }
                catch (ArgumentException ex) 
                {
                    Console.WriteLine($"Помилка додавання клієнта: {ex.Message}");
                }
                break;
                
           case 2:
                Console.WriteLine("--- Список клієнтів ---");
                var clients = clientService.GetAllClients();
                if (clients.Count == 0)
                {
                    Console.WriteLine("Клієнтів не знайдено.");
                    break;
                }
                
                foreach (var client in clients)
                {
                    Console.WriteLine($"Id: {client.Id}, Ім'я: {client.FullName}, Тип: {client.ClientType}");
                    Console.WriteLine($"  Полісів: {client.PolicyCount}, Загальні виплати: {client.TotalPayouts:0.00} ₴");
                }
                break;
            
            case 3: // ЛОГІКА DELETE КЛІЄНТА
                Console.WriteLine("--- Видалення Клієнта ---");
                Console.WriteLine("Введіть Id Клієнта для видалення:");
                if (!int.TryParse(Console.ReadLine(), out int deleteClientId))
                {
                    Console.WriteLine("Некоректний Id клієнта.");
                    return;
                }
                
                try
                {
                    // Викликаємо метод сервісу, який обробляє бізнес-правило (перевірка активних полісів)
                    if (clientService.DeleteClient(deleteClientId))
                    {
                        Console.WriteLine($"Клієнта з Id {deleteClientId} успішно видалено.");
                    }
                    else
                    {
                        Console.WriteLine($"Помилка: Клієнта з Id {deleteClientId} не знайдено.");
                    }
                }
                catch (ArgumentException ex) 
                {
                    // Ловимо виняток з BusinessLogic, якщо клієнт має активні поліси
                    Console.WriteLine($"Помилка видалення клієнта: {ex.Message}");
                }
                break;
        }
    }

    /// <summary>
    /// Handles the Policy Management sub-menu.
    /// </summary>
    private static void ManagePolicies(IPolicyService policyService)
    {
        Console.WriteLine("\n--- Управління Полісами ---");
        Console.WriteLine("Натисніть 1, щоб додати поліс");
        Console.WriteLine("Натисніть 2, щоб переглянути список полісів");
        Console.WriteLine("Натисніть 3, щоб змінити статус поліса");
        Console.Write("Ваш вибір: ");

        if (!int.TryParse(Console.ReadLine(), out int choice))
        {
            Console.WriteLine("Некоректне введення числа.");
            return;
        }
        
        switch (choice)
        {
            case 1:
                Console.WriteLine("Додавання поліса");
                
                Console.WriteLine("Введіть Id Клієнта:");
                if (!int.TryParse(Console.ReadLine(), out int clientId))
                {
                    Console.WriteLine("Некоректний Id клієнта.");
                    return;
                }
                
                int? agentId = null;
                Console.WriteLine("Введіть Id Агента (або Enter, якщо немає):");
                string agentIdInput = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(agentIdInput) && int.TryParse(agentIdInput, out int agentValue))
                {
                    agentId = agentValue;
                }

                Console.WriteLine("Введіть тип поліса (1=Авто, 2=Медичне, 3=Майно):");
                if (!int.TryParse(Console.ReadLine(), out int typeChoice) || !Enum.IsDefined(typeof(PolicyTypes), typeChoice - 1))
                {
                    Console.WriteLine("Некоректний вибір типу поліса.");
                    return;
                }
                PolicyTypes policyType = (PolicyTypes)(typeChoice - 1); 

                Console.WriteLine("Введіть дату початку (напр., 2025-01-30):");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
                {
                    Console.WriteLine("Некоректний формат дати початку.");
                    return;
                }
                
                Console.WriteLine("Введіть дату закінчення (напр., 2026-01-30):");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
                {
                    Console.WriteLine("Некоректний формат дати закінчення.");
                    return;
                }
                
                Console.WriteLine("Введіть суму покриття:");
                if (!decimal.TryParse(Console.ReadLine(), out decimal coverageAmount))
                {
                    Console.WriteLine("Некоректне введення суми покриття.");
                    return;
                }

                try
                {
                    var newPolicy = policyService.CreatePolicy(clientId, agentId, policyType, startDate, endDate, coverageAmount);
                    Console.WriteLine($"Поліс створено! Id: {newPolicy.Id}, Вартість: {newPolicy.Price:0.00} ₴");
                }
                catch (ArgumentException ex) 
                {
                    Console.WriteLine($"Помилка створення поліса: {ex.Message}");
                }
                break;
                
            case 2:
                Console.WriteLine("--- Список полісів ---");
                var policies = policyService.GetAllPolicies();
                if (policies.Count == 0)
                {
                    Console.WriteLine("Полісів не знайдено.");
                    break;
                }
                
                foreach (var policy in policies)
                {
                    string agentInfo = policy.AgentId.HasValue ? $", Агент: {policy.AgentId}" : "";
                    Console.WriteLine($"Id: {policy.Id}, Клієнт: {policy.ClientId}{agentInfo}, Тип: {policy.PolicyType}, Вартість: {policy.Price:0.00} ₴, Статус: {policy.Status}");
                }
                break;
            
            case 3:
                Console.WriteLine("--- Зміна Статусу Поліса ---");
                Console.WriteLine("Введіть Id Поліса:");
                if (!int.TryParse(Console.ReadLine(), out int policyId))
                {
                    Console.WriteLine("Некоректний Id поліса.");
                    return;
                }

                Console.WriteLine("Введіть новий статус (0=Активний, 1=Призупинено, 2=Завершено, 3=Скасовано):");
                if (!int.TryParse(Console.ReadLine(), out int statusChoice) || !Enum.IsDefined(typeof(StatusTypes), statusChoice))
                {
                    Console.WriteLine("Некоректний вибір статусу.");
                    return;
                }
                
                try
                {
                    policyService.ChangePolicyStatus(policyId, (StatusTypes)statusChoice);
                    Console.WriteLine($"Статус поліса {policyId} успішно змінено на {(StatusTypes)statusChoice}."); 
                }
                catch (ArgumentException ex) 
                {
                    Console.WriteLine($"Помилка зміни статусу: {ex.Message}");
                }
                break;
        }
    }

    /// <summary>
    /// Handles the Claim Management sub-menu.
    /// </summary>
    private static void ManageClaims(IClaimService claimService)
    {
        Console.WriteLine("\n--- Управління Подіями ---");
        Console.WriteLine("Натисніть 1, щоб створити нову подію (Етап 1)");
        Console.WriteLine("Натисніть 2, щоб змінити статус події (Етап 2)");
        Console.Write("Ваш вибір: ");

        if (!int.TryParse(Console.ReadLine(), out int choice))
        {
            Console.WriteLine("Некоректне введення. Спробуйте ще раз.");
            return;
        }

        switch (choice)
        {
            case 1:
                CreateNewClaim(claimService); 
                break;
            case 2:
                Console.WriteLine("--- Зміна Статусу Події ---");
                Console.WriteLine("Введіть Id Події:");
                if (!int.TryParse(Console.ReadLine(), out int claimId))
                {
                    Console.WriteLine("Некоректний Id події.");
                    return;
                }

                Console.WriteLine("Введіть новий статус (0=Нова, 1=На розгляді, 2=Затверджено, 3=Виплачено):");
                if (!int.TryParse(Console.ReadLine(), out int statusChoice) || !Enum.IsDefined(typeof(ClaimStatusTypes), statusChoice))
                {
                    Console.WriteLine("Некоректний вибір статусу.");
                    return;
                }
                
                try
                {
                    claimService.ChangeClaimStatus(claimId, (ClaimStatusTypes)statusChoice);
                    Console.WriteLine($"Статус події {claimId} успішно змінено на {(ClaimStatusTypes)statusChoice}.");
                }
                catch (ArgumentException ex) 
                {
                    Console.WriteLine($"Помилка зміни статусу події: {ex.Message}");
                }
                break;
        }
    }

    /// <summary>
    /// Handles the creation of a new claim.
    /// </summary>
    private static void CreateNewClaim(IClaimService claimService)
    {
        Console.WriteLine("\n--- Створення Нової Події ---");
        
        Console.WriteLine("Введіть Id Поліса:");
        if (!int.TryParse(Console.ReadLine(), out int policyId))
        {
            Console.WriteLine("Некоректний Id поліса.");
            return;
        }
        
        Console.WriteLine("Введіть опис події:");
        string description = Console.ReadLine() ?? string.Empty; 
        if (string.IsNullOrWhiteSpace(description))
        {
            Console.WriteLine("Опис не може бути порожнім.");
            return;
        }
        
        Console.WriteLine("Введіть суму виплати:");
        if (!decimal.TryParse(Console.ReadLine(), out decimal payoutAmount))
        {
            Console.WriteLine("Некоректне введення суми виплати.");
            return;
        }

        DateTime date = DateTime.Now;

        try
        {
            var newClaim = claimService.CreateClaim(policyId, date, description, payoutAmount);
            Console.WriteLine($"Подія зареєстрована! Id: {newClaim.Id} для поліса {newClaim.PolicyId}");
        }
        catch (ArgumentException ex) 
        {
            Console.WriteLine($"Помилка реєстрації події: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Handles the Agent Management sub-menu.
    /// </summary>
    private static void ManageAgents(IRepository<Agent> agentRepository)
    {
        Console.WriteLine("\n--- Управління Агентами ---");
        Console.WriteLine("Натисніть 1, щоб додати агента");
        Console.WriteLine("Натисніть 2, щоб переглянути список агентів");
        Console.WriteLine("Натисніть 3, щоб видалити агента (Delete)"); 
        Console.Write("Ваш вибір: ");
        
        if (!int.TryParse(Console.ReadLine(), out int choice))
        {
            Console.WriteLine("Некоректне введення числа.");
            return;
        }
        
        switch (choice)
        {
           case 1:
                Console.WriteLine("Додавання агента");
                Console.WriteLine("Введіть ім'я агента:");
                string name = Console.ReadLine() ?? string.Empty;
                
                Console.WriteLine("Введіть відсоток комісії (напр., 0.15 для 15%):");
                if (!decimal.TryParse(Console.ReadLine(), out decimal commission))
                {
                    Console.WriteLine("Некоректне введення комісії.");
                    return;
                }
                
                if (commission < 0m || commission > 1m)
                {
                    Console.WriteLine("Комісія повинна бути між 0.00 та 1.00 (напр., 0.15).");
                    return;
                }

                try
                {
                    var newAgent = new Agent { Name = name, CommissionPercentage = commission };
                    agentRepository.Add(newAgent);
                    Console.WriteLine($"Агента додано! Id: {newAgent.Id}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка додавання агента: {ex.Message}");
                }
                break;
                
           case 2:
                Console.WriteLine("--- Список агентів ---");
                var agents = agentRepository.GetAll();
                if (agents.Count == 0)
                {
                    Console.WriteLine("Агентів не знайдено.");
                    break;
                }
                
                foreach (var agent in agents)
                {
                    Console.WriteLine($"Id: {agent.Id}, Ім'я: {agent.Name}, Комісія: {agent.CommissionPercentage:P2}");
                }
                break;
            
            case 3: // ЛОГІКА DELETE АГЕНТА (Викликається напряму на репозиторії)
                Console.WriteLine("--- Видалення Агента ---");
                Console.WriteLine("Введіть Id Агента для видалення:");
                if (!int.TryParse(Console.ReadLine(), out int deleteAgentId))
                {
                    Console.WriteLine("Некоректний Id агента.");
                    return;
                }
                
                if (agentRepository.Delete(deleteAgentId))
                {
                    Console.WriteLine($"Агента з Id {deleteAgentId} успішно видалено.");
                }
                else
                {
                    Console.WriteLine($"Помилка: Агента з Id {deleteAgentId} не знайдено.");
                }
                break;
        }
    }

    /// <summary>
    /// Handles the Policy Search functionality.
    /// </summary>
    private static void SearchPolicies(IPolicyService policyService)
    {
        Console.WriteLine("\n--- Пошук Полісів ---");
        
        PolicyTypes? type = null;
        StatusTypes? status = null;
        int? clientId = null;
        decimal? minPrice = null;
        decimal? maxPrice = null;

        Console.WriteLine("Фільтр за типом поліса (1=Авто, 2=Медичне, 3=Майно, або ENTER для будь-якого):");
        string typeInput = Console.ReadLine();
        if (int.TryParse(typeInput, out int typeChoice) && Enum.IsDefined(typeof(PolicyTypes), typeChoice - 1))
        {
            type = (PolicyTypes)(typeChoice - 1);
        }
        
        
        Console.WriteLine("Фільтр за Id Клієнта (або ENTER для будь-якого):");
        string clientInput = Console.ReadLine();
        if (int.TryParse(clientInput, out int clientValue))
        {
            clientId = clientValue;
        }

        Console.WriteLine("Фільтр за статусом (0=Активний, 1=Призупинено, 2=Завершено, 3=Скасовано, або ENTER для будь-якого):");
        string statusInput = Console.ReadLine();
        if (int.TryParse(statusInput, out int statusChoice) && Enum.IsDefined(typeof(StatusTypes), statusChoice))
        {
            status = (StatusTypes)statusChoice;
        }

        Console.WriteLine("Фільтр за мінімальною вартістю (або ENTER для будь-якої):");
        string minPriceInput = Console.ReadLine();
        if (decimal.TryParse(minPriceInput, out decimal minPriceValue))
        {
            minPrice = minPriceValue;
        }

        Console.WriteLine("Фільтр за максимальною вартістю (або ENTER для будь-якої):");
        string maxPriceInput = Console.ReadLine();
        if (decimal.TryParse(maxPriceInput, out decimal maxPriceValue))
        {
            maxPrice = maxPriceValue;
        }

        // Виконання пошуку
        var foundPolicies = policyService.SearchPolicies(type, clientId, status, minPrice, maxPrice);

        Console.WriteLine($"\n--- Знайдено полісів ({foundPolicies.Count}) ---");
        if (foundPolicies.Count == 0)
        {
            Console.WriteLine("Полісів, що відповідають критеріям, не знайдено.");
            return;
        }

        foreach (var policy in foundPolicies)
        {
            string agentInfo = policy.AgentId.HasValue ? $", Агент: {policy.AgentId}" : "";
            Console.WriteLine($"Id: {policy.Id}, Клієнт: {policy.ClientId}{agentInfo}, Тип: {policy.PolicyType}, Вартість: {policy.Price:0.00} ₴, Статус: {policy.Status}");
        }
    }
}