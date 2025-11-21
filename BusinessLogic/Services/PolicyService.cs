using BusinessLogic.Interfaces;
using Domain;
using Persistence;

namespace BusinessLogic.Services;

/// <summary>
/// Реалізує інтерфейс IPolicyService. Містить бізнес-логіку для управління полісами.
/// </summary>
public class PolicyService : IPolicyService
{
    private readonly IRepository<Policy> _policyRepository;
    private readonly IClientService _clientService; 
    private readonly IRepository<Agent> _agentRepository; 
    
    public PolicyService(IRepository<Policy> policyRepository, IClientService clientService, IRepository<Agent> agentRepository)
    {
        _policyRepository = policyRepository;
        _clientService = clientService;
        _agentRepository = agentRepository; 
    }
    
    public List<Policy> GetAllPolicies()
    {
        return _policyRepository.GetAll();
    }
    
    public Policy CreatePolicy(int clientId, int? agentId, PolicyTypes type, DateTime startDate, DateTime endDate, decimal coverageAmount)
    {
        // 1. Перевірка існування клієнта
        var client = _clientService.GetClientById(clientId);
        if (client == null)
        {
            throw new ArgumentException($"Клієнт з Id={clientId} не знайдено");
        }

        // 2. Валідація дат (Бізнес-правило, додане раніше)
        if (startDate >= endDate)
        {
            throw new ArgumentException("Дата початку полісу має бути строго раніше кінцевої дати.");
        }
        
        // !!! НОВЕ: Валідація Агента
        if (agentId.HasValue)
        {
            var agent = _agentRepository.GetById(agentId.Value);
            if (agent == null)
            {
                throw new ArgumentException($"Агента з Id={agentId.Value} не знайдено.");
            }
        }
        
        // 3. Валідація суми покриття (додано для надійності)
         if (coverageAmount <= 0)
        {
            throw new ArgumentException("Сума покриття має бути позитивним числом.");
        }


        // Бізнес-логіка: Розрахунок вартості
        decimal price = CalculatePolicyPrice(coverageAmount, type);
        
        var newPolicy = new Policy
        {
            ClientId = clientId,
            AgentId = agentId,
            PolicyType = type,
            StartDate = startDate,
            EndDate = endDate,
            CoverageAmount = coverageAmount,
            Price = price,
            Status = StatusTypes.Active 
        };
        
        _policyRepository.Add(newPolicy);
        
        // Оновлюємо статистику клієнта
        _clientService.UpdateClientStats(clientId, policyChange: 1);

        return newPolicy;
    }

    private decimal CalculatePolicyPrice(decimal coverageAmount, PolicyTypes policyType)
    {
        switch (policyType)
        {
            case PolicyTypes.CarInsurance:
                return coverageAmount * 0.05m;
            case PolicyTypes.MedicalInsurance:
                return coverageAmount * 0.07m;
            case PolicyTypes.PropertyInsurance:
                return coverageAmount * 0.10m;
            default:
                return 0; 
        }
    }

    public bool ChangePolicyStatus(int policyId, StatusTypes newStatus)
    {
        var policy = _policyRepository.GetById(policyId);
        if (policy == null)
        {
            throw new ArgumentException($"Поліс з Id={policyId} не знайдено.");
        }

        // ЛОГІКА POLICY COUNT (виправлено раніше)
        bool wasActive = policy.Status == StatusTypes.Active;
        bool willBeInactive = newStatus == StatusTypes.Completed || newStatus == StatusTypes.Cancelled;
        
        if (wasActive && willBeInactive)
        {
            _clientService.UpdateClientStats(policy.ClientId, policyChange: -1);
        }
     
        policy.Status = newStatus;
        _policyRepository.Update(policy);
        return true;
    }

    // !!! ВИПРАВЛЕНО: Додано параметр agentId
    public List<Policy> SearchPolicies(PolicyTypes? type = null, int? clientId = null, 
        StatusTypes? status = null, decimal? minPrice = null, decimal? maxPrice = null, int? agentId = null)
    {
        IEnumerable<Policy> query = _policyRepository.GetAll();
        
        if (type.HasValue)
        {
            query = query.Where(p => p.PolicyType == type.Value);
        }

        if (clientId.HasValue)
        {
            query = query.Where(p => p.ClientId == clientId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }
        
        // !!! НОВЕ: Фільтрація за AgentId
        if (agentId.HasValue)
        {
            query = query.Where(p => p.AgentId == agentId.Value);
        }

        return query.ToList();
    }
}