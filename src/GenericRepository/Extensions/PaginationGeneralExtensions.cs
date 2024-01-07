using GenericRepository.Core.Contracts.QueryParams;
using GenericRepository.Core.Enums;

namespace GenericRepository.Extensions;

/// <summary>
///     A set of extension methods to work with <see cref="ISortedQueryParams" />.
/// </summary>
public static class PaginationGeneralExtensions
{
    /// <summary>
    ///     Returns a <paramref name="sortingDirection" /> it is determined and <paramref name="defaultSortingDirection" /> otherwise.
    /// </summary>
    /// <param name="sortingDirection">The sorting sortingDirection.</param>
    /// <param name="defaultSortingDirection">The default value of sorting direction.</param>
    /// <returns>Final sorting direction.</returns>
    public static SortingDirection DefaultIfNotSet(
        this SortingDirection sortingDirection,
        SortingDirection defaultSortingDirection = SortingDirection.NotSet)
    {
        return sortingDirection == SortingDirection.NotSet
            ? defaultSortingDirection
            : sortingDirection;
    }

    /// <summary>
    ///     Returns a <paramref name="sortingDirection" /> it is determined and <paramref name="defaultSortingDirection" /> otherwise.
    /// </summary>
    /// <param name="sortingDirection">The sorting sortingDirection.</param>
    /// <param name="defaultSortingDirection">The default value of sorting direction.</param>
    /// <returns>Final sorting direction.</returns>
    public static SortingDirection DefaultIfNotSet(
        this SortingDirection? sortingDirection,
        SortingDirection defaultSortingDirection = SortingDirection.NotSet)
    {
        return sortingDirection?.DefaultIfNotSet(defaultSortingDirection) ?? defaultSortingDirection;
    }
}