using Domain;

namespace BusinessLogic.Interfaces;

/// <summary>
/// Контракт для Сервісу аналітики. Визначає операції для формування звітів та статистики.
/// </summary>
public interface IAnalyticsService
{
    /// <summary>
    /// Отримує загальну кількість активних полісів.
    /// </summary>
    int GetActivePolicyCount();
    
    /// <summary>
    /// Отримує кількість страхових подій за вказаний період.
    /// </summary>
    int GetClaimsByPeriod(DateTime startDate, DateTime endDate);
    
    /// <summary>
    /// Отримує загальну суму всіх виплат, здійснених компанією.
    /// </summary>
    decimal GetTotalPayouts();
    
    /// <summary>
    /// Розраховує чистий дохід компанії (Внески мінус Виплати).
    /// </summary>
    decimal GetCompanyRevenue();
    
    /// <summary>
    /// Отримує статистику кількості полісів за типом страхування.
    /// </summary>
    Dictionary<PolicyTypes, int> GetPolicyStatsByType();
    
    /// <summary>
    /// Формує статистику ефективності агентів (продажі, комісія).
    /// </summary>
    List<string> GetAgentPerfomanceStats();
}