namespace Domain;

/// <summary>
/// Представляє клієнта страхової компанії (фізичну або юридичну особу).
/// </summary>
public class Client : BaseEntity
{
    /// <summary>
    /// Повне ім'я фізичної особи або назва компанії.
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    /// Контактна електронна адреса клієнта.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Тип клієнта (Фізична особа або Компанія).
    /// </summary>
    public ClientTypes ClientType { get; set; }
    
    /// <summary>
    /// Загальна кількість полісів, які наразі має клієнт.
    /// </summary>
    public int PolicyCount { get; set; } = 0;

    /// <summary>
    /// Сукупна сума всіх виплат, отриманих клієнтом.
    /// </summary>
    public decimal TotalPayouts { get; set; } = 0m;
}