using Domain;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic;

public class RequestService : IRequestService
{
    private readonly IRepository<Request> _requestRepository;
    private readonly IRepository<Policy> _policyRepository; // !!! НОВЕ: Потрібне для підбору полісів

    // !!! ОНОВЛЕНО: Додано IRepository<Policy> у конструктор
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
    
    // !!! НОВИЙ МЕТОД ЕТАПУ 4: Логіка підбору полісів під запит клієнта
    /// <summary>
    /// Finds existing active policies that approximately match the client's request based on type and coverage amount.
    /// </summary>
    /// <param name="requestId">The ID of the client request.</param>
    /// <returns>A list of matching policies.</returns>
    public List<Policy> MatchRequestToPolicies(int requestId)
    {
        var request = _requestRepository.GetById(requestId);
        
        if (request == null)
        {
            throw new ArgumentException($"Запит з Id={requestId} не знайдено.");
        }

        // Встановлюємо толерантність (наприклад, 10% різниці у сумі покриття)
        decimal tolerance = 0.10m; 
        decimal minCoverage = request.DesiredCoverageAmount * (1 - tolerance);
        decimal maxCoverage = request.DesiredCoverageAmount * (1 + tolerance);

        // Фільтрація: 
        // 1. Збіг за типом поліса
        // 2. Покриття існуючого поліса має бути у діапазоні бажаного (з толерантністю)
        // 3. Статус поліса має бути Активний (StatusTypes.Active)
        return _policyRepository.GetAll()
            .Where(p => 
                p.PolicyType == request.PolicyType && 
                p.CoverageAmount >= minCoverage &&    
                p.CoverageAmount <= maxCoverage &&
                p.Status == StatusTypes.Active)       
            .ToList();
    }
}