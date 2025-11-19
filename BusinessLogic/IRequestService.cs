using Domain;

namespace BusinessLogic;

public interface IRequestService
{
    Request CreateRequest(int clientId, PolicyTypes type, decimal desiredCoverage, int duration);
    List<Request> GetClientRequests(int clientId);
}