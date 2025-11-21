namespace Domain;

/// <summary>
/// Абстрактний базовий клас для всіх сутностей домену.
/// Гарантує, що кожна сутність має унікальний ідентифікатор.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Унікальний ідентифікатор для сутності.
    /// </summary>
    public int Id { get; set; }
}