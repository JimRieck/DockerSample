namespace RevverDigital.Common.Data.Models.Interfaces;

/// <summary>
/// Defines an entity object with a generic identifier, used for all the repository operations.
/// </summary>
/// <typeparam name="T">The type of the identifier.</typeparam>
public interface IEntity<T> : IEntity
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public T Id { get; set; }
}

/// <summary>
/// Defines an Entity object, used for all the repository operations.
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Gets or sets whether or not this object has been deleted.
    /// </summary>
    public bool IsDeleted { get; set; }
}
