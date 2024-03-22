using RevverDigital.Common.Data.Models.Interfaces;
using RevverDigital.Common.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace RevverDigital.Common.Data.Repositories;

/// <summary>
/// An implementation of <see cref="IWriteEntityRepository{TEntity, TId}"/> that uses EntityFramework.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to be loaded.</typeparam>
/// <typeparam name="TId">The type of the identifiers for the given entity.</typeparam>
public class WriteEntityRepository<TEntity, TId> : IWriteEntityRepository<TEntity, TId>
    where TEntity : class, IEntity<TId>, new()
{
    private readonly DbContext _entityContext;
    private readonly IUser _userInfo;

    /// <summary>
    /// Initializes a new instance of the <see cref="WriteEntityRepository{TEntity, TId}"/> class.
    /// </summary>
    /// <param name="db">The data context.</param>
    /// <param name="userInfo">The users information to tie to any changes.</param>
    /// <exception cref="ArgumentNullException">The exception thrown when the data context is null.</exception>
    public WriteEntityRepository(DbContext db, IUser userInfo)
    {
        _entityContext = db ?? throw new ArgumentNullException(nameof(db));
        _userInfo = userInfo;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WriteEntityRepository{TEntity, TId}"/> class.
    /// </summary>
    /// <param name="db">The data context.</param>
    /// <exception cref="ArgumentNullException">The exception thrown when the data context is null.</exception>
    public WriteEntityRepository(DbContext db)
    {
        _entityContext = db ?? throw new ArgumentNullException(nameof(db));
        _userInfo = null;
    }


    /// <summary>
    /// Inserts a new <see cref="TEntity"/> entity into the repository.
    /// </summary>
    /// <param name="entity">The entity to insert.</param>
    /// <returns>The same entity with updated values.</returns>
    public virtual async Task<TEntity> InsertAsync(TEntity entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException("Entity parameter cannot be null.");
        }

        SetUserId(entity);

        var newEntity = _entityContext.Set<TEntity>().Add(entity);
        await _entityContext.SaveChangesAsync();
        return newEntity.Entity;
    }

    /// <summary>
    /// Updates an existing <see cref="TEntity"/> entity.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <returns>The same entity with updated values.</returns>
    public virtual async Task<TEntity> UpdateAsync(TEntity entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException("Entity parameter cannot be null.");
        }

        SetUserId(entity);

        var entityFromDB = await _entityContext.Set<TEntity>().FindAsync(entity.Id);
        if (entityFromDB == null)
        {
            throw new Exception("Entity cannot be updated. Not entity exists with the given id.");
        }

        _entityContext.Entry(entityFromDB).CurrentValues.SetValues(entity);

        await _entityContext.SaveChangesAsync();
        return entityFromDB;
    }

    /// <summary>
    /// Soft deletes an entity with the given id.
    /// </summary>
    /// <param name="id">The identifier used to find the entity to delete.</param>
    public async Task DeleteByIdAsync(TId id)
    {
        if (id is null || id.Equals(default(TId)))
        {
            throw new ArgumentNullException(nameof(id));
        }

        var entityFromDB = await _entityContext.Set<TEntity>().FindAsync(id);
        if (entityFromDB == null)
        {
            throw new Exception("Entity cannot be deleted. Not entity exists with the given id.");
        }

        SetUserId(entityFromDB);

        entityFromDB.IsDeleted = true;

        await _entityContext.SaveChangesAsync();
    }

    /// <summary>
    /// Hard deletes an entity with the given id.
    /// </summary>
    /// <param name="id">The identifier used to find the entity to delete.</param>
    public async Task HardDeleteByIdAsync(TId id)
    {
        if (id is null || id.Equals(default(TId)))
        {
            throw new ArgumentNullException(nameof(id));
        }

        var entityFromDB = await _entityContext.Set<TEntity>().FindAsync(id);
        if (entityFromDB == null)
        {
            throw new Exception("Entity cannot be deleted. Not entity exists with the given id.");
        }

        _entityContext.Set<TEntity>().Remove(entityFromDB);
        await _entityContext.SaveChangesAsync();
    }

    protected void SetUserId(TEntity entity)
    {
        if(_userInfo != null && entity != null)
        {
            if (entity.Id is null || entity.Id.Equals(default(TId)))
            {
                if (entity is IUserCreatable creatable)
                {
                    creatable.CreatedBy = _userInfo.UserName;
                    creatable.CreatedDate = DateTime.Now;
                }
            }

            if (entity is IUserUpdatable updateable)
            {
                updateable.UpdatedBy = _userInfo.UserName;
                updateable.UpdatedDate = DateTime.Now;
            }
        }
    }
}

/// <summary>
/// An implementation of <see cref="IWriteEntityRepository{TEntity}"/> that uses EntityFramework. The identifier for the entity will be of type <see cref="Guid"/>.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to be loaded.</typeparam>
public class WriteEntityRepository<TEntity> : WriteEntityRepository<TEntity, Guid>
    where TEntity : class, IEntity<Guid>, new()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WriteEntityRepository{TEntity}"/> class.
    /// </summary>
    /// <param name="db">The data context.</param>
    /// <param name="userInfo">The users information to tie to any changes.</param>
    /// <exception cref="ArgumentNullException">The exception thrown when the data context is null.</exception>
    public WriteEntityRepository(DbContext db, IUser userInfo) : base(db, userInfo)
    {
    }
}