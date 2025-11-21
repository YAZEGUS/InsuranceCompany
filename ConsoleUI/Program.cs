using Domain;
using Persistence;
using BusinessLogic.Interfaces;
using BusinessLogic.Services;

namespace ConsoleUI;

/// <summary>
/// The main entry point and user interface for the application.
/// Manages dependency injection setup and the main application loop, now supporting user roles (Stage 6).
/// </summary>
public static class Program
{   
    // --- Service fields for Dependency Injection ---
    private static readonly IClientService ClientService;
    private static readonly IPolicyService PolicyService;
    private static readonly IClaimService ClaimService;
    private static readonly IRepository<Agent> AgentRepository; 
    private static readonly IRequestService RequestService;
    private static readonly IAnalyticsService AnalyticsService;
    private static readonly IPaymentService PaymentService; 
    private static readonly HttpClient HttpClient;
    private static readonly ICurrencyRateService CurrencyRateService;

    /// <summary>
    /// Static constructor to set up repositories and services (Dependency Injection).
    /// </summary>
    static Program()
    {
        // Створення контексту БД та HTTP клієнта
        var dbContext = new AppDbContext();
        HttpClient = new HttpClient(); 
        
        IRepository<Client> clientRepository = new SqlRepository<Client>(dbContext);
        IRepository<Policy> policyRepository = new SqlRepository<Policy>(dbContext);
        IRepository<Claim> claimRepository = new SqlRepository<Claim>(dbContext);
        AgentRepository = new SqlRepository<Agent>(dbContext); 
        IRepository<Request> requestRepository = new SqlRepository<Request>(dbContext);
        IRepository<Payment> paymentRepository = new SqlRepository<Payment>(dbContext); 
        
        // Ініціалізація залежностей (Business Logic Layer)
        CurrencyRateService = new CurrencyRateService(HttpClient);
        ClientService = new ClientService(clientRepository, policyRepository);
        PolicyService = new PolicyService(policyRepository, ClientService, AgentRepository);
        ClaimService = new ClaimService(claimRepository, policyRepository, ClientService);
        RequestService = new RequestService(requestRepository, policyRepository); 
        AnalyticsService = new AnalyticsService(policyRepository, claimRepository, AgentRepository);
        PaymentService = new PaymentService(paymentRepository, policyRepository, CurrencyRateService);
    }

