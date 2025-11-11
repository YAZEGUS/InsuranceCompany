using BusinessLogic;
using Domain;
using Persistence;
using System;
using System.Linq; // Залишаємо, бо використовується у ClientService.GetAllClients().FirstOrDefault()

namespace ConsoleUI;

// Оголошуємо клас як статичний, оскільки він містить лише статичні методи
public static class Program
{
    // --- Service fields for Dependency Injection ---
    private static readonly IClientService ClientService;
    private static readonly IPolicyService PolicyService;
    private static readonly IClaimService ClaimService;

    /// <summary>
    /// Static constructor to set up repositories and services (Dependency Injection).
    /// </summary>
    static Program()
    {
        // Initialize Repositories (Етап 1: Persistence Layer)
        IRepository<Client> clientRepository = new JsonRepository<Client>("clients.json");
        IRepository<Policy> policyRepository = new JsonRepository<Policy>("policies.json");
        IRepository<Claim> claimRepository = new JsonRepository<Claim>("claims.json");
        
        // Ініціалізація залежностей з новими іменами полів 
        ClientService = new ClientService(clientRepository, policyRepository);
        PolicyService = new PolicyService(policyRepository, ClientService);
        ClaimService = new ClaimService(claimRepository, policyRepository, ClientService); 
    }

    /// <summary>
    /// The main entry point for the application.
    /// Runs the main menu loop.
    /// Видалено невикористовуваний параметр 'args'
    /// </summary>
    static void Main()
    {
        Console.WriteLine("--- Система Управління Страхуванням ---");
        
        while (true)
        {
            Console.WriteLine("\n--- ГОЛОВНЕ МЕНЮ ---");
            Console.WriteLine("Натисніть 1 для управління клієнтами");
            Console.WriteLine("Натисніть 2 для управління полісами");
            Console.WriteLine("Натисніть 3 для управління подіями (створення/оновлення)");
            Console.WriteLine("Натисніть 4 для пошуку полісів");
            Console.WriteLine("Натисніть 0 для виходу");
            Console.Write("Ваш вибір: ");
            
            // Nullability Fix: Використовуємо TryParse
            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Некоректне введення. Спробуйте ще раз.");
                continue;
            }
            
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
                case 0:
                    return; 
                default:
                    Console.WriteLine("Некоректний вибір.");
                    break;
            }
        }
    }
    
    /// <summary>
    /// Handles the Client Management sub-menu.
    /// </summary>
    private static void ManageClients(IClientService clientService)
    {
        Console.WriteLine("\n--- Управління Клієнтами ---");
        Console.WriteLine("Натисніть 1, щоб додати клієнта");
        Console.WriteLine("Натисніть 2, щоб переглянути список клієнтів");
        Console.Write("Ваш вибір: ");
        
        // --- Nullability Fix: Використовуємо TryParse ---
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
                // Nullability Fix: Додаємо перевірку на null/порожній рядок 
                string fullName = Console.ReadLine() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(fullName))
                {
                    Console.WriteLine("Ім'я не може бути порожнім.");
                    return;
                }
                
                Console.WriteLine("Введіть email клієнта:");
                string email = Console.ReadLine() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(email))
                {
                    Console.WriteLine("Email не може бути порожнім.");
                    return;
                }

                Console.WriteLine("Введіть тип клієнта (1 = Фізична особа, 2 = Компанія):");
                if (!int.TryParse(Console.ReadLine(), out int typeChoice))
                {
                    Console.WriteLine("Некоректне введення типу.");
                    return;
                }
                ClientTypes clientType = (typeChoice == 2) ? ClientTypes.Company : ClientTypes.Individual;

                var newClient = clientService.CreateClient(fullName, email, clientType);
                Console.WriteLine($"Клієнта додано! Id: {newClient.Id}");
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
                    Console.WriteLine($"Id: {client.Id}, Ім'я: {client.FullName}, Тип: {(client.ClientType == ClientTypes.Individual ? "Фіз. особа" : "Компанія")}");
                    Console.WriteLine($"  Полісів: {client.PolicyCount}, Загальні виплати: {client.TotalPayouts:0.00} ₴");
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

        // --- Nullability Fix: Використовуємо TryParse ---
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

                var newPolicy = policyService.CreatePolicy(clientId, policyType, startDate, endDate, coverageAmount);
                
                if (newPolicy != null)
                {
                    Console.WriteLine($"Поліс створено! Id: {newPolicy.Id}, Вартість: {newPolicy.Price:0.00} ₴");
                }
                else
                {
                    Console.WriteLine("Помилка! Не вдалося створити поліс.");
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
                    Console.WriteLine($"Id: {policy.Id}, Клієнт: {policy.ClientId}, Тип: {policy.PolicyType}, Вартість: {policy.Price:0.00} ₴, Статус: {policy.Status}");
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
                
                policyService.ChangePolicyStatus(policyId, (StatusTypes)statusChoice);
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
                
                claimService.ChangeClaimStatus(claimId, (ClaimStatusTypes)statusChoice);
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
        string description = Console.ReadLine() ?? string.Empty; // Обробка null
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

        var newClaim = claimService.CreateClaim(policyId, date, description, payoutAmount);
        
        if (newClaim != null) // Прибираємо надлишкову перевірку
        {
            Console.WriteLine($"Подія зареєстрована! Id: {newClaim.Id} для поліса {newClaim.PolicyId}");
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
            Console.WriteLine($"Id: {policy.Id}, Клієнт: {policy.ClientId}, Тип: {policy.PolicyType}, Вартість: {policy.Price:0.00} ₴, Статус: {policy.Status}");
        }
    }
}