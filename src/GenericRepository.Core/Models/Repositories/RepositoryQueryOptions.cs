namespace GenericRepository.Core.Models.Repositories;

public record RepositoryQueryOptions : RepositoryQueryOptions<AccessRightsPolicyParams>;

/// <summary>
///     A type that represents options for repository request.
/// </summary>
public record RepositoryQueryOptions<TAccessRightsPolicyParams>(
    bool? LoadDetailedViewIncludes = null,
    bool? LoadCommonIncludes = null,
    bool? LoadOwnedProperties = null,
    bool Required = false,
    TAccessRightsPolicyParams? AccessRightsPolicyParams = default) where TAccessRightsPolicyParams : AccessRightsPolicyParams, new()
{
    /// <summary>
    ///     Gets an instance of <see cref="RepositoryQueryOptions{TAccessRightsPolicyParams}" /> configured for retrieving only parent entity. No
    ///     additional includes
    ///     performed.
    /// </summary>
    public static RepositoryQueryOptions<TAccessRightsPolicyParams> BareView => new(false, false, false);

    /// <summary>
    ///     Gets an instance of <see cref="RepositoryQueryOptions{TAccessRightsPolicyParams}" /> configured for default view. Only owned properties
    ///     are loaded.
    /// </summary>
    public static RepositoryQueryOptions<TAccessRightsPolicyParams> Default => new(false, false, true);

    /// <summary>
    ///     Gets options best fitting for detailed view for entity.
    /// </summary>
    public static RepositoryQueryOptions<TAccessRightsPolicyParams> DetailedView => new(true, true, true);

    /// <summary>
    ///     Gets an instance of <see cref="RepositoryQueryOptions{TAccessRightsPolicyParams}" /> configured for list view for entity.
    /// </summary>
    public static RepositoryQueryOptions<TAccessRightsPolicyParams> ListView => new(false, true, true);

    /// <summary>
    ///     Gets a copy of current <see cref="RepositoryQueryOptions{TAccessRightsPolicyParams}" /> with configured ThrowIfNoResult property.
    /// </summary>
    /// <param name="required">Boolean value indicating if this query should throw a NotFoundException if nothing is found.</param>
    /// <returns>An instance of <see cref="RepositoryQueryOptions{TAccessRightsPolicyParams}" /> with configured ThrowIfNoResult property.</returns>
    public RepositoryQueryOptions<TAccessRightsPolicyParams> SetRequired(bool required = true)
    {
        return this with
        {
            Required = required
        };
    }

    public RepositoryQueryOptions<TAccessRightsPolicyParams> SetAccessRightsPolicyParams(
        TAccessRightsPolicyParams? accessRightsPolicyParams)
    {
        return this with
        {
            AccessRightsPolicyParams = accessRightsPolicyParams
        };
    }

    public RepositoryQueryOptions<TAccessRightsPolicyParams> SetFullAccessRightsPolicyParams()
    {
        return this with
        {
            AccessRightsPolicyParams = new TAccessRightsPolicyParams
            {
                HasFullAccess = true
            }
        };
    }
}