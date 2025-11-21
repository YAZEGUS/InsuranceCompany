using Domain;

namespace BusinessLogic.Interfaces;

/// <summary>
/// Контракт для Сервісу Запитів. Визначає операції для керування попередніми запитами клієнтів.
/// </summary>
public interface IRequestService
{
    /// <summary>
    /// Створює новий запит клієнта на поліс.
    /// </summary>
    Request CreateRequest(int clientId, PolicyTypes type, decimal desiredCoverage, int duration);
    
    /// <summary>
    /// Отримує список усіх запитів, створених певним клієнтом.
    /// </summary>
    List<Request> GetClientRequests(int clientId);
    
    /// <summary>
    /// Здійснює підбір активних полісів, що відповідають критеріям запиту.
    /// </summary>
    /// <param name="requestId">Ідентифікатор запиту.</param>
    /// <returns>Список відповідних об'єктів Policy.</returns>
    List<Policy> MatchRequestToPolicies(int requestId); 
}