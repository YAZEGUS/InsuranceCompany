using Domain;

namespace BusinessLogic.Interfaces;

public interface IAnalyticsService
{
    int GetActivePolicyCount();
    int GetClaimsByPeriod(DateTime startDate, DateTime endDate);
    decimal GetTotalPayouts();
    decimal GetCompanyRevenue();
    Dictionary<PolicyTypes, int> GetPolicyStatsByType();
    List<string> GetAgentPerfomanceStats();
}