using Microsoft.EntityFrameworkCore;
using RevverDigital.Common.Data.Models.Interfaces;

namespace RevverDigital.Common.Data.Extensions;

/// <summary>
/// Defines a set of extension methods for <see cref="IQueryable{T}"/>.
/// </summary>
public static class IQueryableExtensions
{
    /// <summary>
    /// Includes any expand properties into the query, if they are in the supported properties list.
    /// </summary>
    /// <typeparam name="T">The entity type for the IQueryable.</typeparam>
    /// <param name="query">The query being expanded.</param>
    /// <param name="supportedPropertiesMap">A set of properties that are allowed to be included.</param>
    /// <param name="expand">A comma separated list of properties to populate. Default is null.</param>
    /// <returns>The original query with included expand properties.</returns>
    public static IQueryable<T> ExpandNavigationProperties<T>(
        this IQueryable<T> query,
        IDictionary<string, string> supportedPropertiesMap,
        string expand)
        where T : class, IEntity
    {
        if (supportedPropertiesMap == null)
        {
            return query;
        }

        if (string.IsNullOrWhiteSpace(expand))
        {
            return query;
        }

        var properties = expand.Split(',');
        foreach (var property in properties)
        {
            if (string.IsNullOrWhiteSpace(property))
            {
                continue;
            }

            if (supportedPropertiesMap.TryGetValue(property.Trim(), out string propertyName))
            {
                query = query.Include(propertyName);
            }
        }

        return query;
    }
}
