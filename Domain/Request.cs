using System;

namespace Domain;

/// <summary>
/// Представляє попередній запит від клієнта щодо бажаних умов страхування.
/// </summary>
public class Request : BaseEntity
{
    /// <summary>
    /// Ідентифікатор клієнта, який створив запит.
    /// </summary>
    public int ClientId { get; set; }
    
    /// <summary>
    /// Бажаний тип поліса.
    /// </summary>
    public PolicyTypes PolicyType { get; set; }
    
    /// <summary>
    /// Бажана сума покриття.
    /// </summary>
    public decimal DesiredCoverageAmount { get; set; }
    
    /// <summary>
    /// Дата створення запиту.
    /// </summary>
    public DateTime CreationDate { get; set; }
    
    /// <summary>
    /// Бажаний термін дії поліса у місяцях.
    /// </summary>
    public int DurationInMonths { get; set; }
}