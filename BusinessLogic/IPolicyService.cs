using Domain;
namespace BusinessLogic;

public interface IPolicyService
{
    public List<Policy> GetAllPolicies();
    public Policy CreatePolicy(int clientId, PolicyTypes type, 
        DateTime startDate, DateTime endDate, decimal coverageAmount);
}