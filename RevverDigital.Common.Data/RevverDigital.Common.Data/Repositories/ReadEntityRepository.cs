using Microsoft.EntityFrameworkCore;
using RevverDigital.Common.Data.Extensions;
using RevverDigital.Common.Data.Models.Interfaces;
using RevverDigital.Common.Data.Repositories.Interfaces;
using System.Linq.Expressions;

namespace RevverDigital.Common.Data.Repositories;

/// <summary>
/// An implementation of <see cref="IReadEntityRepository{TEntity, TId}"/> that uses EntityFramework.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to be loaded.</typeparam>
/// <typeparam name="TId">The type of the identifiers for the given entity.</typeparam>
public class ReadEntityRepository<TEntity, TId> : IReadEntityRepository<TEntity, TId>
    where TEntity : class, IEntity<TId>
{
    private readonly DbContext _entityContext;

    protected static readonly IDictionary<string, string> NavigationProperties;

    static ReadEntityRepository()
    {
        NavigationProperties = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        var type = typeof(TEntity);
        foreach (var prop in type.GetProperties())
        {
            if (typeof(IEntity).IsAssignableFrom(prop.PropertyType)
                || typeof(IEnumerable<IEntity>).IsAssignableFrom(prop.PropertyType))
            {
                NavigationProperties.Add(prop.Name.ToLower(), prop.Name);
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadEntityRepository{TEntity, TId}"/> class.
    /// </summary>
    /// <param name="db">The data context.</param>
    /// <exception cref="ArgumentNullException">The exception thrown when the data context is null.</exception>
    public ReadEntityRepository(DbContext db)
    {
        _entityContext = db ?? throw new ArgumentNullException(nameof(db));
    }

    /// <summary>
    /// Determines if an entity exists with the given identifier.
    /// </summary>
    /// <param name="id">The identifier used to find the entity.</param>
    /// <returns>True if the entity exists, false otherwise.</returns>
    public virtual async Task<bool> ExistsAsync(TId id)
    {
        if (id is null || id.Equals(default(TId)))
        {
            throw new ArgumentNullException(nameof(id));
        }

        return await _entityContext.Set<TEntity>().AsNoTracking().AnyAsync(x => id.Equals(x.Id) && !x.IsDeleted);
    }

    /// <summary>
    /// Retrieves all of the <see cref="TEntity"/> entities. Related objects will only be populated
    /// if their properties are included in the expand string.
    /// </summary>
    /// <param name="expand">A comma separated list of properties to populate. Default is null.</param>
    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(string expand = null)
    {
        return await _entityContext
            .Set<TEntity>()
            .ExpandNavigationProperties(NavigationProperties, expand)
            .Where(x => !x.IsDeleted)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a <see cref="TEntity"/> entity with the given identifier. Related objects will only be populated
    /// if their properties are included in the expand string.
    /// </summary>
    /// <param name="id">The identifier used to find the entity.</param>
    /// <param name="expand">A comma separated list of properties to populate. Default is null.</param>
    /// <returns>The entity with given identifier.</returns>
    public virtual async Task<TEntity> GetByIdAsync(TId id, string expand = null)
    {
        if (id is null || id.Equals(default(TId)))
        {
            throw new ArgumentNullException(nameof(id));
        }

        return await _entityContext
            .Set<TEntity>()
            .ExpandNavigationProperties(NavigationProperties, expand)
            .AsNoTracking()
            .SingleOrDefaultAsync(x => id.Equals(x.Id) && !x.IsDeleted);
    }

    /// <summary>
    /// Finds all <see cref="TEntity"/> entities that match the given query expression. Related objects 
    /// will only be populated if their properties are included in the expand string.
    /// </summary>
    /// <param name="query">The query expression used to find entities.</param>
    /// <param name="expand">A comma separated list of properties to populate. Default is null.</param>
    /// <returns>All entities matching the query expression.</returns>
    public virtual async Task<IEnumerable<TEntity>> SearchAsync(Expression<Func<TEntity, bool>> query = null, string expand = null)
    {
        return await _entityContext
            .Set<TEntity>()
            .ExpandNavigationProperties(NavigationProperties, expand)
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Where(query)
            .ToListAsync();
    }
}

/// <summary>
/// An implementation of <see cref="IReadEntityRepository{TEntity}"/> that uses EntityFramework. The identifier for the entity will be of type <see cref="Guid"/>.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to be loaded.</typeparam>
public class ReadEntityRepository<TEntity> : ReadEntityRepository<TEntity, Guid>
    where TEntity : class, IEntity<Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadEntityRepository{TEntity}"/> class.
    /// </summary>
    /// <param name="db">The data context.</param>
    public ReadEntityRepository(DbContext db) : base(db)
    {
    }
}