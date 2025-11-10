using Domain;
using System.Collections.Generic;

namespace Persistence;

/// <summary>
/// Generic repository interface (Contract) for data access.
/// Defines the standard CRUD (Create, Read, Update, Delete) operations
/// for any entity that inherits from BaseEntity.
/// </summary>
/// <typeparam name="T">The entity type, must inherit from BaseEntity.</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Gets all entities of type T.
    /// </summary>
    /// <returns>A list of all entities.</returns>
    List<T> GetAll();

    /// <summary>
    /// Gets a single entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <returns>The entity, or null if not found.</returns>
    T GetById(int id);

    /// <summary>
    /// Adds a new entity to the repository.
    /// The entity's Id will be set by the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    void Add(T entity);

    /// <summary>
    /// Updates an existing entity in the repository.
    /// </summary>
    /// <param name="entity">The entity with updated values.</param>
    void Update(T entity);

    /// <summary>
    /// Deletes an entity from the repository by its unique identifier.
    /// </summary>
    /// <param name="id">The Id of the entity to delete.</param>
    void Delete(int id);
}