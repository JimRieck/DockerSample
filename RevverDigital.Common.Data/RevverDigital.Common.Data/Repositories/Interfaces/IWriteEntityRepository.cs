using RevverDigital.Common.Data.Models.Interfaces;

namespace RevverDigital.Common.Data.Repositories.Interfaces;

/// <summary>
/// Defines an object that creates, updates, and deletes <see cref="TEntity"/> entities.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to be stored.</typeparam>
/// <typeparam name="TId">The type of the identifiers for the given entity.</typeparam>
public interface IWriteEntityRepository<TEntity, TId>
    where TEntity : class, IEntity<TId>
{
    /// <summary>
    /// Inserts a new <see cref="TEntity"/> entity into the repository.
    /// </summary>
    /// <param name="entity">The entity to insert.</param>
    /// <returns>The same entity with updated values.</returns>
    Task<TEntity> InsertAsync(TEntity entity);
    
    /// <summary>
    /// Updates an existing <see cref="TEntity"/> entity.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <returns>The same entity with updated values.</returns>
    Task<TEntity> UpdateAsync(TEntity entity);

    /// <summary>
    /// Soft deletes an entity with the given id.
    /// </summary>
    /// <param name="id">The identifier used to find the entity to delete.</param>
    Task DeleteByIdAsync(TId id);

    /// <summary>
    /// Hard deletes an entity with the given id.
    /// </summary>
    /// <param name="id">The identifier used to find the entity to delete.</param>
    Task HardDeleteByIdAsync(TId id);
}

/// <summary>
/// Defines an object that creates, updates, and deletes <see cref="TEntity"/> entities. The identifier for the entity will be of type <see cref="Guid"/>.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to be stored.</typeparam>
public interface IWriteEntityRepository<TEntity> : IWriteEntityRepository<TEntity, Guid>
    where TEntity : class, IEntity<Guid>
{
}
