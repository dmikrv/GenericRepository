using System.Linq.Expressions;
using GenericRepository.Core.Common;
using GenericRepository.Core.Common.Auditable.SoftDelete;
using GenericRepository.Core.Contracts.QueryParams;
using GenericRepository.Core.Contracts.Repositories;
using GenericRepository.Core.Models.Repositories;
using GenericRepository.Core.Models.Repositories.Result;
using GenericRepository.Extensions;
using Microsoft.EntityFrameworkCore;

#pragma warning disable SA1402

namespace GenericRepository.Repositories;

public abstract class UnitOfWorkEntityRepositoryBase<TEntity> : UnitOfWorkEntityRepositoryBase<TEntity, Guid>
    where TEntity : class, IEntity<Guid>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UnitOfWorkEntityRepositoryBase{TEntity}" /> class.
    /// </summary>
    /// <param name="context">The db context.</param>
    /// <param name="dependencies">The common dependencies.</param>
    protected UnitOfWorkEntityRepositoryBase(DbContext context, RepositoryCommonDependencies dependencies) :
        base(context, dependencies)
    {
    }
}

/// <inheritdoc />
public class
    UnitOfWorkEntityRepositoryBase<TEntity, TPrimaryKey> : UnitOfWorkEntityRepositoryBase<TEntity, TPrimaryKey, DbContext>
    where TEntity : class, IEntity<TPrimaryKey> where TPrimaryKey : IEquatable<TPrimaryKey>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UnitOfWorkEntityRepositoryBase{TEntity,TPrimaryKey}" /> class.
    /// </summary>
    /// <param name="context">The db context.</param>
    /// <param name="dependencies">The common dependencies.</param>
    protected UnitOfWorkEntityRepositoryBase(DbContext context, RepositoryCommonDependencies dependencies) :
        base(context, dependencies)
    {
    }
}

/// <summary>
///     Provides overridable CRUD actions for entity.
/// </summary>
/// <typeparam name="TEntity">The type of data to work with.</typeparam>
/// <typeparam name="TPrimaryKey">A type of entity's primary key.</typeparam>
/// <typeparam name="TContext">The context type.</typeparam>
public class
    UnitOfWorkEntityRepositoryBase<TEntity, TPrimaryKey, TContext> : UnitOfWorkEntityRepositoryBase<TEntity, TPrimaryKey, TContext,
    IListItem<TPrimaryKey>> where TEntity : class, IEntity<TPrimaryKey>
    where TPrimaryKey : IEquatable<TPrimaryKey>
    where TContext : DbContext
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UnitOfWorkEntityRepositoryBase{TEntity, TPrimaryKey, TContext}" /> class.
    /// </summary>
    /// <param name="context">The db context.</param>
    /// <param name="dependencies">The common dependencies.</param>
    protected UnitOfWorkEntityRepositoryBase(TContext context, RepositoryCommonDependencies dependencies) : base(
        context,
        dependencies)
    {
    }
}

/// <summary>
///     Provides overridable CRUD actions for entity.
/// </summary>
/// <typeparam name="TEntity">The type of data to work with.</typeparam>
/// <typeparam name="TPrimaryKey">A type of entity's primary key.</typeparam>
/// <typeparam name="TContext">The context type.</typeparam>
/// <typeparam name="TAutocomplete">The type used for autocomplete.</typeparam>
public class UnitOfWorkEntityRepositoryBase<TEntity, TPrimaryKey, TContext, TAutocomplete> :
    UnitOfWorkEntityRepositoryBase<TEntity, TPrimaryKey, TContext, TAutocomplete, IQueryParams<TPrimaryKey>>
    where TEntity : class, IEntity<TPrimaryKey> where TPrimaryKey : IEquatable<TPrimaryKey> where TContext : DbContext
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UnitOfWorkEntityRepositoryBase{TEntity, TPrimaryKey, TContext}" /> class.
    /// </summary>
    /// <param name="context">The db context.</param>
    /// <param name="dependencies">The common dependencies.</param>
    protected UnitOfWorkEntityRepositoryBase(TContext context, RepositoryCommonDependencies dependencies) : base(
        context,
        dependencies)
    {
    }
}

