using RevverDigital.Common.Data.Models.Interfaces;

namespace RevverDigital.Common.Data.Repositories.Interfaces;

/// <summary>
/// Defines an an object that creates, reads, updates, and deletes <see cref="TEntity"/> entities.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to be loaded or stored.</typeparam>
/// <typeparam name="TId">The type of the identifiers for the given entity.</typeparam>
public interface IEntityRepository<TEntity, TId> : IReadEntityRepository<TEntity, TId>, IWriteEntityRepository<TEntity, TId>
    where TEntity : class, IEntity<TId>
{
}

/// <summary>
/// Defines an an object that creates, reads, updates, and deletes <see cref="TEntity"/> entities. The identifier for the entity will be of type <see cref="Guid"/>.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to be loaded or stored.</typeparam>
/// <typeparam name="TId">The type of the identifiers for the given entity.</typeparam>
public interface IEntityRepository<TEntity> : IEntityRepository<TEntity, Guid>
    where TEntity : class, IEntity<Guid>
{
}
