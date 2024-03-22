namespace RevverDigital.Common.Data.Models.Interfaces;

/// <summary>
/// Defines an entity that can be updated by a user.
/// </summary>
public interface IUserUpdatable
{
    /// <summary>
    /// Gets or sets the users that updated the entity.
    /// </summary>
    string UpdatedBy { get; set; }

    /// <summary>
    /// Gets or sets the date that the entity was updated.
    /// </summary>
    DateTime UpdatedDate { get; set; }
}