/// <summary>
///     Provides overridable CRUD actions for entity.
/// </summary>
/// <typeparam name="TEntity">The type of data to work with.</typeparam>
/// <typeparam name="TPrimaryKey">A type of entity's primary key.</typeparam>
/// <typeparam name="TContext">The context type.</typeparam>
/// <typeparam name="TQueryParams">A model used to specify custom filters or other criteria for querying.</typeparam>
/// <typeparam name="TAutocomplete">The type used for autocomplete.</typeparam>
public class UnitOfWorkEntityRepositoryBase<TEntity, TPrimaryKey, TContext, TAutocomplete, TQueryParams> :
    UnitOfWorkEntityRepositoryBase<TEntity, TPrimaryKey, TContext, TAutocomplete, TQueryParams, AccessRightsPolicyParams>
    where TEntity : class, IEntity<TPrimaryKey>
    where TPrimaryKey : IEquatable<TPrimaryKey>
    where TContext : DbContext
    where TQueryParams : IQueryParams<TPrimaryKey>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UnitOfWorkEntityRepositoryBase{TEntity, TPrimaryKey, TContext}" /> class.
    /// </summary>
    /// <param name="context">The db context.</param>
    /// <param name="dependencies">The common dependencies.</param>
    protected UnitOfWorkEntityRepositoryBase(TContext context, RepositoryCommonDependencies dependencies) : base(
        context,
        dependencies)
    {
    }
}

