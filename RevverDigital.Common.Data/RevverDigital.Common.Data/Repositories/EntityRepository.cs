using RevverDigital.Common.Data.Models.Interfaces;
using RevverDigital.Common.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace RevverDigital.Common.Data.Repositories;

/// <summary>
/// An implementation of <see cref="IEntityRepository{TEntity, TId}"/> that uses EntityFramework.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to be loaded.</typeparam>
/// <typeparam name="TId">The type of the identifiers for the given entity.</typeparam>
public class EntityRepository<TEntity, TId> : IEntityRepository<TEntity, TId>
    where TEntity : class, IEntity<TId>, new()
{
    private IWriteEntityRepository<TEntity, TId> _writeEntityRepository;
    private IReadEntityRepository<TEntity, TId> _readEntityRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityRepository{TEntity, TId}"/> class.
    /// </summary>
    /// <param name="db">The data context.</param>
    /// <param name="userInfo">The users information to tie to any changes.</param>
    public EntityRepository(DbContext db, IUser userInfo)
    {
        _writeEntityRepository = new WriteEntityRepository<TEntity, TId>(db, userInfo);
        _readEntityRepository = new ReadEntityRepository<TEntity, TId>(db);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityRepository{TEntity, TId}"/> class.
    /// </summary>
    /// <param name="db">The data context.</param>
    public EntityRepository(DbContext db)
    {
        _writeEntityRepository = new WriteEntityRepository<TEntity, TId>(db);
        _readEntityRepository = new ReadEntityRepository<TEntity, TId>(db);
    }

    /// <summary>
    /// Determines if an entity exists with the given identifier.
    /// </summary>
    /// <param name="id">The identifier used to find the entity.</param>
    /// <returns>True if the entity exists, false otherwise.</returns>
    public virtual Task<bool> ExistsAsync(TId id) => _readEntityRepository.ExistsAsync(id);

    /// <summary>
    /// Retrieves all of the <see cref="TEntity"/> entities. Related objects will only be populated
    /// if their properties are included in the expand string.
    /// </summary>
    /// <param name="expand">A comma separated list of properties to populate. Default is null.</param>
    public virtual Task<IEnumerable<TEntity>> GetAllAsync(string expand = null) => _readEntityRepository.GetAllAsync(expand);

    /// <summary>
    /// Retrieves a <see cref="TEntity"/> entity with the given identifier. Related objects will only be populated
    /// if their properties are included in the expand string.
    /// </summary>
    /// <param name="id">The identifier used to find the entity.</param>
    /// <param name="expand">A comma separated list of properties to populate. Default is null.</param>
    public virtual Task<TEntity> GetByIdAsync(TId id, string expand = null) => _readEntityRepository.GetByIdAsync(id, expand);

    /// <summary>
    /// Finds all <see cref="TEntity"/> entities that match the given query expression. Related objects 
    /// will only be populated if their properties are included in the expand string.
    /// </summary>
    /// <param name="query">The query expression used to find entities.</param>
    /// <param name="expand">A comma separated list of properties to populate. Default is null.</param>
    /// <returns>All entities matching the query expression.</returns>
    public virtual Task<IEnumerable<TEntity>> SearchAsync(Expression<Func<TEntity, bool>> query = null, string expand = null) => _readEntityRepository.SearchAsync(query, expand);

    /// <summary>
    /// Inserts a new <see cref="TEntity"/> entity into the repository.
    /// </summary>
    /// <param name="entity">The entity to insert.</param>
    /// <returns>The same entity with updated values.</returns>
    public virtual Task<TEntity> InsertAsync(TEntity entity) => _writeEntityRepository.InsertAsync(entity);

    /// <summary>
    /// Updates an existing <see cref="TEntity"/> entity.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <returns>The same entity with updated values.</returns>
    public virtual Task<TEntity> UpdateAsync(TEntity entity) => _writeEntityRepository.UpdateAsync(entity);

    /// <summary>
    /// Soft deletes an entity with the given id.
    /// </summary>
    /// <param name="id">The identifier used to find the entity to delete.</param>
    public virtual Task DeleteByIdAsync(TId id) => _writeEntityRepository.DeleteByIdAsync(id);

    /// <summary>
    /// Hard deletes an entity with the given id.
    /// </summary>
    /// <param name="id">The identifier used to find the entity to delete.</param>
    public virtual Task HardDeleteByIdAsync(TId id) => _writeEntityRepository.HardDeleteByIdAsync(id);
}

/// <summary>
/// An implementation of <see cref="IEntityRepository{TEntity}"/> that uses EntityFramework. The identifier for the entity will be of type <see cref="Guid"/>.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to be loaded.</typeparam>
public class EntityRepository<TEntity> : EntityRepository<TEntity, Guid>
    where TEntity : class, IEntity<Guid>, new()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EntityRepository{TEntity}"/> class.
    /// </summary>
    /// <param name="db">The data context.</param>
    /// <param name="userInfo">The users information to tie to any changes.</param>
    public EntityRepository(DbContext db, IUser userInfo = null) : base(db, userInfo)
    {
    }
}
