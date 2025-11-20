using Domain;

namespace BusinessLogic.Interfaces;

public interface IRequestService
{
    Request CreateRequest(int clientId, PolicyTypes type, decimal desiredCoverage, int duration);
    List<Request> GetClientRequests(int clientId);
    // !!! НОВЕ
    List<Policy> MatchRequestToPolicies(int requestId); 
}