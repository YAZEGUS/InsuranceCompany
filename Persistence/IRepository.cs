using Domain;
using System.Collections.Generic;

namespace Persistence;

/// <summary>
/// Defines the generic contract for a Repository.
/// This interface specifies standard CRUD (Create, Read, Update, Delete) operations
/// for any entity that inherits from BaseEntity.
/// </summary>
/// <typeparam name="T">The type of the entity, must inherit from BaseEntity.</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    void Add(T entity);

    /// <summary>
    /// Gets all entities from the repository.
    /// </summary>
    /// <returns>A list of all entities.</returns>
    List<T> GetAll();

    /// <summary>
    /// Gets an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The ID of the entity.</param>
    /// <returns>The found entity, or null if not found.</returns>
    T? GetById(int id);

    /// <summary>
    /// Updates an existing entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <returns>True if the entity was found and updated, false otherwise.</returns>
    bool Update(T entity);

    /// <summary>
    /// Deletes an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The ID of the entity to delete.</param>
    /// <returns>True if the entity was found and deleted, false otherwise.</returns>
    bool Delete(int id);
}