namespace RevverDigital.Common.Data.Models.Interfaces;

/// <summary>
/// Defines an object that represents any user that is altering repository information.
/// </summary>
public interface IUser
{
    /// <summary>
    /// Gets or sets the user's UserName.
    /// </summary>
    public string UserName { get; set; }
}
