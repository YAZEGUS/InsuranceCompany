using Domain;

namespace BusinessLogic.Interfaces;

public interface IPaymentService
{
    /// <summary>
    /// Creates a record of a new payment (Contribution or Payout).
    /// </summary>
    Payment RecordPayment(int policyId, decimal amount, PaymentType type);

    /// <summary>
    /// Creates a record of a new payment, converting the amount if currencies differ.
    /// </summary>
    Task<Payment> RecordPaymentAsync(int policyId, 
        decimal amount, 
        PaymentType type, 
        string paymentCurrency);
    
    /// <summary>
    /// Gets a list of all payments associated with a specific policy.
    /// </summary>
    List<Payment> GetPaymentsByPolicy(int policyId);
}