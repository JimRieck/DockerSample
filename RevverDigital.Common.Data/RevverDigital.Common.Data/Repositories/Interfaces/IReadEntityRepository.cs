using RevverDigital.Common.Data.Models.Interfaces;
using System.Linq.Expressions;

namespace RevverDigital.Common.Data.Repositories.Interfaces;

/// <summary>
/// Defines an object that reads <see cref="TEntity"/> entities.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to be loaded.</typeparam>
/// <typeparam name="TId">The type of the identifiers for the given entity.</typeparam>
public interface IReadEntityRepository<TEntity, TId>
    where TEntity : class, IEntity<TId>
{
    /// <summary>
    /// Determines if an entity exists with the given identifier.
    /// </summary>
    /// <param name="id">The identifier used to find the entity.</param>
    /// <returns>True if the entity exists, false otherwise.</returns>
    Task<bool> ExistsAsync(TId id);

    /// <summary>
    /// Retrieves all of the <see cref="TEntity"/> entities. Related objects will only be populated
    /// if their properties are included in the expand string.
    /// </summary>
    /// <param name="expand">A comma separated list of properties to populate. Default is null.</param>
    /// <returns>All <see cref="TEntity"/> entities.</returns>
    Task<IEnumerable<TEntity>> GetAllAsync(string expand = null);

    /// <summary>
    /// Retrieves a <see cref="TEntity"/> entity with the given identifier. Related objects will only be populated
    /// if their properties are included in the expand string.
    /// </summary>
    /// <param name="id">The identifier used to find the entity.</param>
    /// <param name="expand">A comma separated list of properties to populate. Default is null.</param>
    /// <returns>The entity with given identifier.</returns>
    Task<TEntity> GetByIdAsync(TId id, string expand = null);

    /// <summary>
    /// Finds all <see cref="TEntity"/> entities that match the given query expression. Related objects 
    /// will only be populated if their properties are included in the expand string.
    /// </summary>
    /// <param name="query">The query expression used to find entities.</param>
    /// <param name="expand">A comma separated list of properties to populate. Default is null.</param>
    /// <returns>All entities matching the query expression.</returns>
    Task<IEnumerable<TEntity>> SearchAsync(Expression<Func<TEntity, bool>> query = null, string expand = null);
}

/// <summary>
/// Defines an object that reads <see cref="TEntity"/> entities. The identifier for the entity will be of type <see cref="Guid"/>.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to be loaded.</typeparam>
/// <typeparam name="TId">The type of the identifiers for the given entity.</typeparam>
public interface IReadEntityRepository<TEntity> : IReadEntityRepository<TEntity, Guid>
    where TEntity : class, IEntity<Guid>
{
}
