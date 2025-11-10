namespace Domain;

/// <summary>
/// Abstract base class for all domain entities.
/// Ensures that every entity has a unique identifier.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// The unique identifier for the entity.
    /// </summary>
    public int Id { get; set; }
}