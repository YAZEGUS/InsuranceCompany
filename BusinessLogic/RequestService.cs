using Domain;
using Persistence;

namespace BusinessLogic;

public class RequestService : IRequestService
{
    private readonly IRepository<Request> _requestRepository;
    
    public RequestService(IRepository<Request> requestRepository)
    {
        _requestRepository = requestRepository;
    }   
    
    public Request CreateRequest(int clientId, PolicyTypes type, decimal desiredCoverage, int duration)
    {
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
}