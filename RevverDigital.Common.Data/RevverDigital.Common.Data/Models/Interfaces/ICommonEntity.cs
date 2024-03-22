namespace RevverDigital.Common.Data.Models.Interfaces;

/// <summary>
/// Defines a common type of <see cref="IEntity{T}"/>, that includes <see cref="IUserCreatable"/> and <see cref="IUserUpdatable"/>.
/// </summary>
/// <typeparam name="T">The type of the identifier.</typeparam>
public interface ICommonEntity<T> : IEntity<T>, IUserCreatable, IUserUpdatable
{
}

/// <summary>
/// Defines a version of <see cref="ICommonEntity{T}"/> that uses a <see cref="Guid"/> identifier.
/// </summary>
public interface ICommonEntity : ICommonEntity<Guid>
{
}
