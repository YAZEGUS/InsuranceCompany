using Domain;
using Persistence;
namespace BusinessLogic;

public class PolicyService : IPolicyService
{
    private readonly IRepository<Policy> _policyRepository;
    private readonly IRepository<Client> _clientRepository;
    
    public PolicyService(IRepository<Policy> policyRepository, IRepository<Client> clientRepository)
    {
        _policyRepository = policyRepository;
        _clientRepository = clientRepository;
    }
    
    public List<Policy> GetAllPolicies()
    {
        return _policyRepository.GetAll();
    }

    public Policy CreatePolicy(int clientId, PolicyTypes type, DateTime startDate, DateTime endDate, decimal coverageAmount)
    {
        decimal price = CalculatePolicyPrice(coverageAmount, type);
        
        var newPolicy = new Policy
        {
            ClientId = clientId,
            PolicyType = type,
            StartDate = startDate,
            EndDate = endDate,
            CoverageAmount = coverageAmount,
            Price = price,
            Status = StatusTypes.Active
        };
        _policyRepository.Add(newPolicy);
        return newPolicy;
    }

    private decimal CalculatePolicyPrice(decimal coverageAmount, PolicyTypes policyType)
    {
        switch (policyType)
        {
            case PolicyTypes.CarInsurance:
                return coverageAmount * 0.05m;
            case PolicyTypes.MedicalInsurance:
                return coverageAmount * 0.07m;
            case PolicyTypes.PropertyInsurance:
                return coverageAmount * 0.10m;
            default:
                return 0;
        }
    }
}