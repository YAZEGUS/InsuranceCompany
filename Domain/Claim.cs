using System;

namespace Domain;

/// <summary>
/// Представляє страхову подію, подану до поліса.
/// </summary>
public class Claim : BaseEntity
{
    /// <summary>
    /// Зовнішній ключ, що посилається на Поліс, з яким пов'язана ця подія.
    /// </summary>
    public int PolicyId { get; set; }

    /// <summary>
    /// Дата подання події.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Опис страхової події.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Сума, що підлягає виплаті за цією подією.
    /// </summary>
    public decimal PayoutAmount { get; set; }
    
    /// <summary>
    /// Поточний статус події.
    /// </summary>
    public ClaimStatusTypes Status { get; set; } = ClaimStatusTypes.New; // Статус за замовчуванням
}