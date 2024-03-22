using Microsoft.EntityFrameworkCore;

namespace RevverDigital.Common.Data;

/// <summary>
/// Creates an EntityFramework context that uses memory to store objects. This is intended
/// to be used to simplify testing, not for production use.
/// </summary>
/// <typeparam name="T">The type of entity stored in the repository.</typeparam>
public class InMemoryDBHelper<T>
    where T : DbContext
{
    public readonly T Context;

    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryDBHelper{T}"/> class.
    /// </summary>
    /// <param name="dbName">The name to give the context.</param>
    public InMemoryDBHelper(string dbName = null)
    {
        if (dbName == null)
            dbName = Guid.NewGuid().ToString();

        var builder = new DbContextOptionsBuilder<T>();
        builder.UseInMemoryDatabase(databaseName: dbName);

        var dbContextOptions = builder.Options;
        if (dbContextOptions != null)
        {
            Context = Activator.CreateInstance(typeof(T), dbContextOptions) as T;

            Reset();
        }

    }

    /// <summary>
    /// Completely wipes out all data in the database.
    /// </summary>
    public void Reset()
    {
        if (Context != null)
        {
            // Delete existing db before creating a new one
            Context.Database.EnsureDeleted();
            Context.Database.EnsureCreated();
        }
    }
}