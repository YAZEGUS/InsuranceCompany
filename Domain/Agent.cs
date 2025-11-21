namespace Domain;

/// <summary>
/// Представляє страхового агента, який продає поліси.
/// </summary>
public class Agent : BaseEntity
{
    /// <summary>
    /// Ім'я агента.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Відсоток комісії, який агент отримує від проданих полісів.
    /// </summary>
    public decimal CommissionPercentage { get; set; }
}