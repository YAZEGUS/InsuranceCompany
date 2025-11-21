using Domain;

namespace BusinessLogic.Interfaces;

/// <summary>
/// Контракт для Сервісу претензій (страхових подій).
/// Визначає бізнес-операції, пов'язані з врегулюванням збитків.
/// </summary>
public interface IClaimService
{
    /// <summary>
    /// Отримує список усіх претензій.
    /// </summary>
    List<Claim> GetAll();

    /// <summary>
    /// Створює нову претензію (страхову подію).
    /// </summary>
    /// <param name="policyId">Ідентифікатор поліса.</param>
    /// <param name="date">Дата подання.</param>
    /// <param name="description">Опис події.</param>
    /// <param name="payoutAmount">Запитувана сума виплати.</param>
    /// <returns>Новостворена претензія.</returns>
    Claim CreateClaim(int policyId, DateTime date, string description, decimal payoutAmount);

    /// <summary>
    /// Змінює статус існуючої претензії.
    /// </summary>
    /// <param name="claimId">Ідентифікатор претензії.</param>
    /// <param name="newStatus">Новий статус.</param>
    /// <returns>True, якщо статус успішно оновлено.</returns>
    bool ChangeClaimStatus(int claimId, ClaimStatusTypes newStatus);
}