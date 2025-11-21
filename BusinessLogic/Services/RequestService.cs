using BusinessLogic.Interfaces;
using Domain;
using Persistence;

namespace BusinessLogic.Services;

/// <summary>
/// Реалізує логіку управління запитами клієнтів та підбору полісів.
/// </summary>
public class RequestService : IRequestService
{
    private readonly IRepository<Request> _requestRepository;
    private readonly IRepository<Policy> _policyRepository; 

    public RequestService(IRepository<Request> requestRepository, IRepository<Policy> policyRepository)
    {
        _requestRepository = requestRepository;
        _policyRepository = policyRepository;
    }   
    
    public Request CreateRequest(int clientId, PolicyTypes type, decimal desiredCoverage, int duration)
    {
        // Базова перевірка вхідних даних
        if (desiredCoverage <= 0 || duration <= 0)
        {
            throw new ArgumentException("Сума покриття та тривалість мають бути позитивними.");
        }
        
        var request = new Request
        {
            ClientId = clientId,
            PolicyType = type,
            DesiredCoverageAmount = desiredCoverage,
            CreationDate = DateTime.Now,
            DurationInMonths = duration
        };
        _requestRepository.Add(request);
        return request;
    }

    public List<Request> GetClientRequests(int clientId)
    {
        return _requestRepository.GetAll().Where(r => r.ClientId == clientId).ToList();
    }
    
    /// <summary>
    /// Знаходить існуючі активні поліси, які приблизно відповідають запиту клієнта (з толерантністю 10%).
    /// </summary>
    /// <param name="requestId">Ідентифікатор запиту клієнта.</param>
    /// <returns>Список відповідних полісів.</returns>
    public List<Policy> MatchRequestToPolicies(int requestId)
    {
        var request = _requestRepository.GetById(requestId);
        
        if (request == null)
        {
            // Помилка вже українською
            throw new ArgumentException($"Запит з Id={requestId} не знайдено.");
        }

        // Встановлюємо толерантність (наприклад, 10% різниці у сумі покриття)
        decimal tolerance = 0.10m; 
        decimal minCoverage = request.DesiredCoverageAmount * (1 - tolerance);
        decimal maxCoverage = request.DesiredCoverageAmount * (1 + tolerance);

        // Фільтрація:
        return _policyRepository.GetAll()
            .Where(p => 
                p.PolicyType == request.PolicyType && 
                p.CoverageAmount >= minCoverage &&    
                p.CoverageAmount <= maxCoverage &&
                p.Status == StatusTypes.Active)       
            .ToList();
    }
}