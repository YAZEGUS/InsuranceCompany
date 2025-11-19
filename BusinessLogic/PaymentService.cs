using Domain;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic;

public class PaymentService : IPaymentService
{
    private readonly IRepository<Payment> _paymentRepository;
    private readonly IRepository<Policy> _policyRepository; // Для перевірки існування поліса

    public PaymentService(IRepository<Payment> paymentRepository, IRepository<Policy> policyRepository)
    {
        _paymentRepository = paymentRepository;
        _policyRepository = policyRepository;
    }

    public Payment RecordPayment(int policyId, decimal amount, PaymentType type)
    {
        var policy = _policyRepository.GetById(policyId);
        
        if (policy == null)
        {
            throw new ArgumentException($"Policy with Id={policyId} not found.");
        }
        
        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be positive.");
        }

        var newPayment = new Payment
        {
            PolicyId = policyId,
            Date = DateTime.Now,
            Amount = amount,
            Type = type
        };

        _paymentRepository.Add(newPayment);
        return newPayment;
    }

    public List<Payment> GetPaymentsByPolicy(int policyId)
    {
        return _paymentRepository.GetAll()
            .Where(p => p.PolicyId == policyId)
            .ToList();
    }
}