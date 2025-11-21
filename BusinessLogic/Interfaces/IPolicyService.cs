using Domain;

namespace BusinessLogic.Interfaces;

/// <summary>
/// Контракт для Сервісу полісів. Визначає бізнес-операції, пов'язані зі страховими полісами.
/// </summary>
public interface IPolicyService
{
    /// <summary>
    /// Отримує список усіх полісів.
    /// </summary>
    List<Policy> GetAllPolicies();

    /// <summary>
    /// Створює новий страховий поліс та розраховує його вартість.
    /// </summary>
    Policy CreatePolicy(int clientId, int? agentId, PolicyTypes type, 
        DateTime startDate, DateTime endDate, decimal coverageAmount);

    /// <summary>
    /// Змінює статус існуючого поліса.
    /// </summary>
    /// <param name="policyId">Ідентифікатор поліса.</param>
    /// <param name="newStatus">Новий статус.</param>
    /// <returns>True, якщо оновлено.</returns>
    bool ChangePolicyStatus(int policyId, StatusTypes newStatus);

    /// <summary>
    /// Здійснює пошук полісів на основі заданих критеріїв.
    /// </summary>
    List<Policy> SearchPolicies(PolicyTypes? type = null, int? clientId = null, 
        StatusTypes? status = null, decimal? minPrice = null, decimal? maxPrice = null,
        int? agentId = null);
}