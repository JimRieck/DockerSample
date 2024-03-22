namespace RevverDigital.Common.Data.Models.Interfaces;

/// <summary>
/// Defines an entity that can be created by a user.
/// </summary>
public interface IUserCreatable
{
    /// <summary>
    /// Gets or sets the user that created this entity.
    /// </summary>
    string CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the date that this entity was created.
    /// </summary>
    DateTime CreatedDate { get; set; }
}
