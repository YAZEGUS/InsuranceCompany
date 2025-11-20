using BusinessLogic;
using Domain;
using Persistence;
using System;
using System.Linq; 
using System.Collections.Generic;
using BusinessLogic.Interfaces;
using BusinessLogic.Services;

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
    private static readonly IRequestService RequestService;
    private static readonly IAnalyticsService AnalyticsService;
    private static readonly IPaymentService PaymentService; // !!! ЕТАП 4: НОВИЙ СЕРВІС
    
    // !!! ЕТАП 5: Поля для API
    private static readonly HttpClient HttpClient;
    private static readonly ICurrencyRateService CurrencyRateService;

    /// <summary>
    /// Static constructor to set up repositories and services (Dependency Injection).
    /// </summary>
    static Program()
    {
        var dbContext = new AppDbContext();
        HttpClient = new HttpClient(); 
        
        // Ініціалізація Repositories (Persistence Layer)
        // Створюємо репозиторії як локальні змінні для DI
        IRepository<Client> clientRepository = new SqlRepository<Client>(dbContext);
        IRepository<Policy> policyRepository = new SqlRepository<Policy>(dbContext);
        IRepository<Claim> claimRepository = new SqlRepository<Claim>(dbContext);
        AgentRepository = new SqlRepository<Agent>(dbContext); 
        IRepository<Request> requestRepository = new SqlRepository<Request>(dbContext);
        IRepository<Payment> paymentRepository = new SqlRepository<Payment>(dbContext); // !!! ЕТАП 4: НОВИЙ РЕПОЗИТОРІЙ
        
        // Ініціалізація залежностей (Business Logic Layer)
        // Передаємо репозиторії до сервісів
        ClientService = new ClientService(clientRepository, policyRepository);
        PolicyService = new PolicyService(policyRepository, ClientService, AgentRepository);
        ClaimService = new ClaimService(claimRepository, policyRepository, ClientService);
        CurrencyRateService = new CurrencyRateService(HttpClient);
        
        // !!! ЕТАП 4: ОНОВЛЕННЯ RequestService (потребує PolicyRepository для підбору)
        RequestService = new RequestService(requestRepository, policyRepository); 

        AnalyticsService = new AnalyticsService(policyRepository, claimRepository, AgentRepository);
        
        // !!! ЕТАП 4: ІНІЦІАЛІЗАЦІЯ PaymentService
        PaymentService = new PaymentService(paymentRepository, policyRepository, CurrencyRateService);
    }

    /// <summary>
    /// The main entry point for the application.
    /// Runs the main menu loop.
    /// </summary>
    static async Task Main()
    {
        Console.WriteLine("--- Система Управління Страхуванням ---");
        
        while (true)
        {
            Console.WriteLine("\n--- ГОЛОВНЕ МЕНЮ ---");
            Console.WriteLine("Натисніть 1 для управління клієнтами ");
            Console.WriteLine("Натисніть 2 для управління полісами ");
            Console.WriteLine("Натисніть 3 для управління подіями ");
            Console.WriteLine("Натисніть 4 для пошуку полісів ");
            Console.WriteLine("Натисніть 5 для управління агентами ");
            Console.WriteLine("Натисніть 6 для запитів клієнтів");
            Console.WriteLine("Натисніть 7 для статистики та аналітики");
            Console.WriteLine("Натисніть 8 для управління платежами (Етап 4)"); // !!! НОВИЙ ПУНКТ
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
                        await ManagePolicies(PolicyService);
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
                    case 6:
                        ManageRequests(RequestService);
                        break;
                    case 7:
                        ShowAnalytics(AnalyticsService);
                        break;
                    case 8: // !!! НОВИЙ CASE
                        await ManagePayments(PaymentService);
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

                // !!! ЕТАП 5: Запитуємо валюту
                Console.Write("Введіть Валюту (UAH, USD, EUR): ");
                string currency = Console.ReadLine()?.Trim().ToUpper();
                if (string.IsNullOrEmpty(currency)) currency = "UAH";

                PaymentType type = choice == 1 ? PaymentType.Contribution : PaymentType.Payout;

                Console.WriteLine("Обробка платежу через API курсів валют...");
                
                // Викликаємо асинхронний метод сервісу
                var newPayment = await paymentService.RecordPaymentAsync(policyId, amount, type, currency);
                
                Console.WriteLine($"\n✅ Платіж успішно зафіксовано!");
                Console.WriteLine($"   Сума внесена: {amount} {currency}");
                Console.WriteLine($"   Зараховано в базу (UAH): {newPayment.Amount:0.00} ₴");
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
            Console.WriteLine($"⛔ Помилка підключення до API: {httpEx.Message}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"⚠️ Помилка валідації: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Помилка: {ex.Message}");
        }
    }

    // !!! ЕТАП 4: ОНОВЛЕНИЙ МЕТОД ДЛЯ УПРАВЛІННЯ ЗАПИТАМИ
    private static void ManageRequests(IRequestService requestService)
    {
        {
            Console.WriteLine("\n--- Запити Клієнтів ---");
            Console.WriteLine("1. Створити новий запит");
            Console.WriteLine("2. Переглянути запити клієнта");
            Console.WriteLine("3. Підібрати поліси за запитом"); // !!! НОВИЙ ПУНКТ
            Console.Write("Ваш вибір: ");

            if (!int.TryParse(Console.ReadLine(), out int choice)) return;

            if (choice == 1)
            {
                Console.WriteLine("Введіть ID Клієнта:");
                if (!int.TryParse(Console.ReadLine(), out int clientId)) return;

                Console.WriteLine("Тип поліса (0=CarInsurance, 1=MedicalInsurance, 2=PropertyInsurance):");
                int typeChoice = int.Parse(Console.ReadLine());
                PolicyTypes type = (PolicyTypes)(typeChoice);

                Console.WriteLine("Бажана сума покриття:");
                decimal amount = decimal.Parse(Console.ReadLine());
                
                Console.WriteLine("Бажаний строк дії в місяцях");
                int durationInMonths = int.Parse(Console.ReadLine());

                requestService.CreateRequest(clientId, type, amount, durationInMonths);
                Console.WriteLine("Запит успішно збережено!");
            }
            else if (choice == 2)
            {
                Console.WriteLine("Введіть ID Клієнта:");
                if (!int.TryParse(Console.ReadLine(), out int clientId)) return;

                var requests = requestService.GetClientRequests(clientId);
                if (requests.Count == 0)
                {
                    Console.WriteLine("У цього клієнта немає запитів.");
                    return;
                }
                foreach (var req in requests)
                {
                    Console.WriteLine(
                        $"[ID: {req.Id}] [Дата: {req.CreationDate:d}] Тип: {req.PolicyType} | Сума: {req.DesiredCoverageAmount:0.00} ₴ | Термін: {req.DurationInMonths} міс.");
                }
            }
            else if (choice == 3) // !!! ЛОГІКА ПІДБОРУ
            {
                Console.WriteLine("--- Підбір Полісів за Запитом ---");
                Console.Write("Введіть ID Запиту для підбору: ");
                if (!int.TryParse(Console.ReadLine(), out int requestId)) return;

                try
                {
                    var matchingPolicies = requestService.MatchRequestToPolicies(requestId);
                    Console.WriteLine($"\nЗнайдено {matchingPolicies.Count} відповідних активних полісів:");

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
            else
            {
                 Console.WriteLine("Некоректний вибір.");
            }
        }
    }
    
    private static void ShowAnalytics(IAnalyticsService service)
    {
        Console.WriteLine("\n=== АНАЛІТИКА КОМПАНІЇ ===");
        
        Console.WriteLine($"Активних полісів: {service.GetActivePolicyCount()}");
        Console.WriteLine($"Загальні виплати: {service.GetTotalPayouts():0.00} грн");
        Console.WriteLine($"Чистий дохід:     {service.GetCompanyRevenue():0.00} грн");
        
        var start = DateTime.Now.AddDays(-30);
        var end = DateTime.Now;
        int recentPolicies = service.GetClaimsByPeriod(start, end);
        Console.WriteLine($"Страхових подій (останні 30 днів): {recentPolicies}");
        
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
                Console.WriteLine("--- Створення поліса ---");
                Console.Write("Id Клієнта: "); int clientId = int.Parse(Console.ReadLine());
                Console.Write("Id Агента (Enter якщо немає): "); 
                string agentIn = Console.ReadLine();
                int? agentId = string.IsNullOrEmpty(agentIn) ? null : int.Parse(agentIn);

                Console.Write("Тип (0=Car, 1=Med, 2=Prop): "); PolicyTypes type = (PolicyTypes)int.Parse(Console.ReadLine());
                Console.Write("Дата початку (yyyy-mm-dd): "); DateTime start = DateTime.Parse(Console.ReadLine());
                Console.Write("Дата кінця (yyyy-mm-dd): "); DateTime end = DateTime.Parse(Console.ReadLine());
                Console.Write("Сума покриття (UAH): "); decimal coverage = decimal.Parse(Console.ReadLine());

                try
                {
                    var newPolicy = policyService.CreatePolicy(clientId, agentId, type, start, end, coverage);
                    Console.WriteLine($"✅ Поліс створено! Id: {newPolicy.Id}, Ціна: {newPolicy.Price:0.00} UAH");
                    
                    // !!! API BONUS: Показуємо ціну в доларах для інформації
                    try 
                    {
                        decimal rate = await CurrencyRateService.GetExchangeRateAsync("UAH", "USD");
                        decimal priceUsd = newPolicy.Price * rate;
                        Console.WriteLine($"ℹ️  Орієнтовна ціна в USD: ${priceUsd:0.00} (Курс: {rate})");
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
                break;
                
            case 2:
                var policies = policyService.GetAllPolicies();
                foreach (var p in policies)
                    Console.WriteLine($"Id: {p.Id}, Type: {p.PolicyType}, Price: {p.Price:0.00}, Status: {p.Status}");
                break;
            
            case 3:
                Console.Write("Id Поліса: "); int pId = int.Parse(Console.ReadLine());
                Console.Write("Статус (0-3): "); StatusTypes st = (StatusTypes)int.Parse(Console.ReadLine());
                try { policyService.ChangePolicyStatus(pId, st); Console.WriteLine("Статус змінено."); }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
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

        Console.WriteLine("Фільтр за типом поліса (0=CarInsurance, 1=MedicalInsurance, 2=PropertyInsurance, або ENTER для будь-якого):");
        string typeInput = Console.ReadLine();
        if (int.TryParse(typeInput, out int typeChoice) && Enum.IsDefined(typeof(PolicyTypes), typeChoice))
        {
            type = (PolicyTypes)typeChoice;
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