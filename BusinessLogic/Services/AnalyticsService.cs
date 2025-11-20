using BusinessLogic.Interfaces;
using Domain;
using Persistence;

namespace BusinessLogic.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly IRepository<Policy> _policyRepository;
    private readonly IRepository<Claim> _claimRepository;
    private readonly IRepository<Agent> _agentRepository;

    public AnalyticsService(IRepository<Policy> policyRepository, IRepository<Claim> claimRepository, IRepository<Agent> agentRepository)
    {
        _policyRepository = policyRepository;
        _claimRepository = claimRepository;
        _agentRepository = agentRepository;
    }

    public int GetActivePolicyCount()
    {
        return _policyRepository.GetAll().Count(p => p.Status == StatusTypes.Active);
    }

    public int GetClaimsByPeriod(DateTime startDate, DateTime endDate)
    {
        return _claimRepository.GetAll()
            .Count(c => c.Date >= startDate && c.Date <= endDate);
    }

    public decimal GetTotalPayouts()
    {
        return _claimRepository.GetAll().Sum(c => c.PayoutAmount);
    }

    public decimal GetCompanyRevenue()
    {
        decimal income = _policyRepository.GetAll().Sum(p => p.Price);
        decimal expenses = GetTotalPayouts();
        return income - expenses;
    }

    public Dictionary<PolicyTypes, int> GetPolicyStatsByType()
    {
        return _policyRepository.GetAll().
            GroupBy(p => p.PolicyType).
            ToDictionary(g => g.Key, g => g.Count());
    }

    public List<string> GetAgentPerfomanceStats()
    {
        var stats = new List<string>();
        var agents = _agentRepository.GetAll();
        var policies = _policyRepository.GetAll();
        foreach (var agent in agents)
        {
            var agentPolicies = policies.Where(p => p.AgentId == agent.Id).ToList();
            int salesCount = agentPolicies.Count;
            decimal totalSales = agentPolicies.Sum(p => p.Price);
            decimal earnedCommission = totalSales * agent.CommissionPercentage;
            stats.Add($"Агент: {agent.Name} | Продано: {salesCount} | Комісія: {earnedCommission:0.00}");
        }
        return stats;
    }
}