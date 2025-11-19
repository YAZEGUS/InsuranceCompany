using Domain;
using System;
using System.Collections.Generic;

namespace BusinessLogic;

public interface IPaymentService
{
    /// <summary>
    /// Creates a record of a new payment (Contribution or Payout).
    /// </summary>
    Payment RecordPayment(int policyId, decimal amount, PaymentType type);

    /// <summary>
    /// Gets a list of all payments associated with a specific policy.
    /// </summary>
    List<Payment> GetPaymentsByPolicy(int policyId);
}