    /// <summary>
    /// The main entry point for the application.
    /// Runs the main menu loop for role selection (Stage 6).
    /// </summary>
    static async Task Main()
    {
        Console.WriteLine("--- Система Управління Страхуванням ---");
        
        while (true)
        {
            Console.WriteLine("\n--- ВИБІР РОЛІ ---");
            Console.WriteLine("1. Клієнт (Перегляд своїх даних, Запити)");
            Console.WriteLine("2. Агент (Продажі, Створення)");
            Console.WriteLine("3. Менеджер (Повний доступ, Аналітика)");
            Console.WriteLine("0. Вихід");
            Console.Write("Ваш вибір: ");
            
            if (!int.TryParse(Console.ReadLine(), out int roleChoice))
            {
                Console.WriteLine("Некоректне введення. Спробуйте ще раз.");
                continue;
            }
            
            try 
            {
                switch (roleChoice)
                {
                    case 1:
                        await RunClientMenu();
                        break;
                    case 2:
                        await RunAgentMenu();
                        break;
                    case 3:
                        await RunManagerMenu();
                        break;
                    case 0:
                        return; 
                    default:
                        Console.WriteLine("Некоректний вибір ролі.");
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
    
 
    // роль: клієнта
    private static async Task RunClientMenu()
    {
        Console.Write("Введіть Ваш ID Клієнта: ");
        if (!int.TryParse(Console.ReadLine(), out int clientId)) return;
        
        var client = ClientService.GetClientById(clientId);
        if (client == null) 
        {
            Console.WriteLine("Клієнта з таким ID не знайдено.");
            return;
        }

        while (true)
        {
            Console.WriteLine($"\n--- МЕНЮ КЛІЄНТА: {client.FullName} ---");
            Console.WriteLine("1. Переглянути мої поліси та події");
            Console.WriteLine("2. Створити новий запит на поліс");
            Console.WriteLine("3. Переглянути історію моїх запитів");
            Console.WriteLine("4. Підібрати поліс за останнім запитом");
            Console.WriteLine("0. Повернутися до вибору ролі");
            Console.Write("Ваш вибір: ");

            if (!int.TryParse(Console.ReadLine(), out int choice)) continue;
            
            try
            {
                switch (choice)
                {
                    case 1:
                        // Клієнт бачить лише свої поліси
                        ShowPolicies(PolicyService.SearchPolicies(clientId: clientId), showClaims: true);
                        break;
                    case 2:
                        CreateNewRequest(RequestService, clientId);
                        break;
                    case 3:
                        ShowClientRequests(RequestService, clientId);
                        break;
                    case 4:
                        MatchLastClientRequest(RequestService, clientId);
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
                Console.WriteLine($"Помилка Клієнта: {ex.Message}");
            }
        }
    }
    
   
    // роль агента         
    private static async Task RunAgentMenu()
    {
        Console.Write("Введіть Ваш ID Агента: ");
        if (!int.TryParse(Console.ReadLine(), out int agentId)) return;

        var agent = AgentRepository.GetById(agentId);
        if (agent == null)
        {
            Console.WriteLine("Агента з таким ID не знайдено.");
            return;
        }

        while (true)
        {
            Console.WriteLine($"\n--- МЕНЮ АГЕНТА: {agent.Name} ---");
            Console.WriteLine("1. Додати нового клієнта");
            Console.WriteLine("2. Оформити новий поліс (для клієнта)");
            Console.WriteLine("3. Зареєструвати страхову подію");
            Console.WriteLine("4. Переглянути МОЇ продані поліси");
            Console.WriteLine("5. Моя статистика продажів");
            Console.WriteLine("0. Повернутися до вибору ролі");
            Console.Write("Ваш вибір: ");

            if (!int.TryParse(Console.ReadLine(), out int choice)) continue;

            try
            {
                switch (choice)
                {
                    case 1:
                        CreateNewClient(ClientService);
                        break;
                    case 2:
                        // Створення поліса з фіксацією AgentId
                        await CreateNewPolicy(PolicyService, agentId); 
                        break;
                    case 3:
                        CreateNewClaim(ClaimService); 
                        break;
                    case 4:
                        // Агент бачить лише свої поліси
                        ShowPolicies(PolicyService.SearchPolicies(agentId: agentId), showClaims: false);
                        break;
                    case 5:
                        // Агент бачить лише свою статистику
                        ShowAgentStats(AnalyticsService, agentId); 
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
                Console.WriteLine($"Помилка Агента: {ex.Message}");
            }
        }
    }
    

    //   роль: менеджер
    private static async Task RunManagerMenu()
    {
        while (true)
        {
            Console.WriteLine("\n--- МЕНЮ МЕНЕДЖЕРА (ПОВНИЙ ДОСТУП) ---");
            Console.WriteLine("1. Управління Клієнтами");
            Console.WriteLine("2. Управління Полісами");
            Console.WriteLine("3. Управління Агентами");
            Console.WriteLine("4. Управління Подіями");
            Console.WriteLine("5. Управління Платежами");
            Console.WriteLine("6. Пошук Полісів");
            Console.WriteLine("7. Повна Аналітика та Звіти");
            Console.WriteLine("0. Повернутися до вибору ролі");
            Console.Write("Ваш вибір: ");

            if (!int.TryParse(Console.ReadLine(), out int choice)) continue;

            try
            {
                switch (choice)
                {
                    case 1:
                        ManageClients(ClientService);
                        break;
                    case 2:
                        await ManagePolicies(PolicyService);
                        break;
                    case 3:
                        ManageAgents(AgentRepository);
                        break;
                    case 4:
                        ManageClaims(ClaimService); 
                        break;
                    case 5: 
                        await ManagePayments(PaymentService); 
                        break;
                    case 6:
                        SearchPolicies(PolicyService);
                        break;
                    case 7:
                        ShowAnalytics(AnalyticsService);
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
                Console.WriteLine($"Помилка Менеджера: {ex.Message}");
            }
        }
    }
    

    // домоміжні методи (переаикористання)
    // --- Управління Клієнтами (CRUD) ---
    private static void ManageClients(IClientService clientService)
    {
        Console.WriteLine("\n--- Управління Клієнтами ---");
        Console.WriteLine("1. Додати клієнта (Create)");
        Console.WriteLine("2. Переглянути список клієнтів (Read)");
        Console.WriteLine("3. Видалити клієнта (Delete)"); 
        Console.Write("Ваш вибір: ");
        
        if (!int.TryParse(Console.ReadLine(), out int choice)) return;
        
        switch (choice)
        {
           case 1:
                CreateNewClient(clientService);
                break;
           case 2:
                ShowAllClients(clientService);
                break;
            case 3: 
                DeleteClient(clientService);
                break;
        }
    }

    private static void CreateNewClient(IClientService clientService)
    {
        Console.WriteLine("Додавання клієнта");
        Console.Write("Введіть повне ім'я клієнта: ");
        string fullName = Console.ReadLine() ?? string.Empty;
        
        Console.Write("Введіть email клієнта: ");
        string email = Console.ReadLine() ?? string.Empty;

        Console.Write("Введіть тип клієнта (0 = Individual, 1 = Company): ");
        if (!int.TryParse(Console.ReadLine(), out int typeChoice) || !Enum.IsDefined(typeof(ClientTypes), typeChoice))
        {
            Console.WriteLine("Некоректний вибір типу клієнта.");
            return;
        }
        ClientTypes clientType = (ClientTypes)typeChoice;

        try
        {
            var newClient = clientService.CreateClient(fullName, email, clientType);
            Console.WriteLine($"Клієнта додано! Id: {newClient.Id}");
        }
        catch (ArgumentException ex) 
        {
            Console.WriteLine($"Помилка додавання клієнта: {ex.Message}");
        }
    }

    private static void ShowAllClients(IClientService clientService)
    {
        Console.WriteLine("--- Список клієнтів ---");
        var clients = clientService.GetAllClients();
        if (clients.Count == 0)
        {
            Console.WriteLine("Клієнтів не знайдено.");
            return;
        }
        
        foreach (var client in clients)
        {
            Console.WriteLine($"Id: {client.Id}, Ім'я: {client.FullName}, Тип: {client.ClientType}");
            Console.WriteLine($"  Полісів: {client.PolicyCount}, Загальні виплати: {client.TotalPayouts:0.00} ₴");
        }
    }

    private static void DeleteClient(IClientService clientService)
    {
        Console.WriteLine("--- Видалення Клієнта ---");
        Console.Write("Введіть Id Клієнта для видалення: ");
        if (!int.TryParse(Console.ReadLine(), out int deleteClientId))
        {
            Console.WriteLine("Некоректний Id клієнта.");
            return;
        }
        
        try
        {
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
            Console.WriteLine($"Помилка видалення клієнта: {ex.Message}");
        }
    }


    // --- Управління Полісами (CRUD & Status) ---
    private static async Task ManagePolicies(IPolicyService policyService)
    {
        Console.WriteLine("\n--- Управління Полісами ---");
        Console.WriteLine("1. Додати поліс");
        Console.WriteLine("2. Список полісів");
        Console.WriteLine("3. Змінити статус");
        Console.Write("Ваш вибір: ");

        if (!int.TryParse(Console.ReadLine(), out int choice)) return;
        
        switch (choice)
        {
            case 1:
                await CreateNewPolicy(policyService, null); // Менеджер може створити без агента
                break;
            case 2:
                ShowPolicies(policyService.GetAllPolicies(), showClaims: false);
                break;
            case 3:
                ChangePolicyStatus(policyService);
                break;
        }
    }

    private static void ChangePolicyStatus(IPolicyService policyService)
    {
        Console.Write("Id Поліса: "); 
        if (!int.TryParse(Console.ReadLine(), out int pId)) return;
        Console.Write("Статус (0=Active, 1=Paused, 2=Completed, 3=Cancelled): "); 
        if (!int.TryParse(Console.ReadLine(), out int statusInt) || !Enum.IsDefined(typeof(StatusTypes), statusInt)) return;
        
        try 
        { 
            policyService.ChangePolicyStatus(pId, (StatusTypes)statusInt); 
            Console.WriteLine("Статус змінено."); 
        }
        catch (Exception ex) 
        { 
            Console.WriteLine($"Помилка: {ex.Message}"); 
        }
    }

    private static async Task CreateNewPolicy(IPolicyService policyService, int? fixedAgentId)
    {
        Console.WriteLine("--- Створення поліса ---");
        Console.Write("Id Клієнта: "); 
        if (!int.TryParse(Console.ReadLine(), out int clientId)) return;
        
        int? agentId = fixedAgentId;
        if (!fixedAgentId.HasValue) 
        {
            Console.Write("Id Агента (Enter якщо немає): "); 
            string agentIn = Console.ReadLine();
            if (!string.IsNullOrEmpty(agentIn) && int.TryParse(agentIn, out int aId))
            {
                agentId = aId;
            }
        }

        Console.Write("Тип (0=Car, 1=Med, 2=Prop): "); 
        if (!int.TryParse(Console.ReadLine(), out int typeInt) || !Enum.IsDefined(typeof(PolicyTypes), typeInt)) return;
        PolicyTypes type = (PolicyTypes)typeInt;

        Console.Write("Дата початку (yyyy-mm-dd): "); 
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime start)) return;
        Console.Write("Дата кінця (yyyy-mm-dd): "); 
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime end)) return;
        Console.Write("Сума покриття (UAH): "); 
        if (!decimal.TryParse(Console.ReadLine(), out decimal coverage)) return;

        try
        {
            var newPolicy = policyService.CreatePolicy(clientId, agentId, type, start, end, coverage);
            Console.WriteLine($"Поліс створено! Id: {newPolicy.Id}, Ціна: {newPolicy.Price:0.00} UAH");
            
            // API BONUS: Показуємо ціну в доларах для інформації
            try 
            {
                decimal rate = await CurrencyRateService.GetExchangeRateAsync("UAH", "USD");
                // УВАГА: Якщо API повертає курс USD/UAH, то треба ділити, як тут.
                // Якщо API повертає курс UAH/USD, то треба множити. Припускаємо ділення.
                decimal priceUsd = newPolicy.Price / rate; 
                Console.WriteLine($"Орієнтовна ціна в USD: ${priceUsd:0.00} (Курс: {rate})");
            }
            catch
            {
                Console.WriteLine("   (Не вдалося завантажити курс валют для довідки)");
            }
        }
        catch (Exception ex) 
        {
            Console.WriteLine($"Помилка: {ex.Message}");
        }
    }

    private static void ShowPolicies(List<Policy> policies, bool showClaims)
    {
        Console.WriteLine($"\n--- Список полісів ({policies.Count}) ---");
        if (policies.Count == 0)
        {
            Console.WriteLine("Полісів не знайдено.");
            return;
        }

        foreach (var p in policies)
        {
            string agentInfo = p.AgentId.HasValue ? $", Агент: {p.AgentId}" : "";
            Console.WriteLine($"Id: {p.Id}, Клієнт: {p.ClientId}{agentInfo}, Тип: {p.PolicyType}, Покриття: {p.CoverageAmount:0.00}, Вартість: {p.Price:0.00}, Статус: {p.Status}");
            
            if (showClaims)
            {
                // Для клієнта показуємо його події
                // ClaimService.GetAll() повертає List<Claim>, .Where() та .ToList() вимагають System.Linq (який є)
                var claims = ClaimService.GetAll().Where(c => c.PolicyId == p.Id).ToList();
                if (claims.Any())
                {
                    Console.WriteLine("Події:");
                    foreach (var c in claims)
                    {
                        Console.WriteLine($"     [Claim Id: {c.Id}] Дата: {c.Date:d}, Сума: {c.PayoutAmount:0.00}, Статус: {c.Status}");
                    }
                }
            }
        }
    }

    // --- Управління Подіями (Менеджер: Затвердження) ---
    private static void ManageClaims(IClaimService claimService)
    {
        Console.WriteLine("\n--- Управління Подіями ---");
        Console.WriteLine("1. Створити нову подію");
        Console.WriteLine("2. Змінити статус події (Затвердження/Виплата)");
        Console.Write("Ваш вибір: ");

        if (!int.TryParse(Console.ReadLine(), out int choice)) return;

        switch (choice)
        {
            case 1:
                CreateNewClaim(claimService); 
                break;
            case 2:
                ChangeClaimStatus(claimService);
                break;
        }
    }

    private static void CreateNewClaim(IClaimService claimService)
    {
        Console.WriteLine("\n--- Створення Нової Події ---");
        
        Console.Write("Введіть Id Поліса: ");
        if (!int.TryParse(Console.ReadLine(), out int policyId)) return;
        
        Console.Write("Введіть опис події: ");
        string description = Console.ReadLine() ?? string.Empty; 
        
        Console.Write("Введіть суму виплати: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal payoutAmount)) return;

        try
        {
            var newClaim = claimService.CreateClaim(policyId, DateTime.Now, description, payoutAmount);
            Console.WriteLine($"Подія зареєстрована! Id: {newClaim.Id} для поліса {newClaim.PolicyId}");
        }
        catch (ArgumentException ex) 
        {
            Console.WriteLine($"Помилка реєстрації події: {ex.Message}");
        }
    }
    
    // Новий метод для зміни статусу події (доступний лише Менеджеру)
    private static void ChangeClaimStatus(IClaimService claimService)
    {
        Console.WriteLine("--- Зміна Статусу Події ---");
        Console.Write("Введіть Id Події: ");
        if (!int.TryParse(Console.ReadLine(), out int claimId)) return;

        Console.Write("Введіть новий статус (0=Нова, 1=На розгляді, 2=Затверджено, 3=Виплачено): ");
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
    }


    // --- Управління Агентами (Менеджер: CRUD) ---
    private static void ManageAgents(IRepository<Agent> agentRepository)
    {
        Console.WriteLine("\n--- Управління Агентами ---");
        Console.WriteLine("1. Додати агента");
        Console.WriteLine("2. Переглянути список агентів");
        Console.WriteLine("3. Видалити агента"); 
        Console.Write("Ваш вибір: ");
        
        if (!int.TryParse(Console.ReadLine(), out int choice)) return;
        
        switch (choice)
        {
           case 1:
                Console.WriteLine("Додавання агента");
                Console.Write("Введіть ім'я агента: ");
                string name = Console.ReadLine() ?? string.Empty;
                
                Console.Write("Введіть відсоток комісії (напр., 0.15): ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal commission)) return;
                
                if (commission < 0m || commission > 1m)
                {
                    Console.WriteLine("Комісія повинна бути між 0.00 та 1.00.");
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
            
            case 3: 
                Console.WriteLine("--- Видалення Агента ---");
                Console.Write("Введіть Id Агента для видалення: ");
                if (!int.TryParse(Console.ReadLine(), out int deleteAgentId)) return;
                
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

    // --- Управління Запитами (Клієнт: Створення, Менеджер: Перегляд) ---

    // Для Клієнта (створює для себе)
    private static void CreateNewRequest(IRequestService requestService, int clientId)
    {
        Console.WriteLine("\n--- Створення нового запиту ---");
        Console.WriteLine("Тип поліса (0=Car, 1=Med, 2=Prop):");
        if (!int.TryParse(Console.ReadLine(), out int typeChoice) || !Enum.IsDefined(typeof(PolicyTypes), typeChoice)) return;
        PolicyTypes type = (PolicyTypes)typeChoice;

        Console.Write("Бажана сума покриття: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount)) return;
        
        Console.Write("Бажаний строк дії в місяцях: ");
        if (!int.TryParse(Console.ReadLine(), out int durationInMonths)) return;

        try
        {
            requestService.CreateRequest(clientId, type, amount, durationInMonths);
            Console.WriteLine("Запит успішно збережено!");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
        }
    }

    // Для Клієнта (перегляд своїх запитів)
    private static void ShowClientRequests(IRequestService requestService, int clientId)
    {
        Console.WriteLine("--- Мої Запити ---");
        var requests = requestService.GetClientRequests(clientId);
        if (requests.Count == 0)
        {
            Console.WriteLine("У вас немає запитів.");
            return;
        }
        foreach (var req in requests)
        {
            Console.WriteLine(
                $"[ID: {req.Id}] [Дата: {req.CreationDate:d}] Тип: {req.PolicyType} | Сума: {req.DesiredCoverageAmount:0.00} ₴ | Термін: {req.DurationInMonths} міс.");
        }
    }
    
    // Для Клієнта (підбір за останнім запитом)
    private static void MatchLastClientRequest(IRequestService requestService, int clientId)
    {
        var lastRequest = requestService.GetClientRequests(clientId).OrderByDescending(r => r.CreationDate).FirstOrDefault();
        
        if (lastRequest == null)
        {
            Console.WriteLine("Спочатку створіть хоча б один запит.");
            return;
        }

        try
        {
            Console.WriteLine($"\n--- Підбір Полісів за Запитом #{lastRequest.Id} ({lastRequest.PolicyType} / {lastRequest.DesiredCoverageAmount:0.00} ₴) ---");
            var matchingPolicies = requestService.MatchRequestToPolicies(lastRequest.Id);
            
            Console.WriteLine($"Знайдено {matchingPolicies.Count} відповідних активних полісів:");

            if (matchingPolicies.Count == 0)
            {
                Console.WriteLine("Не знайдено активних полісів, що відповідають критеріям запиту.");
            }
            else
            {
                foreach (var policy in matchingPolicies)
                {
                    Console.WriteLine($"- Id: {policy.Id} | Тип: {policy.PolicyType} | Покриття: {policy.CoverageAmount:0.00} ₴ | Ціна: {policy.Price:0.00} ₴");
                }
            }
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Помилка підбору: {ex.Message}");
        }
    }

    // --- Управління Платежами (Менеджер) ---
    private static async Task ManagePayments(IPaymentService paymentService)
    {
        Console.WriteLine("\n--- Управління Платежами ---");
        Console.WriteLine("1. Зафіксувати Внесок (Premium) - Можна в валюті!");
        Console.WriteLine("2. Зафіксувати Виплату (Payout) - Можна в валюті!");
        Console.WriteLine("3. Переглянути платежі за полісом");
        Console.Write("Ваш вибір: ");

        if (!int.TryParse(Console.ReadLine(), out int choice)) return;

        try
        {
            if (choice == 1 || choice == 2)
            {
                Console.Write("Введіть Id Поліса: ");
                if (!int.TryParse(Console.ReadLine(), out int policyId)) return;

                Console.Write("Введіть Суму платежу: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal amount)) return;

                Console.Write("Введіть Валюту (UAH, USD, EUR, тощо): ");
                string currency = Console.ReadLine()?.Trim().ToUpper() ?? "UAH";

                PaymentType type = choice == 1 ? PaymentType.Contribution : PaymentType.Payout;

                Console.WriteLine("Обробка платежу через API курсів валют...");
                
                var newPayment = await paymentService.RecordPaymentAsync(policyId, amount, type, currency);
                
                Console.WriteLine($"\nПлатіж успішно зафіксовано!");
                Console.WriteLine($"   Сума внесена: {amount} {currency}");
                Console.WriteLine($"   Зараховано в базу: {newPayment.Amount:0.00} ₴");
            }
            else if (choice == 3)
            {
                Console.Write("Введіть Id Поліса: ");
                if (!int.TryParse(Console.ReadLine(), out int policyId)) return;

                var payments = paymentService.GetPaymentsByPolicy(policyId);
                Console.WriteLine($"\n--- Історія платежів (Поліс #{policyId}) ---");
                if (payments.Count == 0)
                {
                    Console.WriteLine("Платежів не знайдено.");
                    return;
                }

                foreach (var p in payments.OrderBy(p => p.Date))
                {
                    string sign = p.Type == PaymentType.Contribution ? "+" : "-";
                    Console.WriteLine($"[Id:{p.Id}] {p.Date:dd.MM.yyyy} | {p.Type} | {sign}{p.Amount:0.00} ₴");
                }
            }
        }
        catch (HttpRequestException httpEx)
        {
            Console.WriteLine($"Помилка підключення до API: {httpEx.Message}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Помилка валідації: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
        }
    }
    
    // --- Пошук Полісів (Менеджер) ---
    private static void SearchPolicies(IPolicyService policyService)
    {
        Console.WriteLine("\n--- Пошук Полісів ---");
        
        PolicyTypes? type = null;
        StatusTypes? status = null;
        int? clientId = null;
        decimal? minPrice = null;
        decimal? maxPrice = null;

        Console.WriteLine("Фільтр за типом (0/1/2 або ENTER):");
        string typeInput = Console.ReadLine();
        if (int.TryParse(typeInput, out int typeChoice) && Enum.IsDefined(typeof(PolicyTypes), typeChoice))
        {
            type = (PolicyTypes)typeChoice;
        }
        
        Console.WriteLine("Фільтр за Id Клієнта (або ENTER):");
        string clientInput = Console.ReadLine();
        if (int.TryParse(clientInput, out int clientValue))
        {
            clientId = clientValue;
        }

        Console.WriteLine("Фільтр за статусом (0-3 або ENTER):");
        string statusInput = Console.ReadLine();
        if (int.TryParse(statusInput, out int statusChoice) && Enum.IsDefined(typeof(StatusTypes), statusChoice))
        {
            status = (StatusTypes)statusChoice;
        }

        Console.WriteLine("Фільтр за мінімальною вартістю (або ENTER):");
        string minPriceInput = Console.ReadLine();
        if (decimal.TryParse(minPriceInput, out decimal minPriceValue))
        {
            minPrice = minPriceValue;
        }

        Console.WriteLine("Фільтр за максимальною вартістю (або ENTER):");
        string maxPriceInput = Console.ReadLine();
        if (decimal.TryParse(maxPriceInput, out decimal maxPriceValue))
        {
            maxPrice = maxPriceValue;
        }

        var foundPolicies = policyService.SearchPolicies(type, clientId, status, minPrice, maxPrice);
        ShowPolicies(foundPolicies, showClaims: false);
    }

    // --- Аналітика (Менеджер) ---
    private static void ShowAnalytics(IAnalyticsService service)
    {
        Console.WriteLine("\n=== ПОВНА АНАЛІТИКА КОМПАНІЇ ===");
        
        Console.WriteLine($"Активних полісів: {service.GetActivePolicyCount()}");
        Console.WriteLine($"Загальні виплати: {service.GetTotalPayouts():0.00} грн");
        Console.WriteLine($"Чистий дохід:     {service.GetCompanyRevenue():0.00} грн");
        
        var start = DateTime.Now.AddDays(-365);
        var end = DateTime.Now;
        int recentPolicies = service.GetClaimsByPeriod(start, end);
        Console.WriteLine($"Страхових подій (останній рік): {recentPolicies}");
        
        Console.WriteLine("\n--- Статистика за типами страхування ---");
        var typeStats = service.GetPolicyStatsByType();
        if (typeStats.Count == 0) Console.WriteLine("Даних немає.");
        foreach (var stat in typeStats)
        {
            Console.WriteLine($"- {stat.Key}: {stat.Value} шт.");
        }
        
        Console.WriteLine("\n--- Ефективність Агентів ---");
        var agentStats = service.GetAgentPerfomanceStats();
        if (agentStats.Count == 0) Console.WriteLine("Інформація відсутня.");
        foreach (var line in agentStats)
        {
            Console.WriteLine(line);
        }
        Console.WriteLine("===========================");
    }
    
    // --- Статистика Агента (Агент) ---
    private static void ShowAgentStats(IAnalyticsService service, int agentId)
    {
        Console.WriteLine($"\n=== АНАЛІТИКА АГЕНТА #{agentId} ===");
        
        // Отримуємо об'єкт агента для виведення його імені
        var agent = AgentRepository.GetById(agentId); 
        
        var agentStats = service.GetAgentPerfomanceStats();
        
        // Шукаємо рядок статистики, що містить ім'я поточного агента
        var myStat = agentStats.FirstOrDefault(s => agent != null && s.Contains($"Агент: {agent.Name}"));
        
        if (myStat == null)
        {
            Console.WriteLine("Даних про продажі агента не знайдено.");
        }
        else
        {
            Console.WriteLine(myStat);
        }
        Console.WriteLine("===========================");
    }
}