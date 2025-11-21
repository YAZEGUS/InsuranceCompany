using System;

namespace Domain;

/// <summary>
/// Визначає структуру страхового поліса, розширюючи BaseEntity.
/// </summary>
public class Policy : BaseEntity
{
    /// <summary>
    /// Зовнішній ключ, що посилається на клієнта, який володіє цим полісом.
    /// </summary>
    public int ClientId { get; set; }

    /// <summary>
    /// Ідентифікатор агента, який продав поліс. Може бути null.
    /// </summary>
    public int? AgentId { get; set; } 

    /// <summary>
    /// Тип страхового поліса.
    /// </summary>
    public PolicyTypes PolicyType { get; set; }

    /// <summary>
    /// Дата початку дії поліса.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Дата завершення дії поліса.
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Максимальна сума покриття, яку надає страховка.
    /// </summary>
    public decimal CoverageAmount { get; set; }

    /// <summary>
    /// Вартість (премія) поліса, розрахована Бізнес-логікою.
    /// </summary>
    public decimal Price { get; set; }
    
    /// <summary>
    /// Валюта поліса (наприклад, UAH, USD).
    /// </summary>
    public string Currency { get; set; } = "UAH";

    /// <summary>
    /// Поточний статус поліса.
    /// </summary>
    public StatusTypes Status { get; set; } = StatusTypes.Active;
}