/// <summary>
///     Provides overridable CRUD actions for entity.
/// </summary>
/// <typeparam name="TEntity">The type of data to work with.</typeparam>
/// <typeparam name="TPrimaryKey">A type of entity's primary key.</typeparam>
/// <typeparam name="TContext">The context type.</typeparam>
/// <typeparam name="TAutocomplete">The type used for autocomplete.</typeparam>
/// <typeparam name="TQueryParams">A model used to specify custom filters or other criteria for querying.</typeparam>
/// <typeparam name="TAccessRightsPolicyParams"></typeparam>
public class UnitOfWorkEntityRepositoryBase<TEntity, TPrimaryKey, TContext, TAutocomplete, TQueryParams, TAccessRightsPolicyParams> :
    UnitOfWorkRepositoryBase<TEntity, TContext, TQueryParams, TAccessRightsPolicyParams>,
    IEntityRepository<TEntity, TPrimaryKey, TAutocomplete, TQueryParams, TAccessRightsPolicyParams>
    where TEntity : class, IEntity<TPrimaryKey>
    where TPrimaryKey : IEquatable<TPrimaryKey>
    where TContext : DbContext
    where TQueryParams : IQueryParams<TPrimaryKey>
    where TAccessRightsPolicyParams : AccessRightsPolicyParams, new()
{
    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="UnitOfWorkEntityRepositoryBase{TEntity, TPrimaryKey, TContext, TAutocomplete,TQueryParams,TAccessRightsPolicyParams}" />
    ///     class.
    /// </summary>
    /// <param name="context">The db context.</param>
    /// <param name="dependencies">The common dependencies.</param>
    protected UnitOfWorkEntityRepositoryBase(TContext context, RepositoryCommonDependencies dependencies) : base(
        context,
        dependencies)
    {
    }

    /// <inheritdoc />
    protected override Expression<Func<TEntity, object?>> DefaultSortingSelector { get; } = x => x.Id;

    /// <inheritdoc />
    /// <returns>A tuple with two values - entity and primary key accessor.</returns>
    public new async Task<CreateEntityResult<TEntity, TPrimaryKey>> CreateAsync(TEntity entity,
        CancellationToken token = default)
    {
        var entityEntry = await base.CreateAsync(entity, token);

        var idProperty = Context.Entry(entityEntry).Property(x => x.Id);
        var valueAccessor = new ValueAccessor<TPrimaryKey>(() => idProperty.CurrentValue);

        return new CreateEntityResult<TEntity, TPrimaryKey>(entityEntry, valueAccessor);
    }

    /// <inheritdoc />
    public virtual async Task<TEntity?> GetByIdAsync(TPrimaryKey id, RepositoryQueryOptions<TAccessRightsPolicyParams>? options = null,
        CancellationToken token = default)
    {
        var queryById = await QueryByIdAsync(id, options);
        var result = await queryById.SingleOrDefaultAsync(token);

        if (options?.Required is true && result is null)
            throw ExceptionFactory.EntityNotFound<TEntity>(id);

        return result;
    }

    /// <inheritdoc />
    public virtual async Task<T?> GetByIdProjectedAsync<T>(TPrimaryKey id,
        RepositoryQueryOptions<TAccessRightsPolicyParams>? options = null,
        CancellationToken token = default)
    {
        var queryById = await QueryByIdAsync(id, options);

        var projectedQuery = await HandleProjectionAsync<T>(queryById);
        var result = await projectedQuery.SingleOrDefaultAsync(token);

        if (options?.Required == true && result == null)
            throw ExceptionFactory.EntityNotFound<TEntity>(id);

        return result;
    }

    /// <inheritdoc />
    public virtual async Task<bool> ExistsAsync(TPrimaryKey id, CancellationToken token = default)
    {
        var queryById = await QueryByIdAsync(id);
        return await queryById.AnyAsync(token);
    }

    public async Task<List<TPrimaryKey>> ExistsManyAsync(ICollection<TPrimaryKey> ids,
        RepositoryQueryOptions<TAccessRightsPolicyParams>? options = default, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(ids);

        var query = await GetBaseQueryAsync(options, token);
        query = await ApplyFilterByIdAsync(query, false, ids as TPrimaryKey[] ?? ids.ToArray());
        var existingIds = await query.Select(x => x.Id).ToListAsync(token);

        if (options is not null && options.Required && ids.Count != existingIds.Count)
            throw ExceptionFactory.EntityNotFound<TEntity>();

        return existingIds;
    }

    public async Task<List<TPrimaryKey>> ExistsManyAsync(TQueryParams? request,
        RepositoryQueryOptions<TAccessRightsPolicyParams>? options = default,
        CancellationToken token = default)
    {
        options = (options ?? RepositoryQueryOptions<TAccessRightsPolicyParams>.BareView) with
        {
            LoadDetailedViewIncludes = false,
            LoadCommonIncludes = false,
            LoadOwnedProperties = false
        };

        if (IsEmptyQuery(request)) return [];

        var query = await GetBaseQueryAsync(options, token);
        query = await ApplyFilteringAsync(query, request, token);

        var existingIds = await query.Select(x => x.Id).ToListAsync(token);

        if (options.Required) EnsureAny(existingIds);

        return existingIds;
    }

    /// <inheritdoc />
    public async Task MustExistsAsync(TPrimaryKey id, CancellationToken token = default)
    {
        if (!await ExistsAsync(id, token)) throw ExceptionFactory.EntityNotFound<TEntity>(id);
    }

    /// <inheritdoc />
    public Task<PagedResult<TAutocomplete>> AutocompleteAsync(TQueryParams? request,
        RepositoryQueryOptions<TAccessRightsPolicyParams>? options = default, CancellationToken token = default)
    {
        return GetProjectedAsync<TAutocomplete>(request, options, token);
    }


    protected override async Task<IQueryable<TEntity>> ApplyFilteringAsync(IQueryable<TEntity> query,
        TQueryParams? request,
        CancellationToken token = default)
    {
        query = await base.ApplyFilteringAsync(query, request, token);

        if (request?.Ids is not null && request.Ids.Length > 0)
            query = await ApplyFilterByIdAsync(query, request.IsInvertIds, request.Ids);

        if (request?.IsDeleted is not null)
            query = await ApplyFilterByDeletedAsync(query, request.IsDeleted.Value);

        return query;
    }

    /// <summary>
    ///     Applies filtering by id of provided query.
    /// </summary>
    /// <param name="query">The source query.</param>
    /// <param name="isInvertIds"></param>
    /// <param name="ids">An id of the desired entity.</param>
    /// <returns>A new query with applied filter by id.</returns>
    /// <exception cref="ArgumentNullException">Some of provided parameter was null or default.</exception>
    protected virtual ValueTask<IQueryable<TEntity>> ApplyFilterByIdAsync(IQueryable<TEntity> query, bool? isInvertIds,
        params TPrimaryKey[] ids)
    {
        ArgumentNullException.ThrowIfNull(query);

        return new ValueTask<IQueryable<TEntity>>(query.FilterById(isInvertIds ?? false, ids));
    }
    
    protected virtual ValueTask<IQueryable<TEntity>> ApplyFilterByDeletedAsync(IQueryable<TEntity> query, bool isDeleted)
    {
        if (!typeof(IAuditableIsDeleted).IsAssignableFrom(typeof(TEntity))) 
            return new ValueTask<IQueryable<TEntity>>(query);
        
        var filteredQuery = query.OfType<IAuditableIsDeleted>()
            .AsQueryable()
            .FilterByDeleted(isDeleted)
            .Cast<TEntity>(); // Cast back to TEntity
        return new ValueTask<IQueryable<TEntity>>(filteredQuery);
    }

    /// <summary>
    ///     Returns a query with applied filtering by provided id.
    /// </summary>
    /// <param name="id">The id of requested entity.</param>
    /// <param name="options">An options for the query.</param>
    /// <returns>An query for the entity with specified id.</returns>
    protected virtual async Task<IQueryable<TEntity>> QueryByIdAsync(TPrimaryKey id,
        RepositoryQueryOptions<TAccessRightsPolicyParams>? options = null)
    {
        options = (options ?? RepositoryQueryOptions<TAccessRightsPolicyParams>.DetailedView) with
        {
            LoadDetailedViewIncludes = options?.LoadDetailedViewIncludes ?? true
        };

        var query = await GetBaseQueryAsync(options);

        return await ApplyFilterByIdAsync(query, false, id);
    }

    protected override bool IsEmptyQuery(TQueryParams? request)
    {
        if (request?.IsInvertIds != true && request?.Ids is not null && request.Ids.Length == 0)
            return true;

        // TODO: check all collection filters

        return false;
    }
}