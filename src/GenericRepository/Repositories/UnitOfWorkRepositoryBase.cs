using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using GenericRepository.Core.Common;
using GenericRepository.Core.Common.Auditable;
using GenericRepository.Core.Contracts;
using GenericRepository.Core.Contracts.QueryParams;
using GenericRepository.Core.Contracts.Repositories;
using GenericRepository.Core.Enums;
using GenericRepository.Core.Exceptions;
using GenericRepository.Core.Models.Repositories;
using GenericRepository.Extensions;
using GenericRepository.Helpers;
using GenericRepository.OwnedPropertiesTree;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using DynamicQueryableExtensions = System.Linq.Dynamic.Core.DynamicQueryableExtensions;

namespace GenericRepository.Repositories;

/// <summary>
///     Base class for generic repository.
/// </summary>
/// <typeparam name="TEntity">The type of data to work with.</typeparam>
/// <typeparam name="TContext">The context type.</typeparam>
/// <typeparam name="TQueryParams">A model used to specify custom filters or other criteria for querying.</typeparam>
/// <typeparam name="TAccessRightsPolicyParams"></typeparam>
public abstract class UnitOfWorkRepositoryBase<TEntity, TContext, TQueryParams, TAccessRightsPolicyParams>
    : IQueryableRepository<TEntity, TQueryParams, TAccessRightsPolicyParams>
    where TContext : DbContext
    where TEntity : class
    where TQueryParams : IQueryParams
    where TAccessRightsPolicyParams : AccessRightsPolicyParams, new()
{
    private IReadOnlyList<IProperty>? _keyProperties;

    /// <summary>
    ///     Initializes a new instance of the <see cref="UnitOfWorkRepositoryBase{TEntity,TContext,TQueryParams,TAccessRightsPolicyParams}" />
    ///     class.
    /// </summary>
    /// <param name="context">The db context.</param>
    /// <param name="dependencies">The common dependencies.</param>
    protected UnitOfWorkRepositoryBase(TContext context, RepositoryCommonDependencies dependencies)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));

        (Mapper, LoggerFactory, ExceptionFactory, TenantIdProvider) = dependencies;
        Logger = dependencies.LoggerFactory.CreateLogger(GetType());

        Set = context.Set<TEntity>();
    }

    /// <summary>
    ///     Gets default properties to include in all queries.
    /// </summary>
    protected virtual IReadOnlyList<Expression<Func<TEntity, object>>> CommonIncludes => null!;

    /// <summary>
    ///     Gets the DbContext.
    /// </summary>
    protected TContext Context { get; }

    /// <summary>
    ///     Gets default order direction for sorting.
    /// </summary>
    protected virtual SortingDirection DefaultSortingDirection => SortingDirection.Asc;

    /// <summary>
    ///     Gets default parameter for sorting. This parameter have to be overriden in order to get predictable pagination behaviour.
    /// </summary>
    protected abstract Expression<Func<TEntity, object?>> DefaultSortingSelector { get; }

    /// <summary>
    ///     Gets properties to include in queries of one single entity.
    /// </summary>
    protected virtual IReadOnlyList<Expression<Func<TEntity, object>>> DetailedViewIncludes => null!;

    /// <summary>
    ///     Gets the exception factory.
    /// </summary>
    protected virtual IRepositoryExceptionFactory ExceptionFactory { get; }

    protected virtual ITenantIdProvider? TenantIdProvider { get; }


    /// <summary>
    ///     Gets a key properties of <typeparamref name="TEntity" />.
    /// </summary>
    protected IReadOnlyList<IProperty> KeyProperties =>
        _keyProperties ??= Context.GetPrimaryKeyProperties<TEntity>() ??
                           throw new NotSupportedException("Can't handle keyless entities.");

    /// <summary>
    ///     Gets the logger.
    /// </summary>
    protected virtual ILogger Logger { get; }

    /// <summary>
    ///     Gets the logger factory.
    /// </summary>
    protected virtual ILoggerFactory LoggerFactory { get; }

    /// <summary>
    ///     Gets the automapper used to convert types.
    /// </summary>
    protected virtual IMapper Mapper { get; }

    /// <summary>
    ///     Gets owned properties of <typeparamref name="TEntity" />.
    ///     Owned property is a property which can't be separated from parent entity. Considered as part of parent entity.
    /// </summary>
    protected virtual IReadOnlyList<Expression<Func<TEntity, object>>>? OwnedProperties => null;

    /// <summary>
    ///     Gets predicate that used for records filtering with <see cref="IQueryParams" />.
    /// </summary>
    protected virtual Func<string, Expression<Func<TEntity, bool>>>? SearchPredicate => null;

    /// <summary>
    ///     Gets predicate that used for records filtering with <see cref="IQueryParams" />.
    /// </summary>
    protected virtual Func<string, Expression<Func<TEntity, bool>>>? TypeaheadPredicate => null;

    /// <summary>
    ///     Gets a <see cref="DbSet{TEntity}" /> object of <typeparamref name="TContext" />.
    /// </summary>
    protected DbSet<TEntity> Set { get; }

    /// <summary>
    ///     Gets a dictionary with allowed sorting parameters. The key is parameter name, the value is property selector.
    /// </summary>
    protected virtual IReadOnlyDictionary<string, Expression<Func<TEntity, object?>>>? SortingConfiguration => null;

    protected virtual Expression<Func<TEntity, bool>> AccessRightPolicyDefaultExpression => _ => false;

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(RepositoryQueryOptions<TAccessRightsPolicyParams>? options = null,
        CancellationToken token = default)
    {
        var baseQuery = await GetBaseQueryAsync(options, token);
        var result = await baseQuery.ToListAsync(token);

        if (options?.Required == true) EnsureAny(result);

        return result;
    }

    /// <inheritdoc />
    public virtual async Task<PagedResult<TEntity>> GetAsync(TQueryParams request,
        RepositoryQueryOptions<TAccessRightsPolicyParams>? options = null,
        CancellationToken token = default)
    {
        options = (options ?? RepositoryQueryOptions<TAccessRightsPolicyParams>.DetailedView) with
        {
            LoadDetailedViewIncludes = options?.LoadDetailedViewIncludes ?? true
        };

        if (IsEmptyQuery(request))
        {
            var res = PagedResult.Empty<TEntity>(request.PageNumber, request.PageSize);
            if (options.Required) EnsureAny(res.Results);
            return res;
        }

        var query = await GetBaseQueryAsync(options, token);
        var result = await GetDataByRepositoryRequestAsync(query, request, token);

        if (options.Required) EnsureAny(result.Results);

        return result;
    }

    /// <inheritdoc />
    public virtual async Task<bool> AnyAsync(TQueryParams? request,
        RepositoryQueryOptions<TAccessRightsPolicyParams>? options = null,
        CancellationToken token = default)
    {
        options = (options ?? RepositoryQueryOptions<TAccessRightsPolicyParams>.BareView) with
        {
            LoadDetailedViewIncludes = false,
            LoadCommonIncludes = false,
            LoadOwnedProperties = false
        };

        if (IsEmptyQuery(request))
        {
            if (options.Required) throw ExceptionFactory.EntityNotFound<TEntity>();
            return false;
        }

        var query = await GetBaseQueryAsync(options, token);
        query = await ApplyFilteringAsync(query, request, token);
        var any = await query.AnyAsync(token);

        if (options.Required && !any) throw ExceptionFactory.EntityNotFound<TEntity>();

        return any;
    }

    /// <inheritdoc />
    public virtual async Task<int> CountAsync(TQueryParams? request,
        RepositoryQueryOptions<TAccessRightsPolicyParams>? options = null,
        CancellationToken token = default)
    {
        options = (options ?? RepositoryQueryOptions<TAccessRightsPolicyParams>.BareView) with
        {
            LoadDetailedViewIncludes = false,
            LoadCommonIncludes = false,
            LoadOwnedProperties = false
        };

        if (IsEmptyQuery(request))
        {
            if (options.Required) throw ExceptionFactory.EntityNotFound<TEntity>();
            return 0;
        }

        var query = await GetBaseQueryAsync(options, token);
        query = await ApplyFilteringAsync(query, request, token);
        var count = await query.CountAsync(token);

        if (options.Required && count <= 0) throw ExceptionFactory.EntityNotFound<TEntity>();

        return count;
    }

    /// <inheritdoc />
    public virtual async Task<PagedResult<T>> GetProjectedAsync<T>(TQueryParams? request,
        RepositoryQueryOptions<TAccessRightsPolicyParams>? options = null,
        CancellationToken token = default)
    {
        options = (options ?? RepositoryQueryOptions<TAccessRightsPolicyParams>.BareView) with
        {
            LoadDetailedViewIncludes = false,
            LoadCommonIncludes = false,
            LoadOwnedProperties = false
        };

        if (IsEmptyQuery(request))
        {
            var res = PagedResult.Empty<T>(request?.PageNumber ?? IPagedQueryParams.DefaultPageNumber,
                request?.PageSize ?? IPagedQueryParams.DefaultPageSize);
            if (options.Required) EnsureAny(res.Results);
            return res;
        }

        var query = await GetBaseQueryAsync(options, token);

        SearchGuard(request);
        TypeaheadGuard(request);
        SortGuard(request);

        query = await ApplyRepositoryRequestAsync(query, request, token);

        var projectedQuery = await HandleProjectionAsync<T>(query);

        var result = await ApplyPagination(projectedQuery, request, token);

        if (options.Required) EnsureAny(result.Results);

        return result;
    }

    /// <inheritdoc />
    public virtual async Task<TEntity> CreateAsync(TEntity entity, CancellationToken token = default)
    {
        var query = Set.AsQueryable();
        query = await IncludeOwnedPropertiesAsync(query, token);

        // TODO: caching
        var ownedPropertiesTree = new OwnedPropertiesTreeExtractor().ExtractOwnedPropertiesTreeNodes(query);
        await UpdateOwnedPropertiesTreeAsync(null, entity, ownedPropertiesTree, token);

        // Ensure the data are not contradictory
        Context.ChangeTracker.DetectChanges();

        return entity;
    }

    /// <inheritdoc />
    public virtual Task DeleteAsync(TEntity entity, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        Context.Remove(entity);

        return Task.CompletedTask;
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken token = default)
    {
        var query = Context.GetFilteredByIdQuery(entity);
        query = await IncludeOwnedPropertiesAsync(query, token);

        var existingEntity = await query.SingleOrDefaultAsync(token);
        if (existingEntity is null) throw ExceptionFactory.EntityNotFound<TEntity>();

        var ownedPropertiesTree = new OwnedPropertiesTreeExtractor().ExtractOwnedPropertiesTreeNodes(query);
        await UpdateOwnedPropertiesTreeAsync(existingEntity, entity, ownedPropertiesTree, token);

        // Ensure the data are not contradictory
        Context.ChangeTracker.DetectChanges();

        return existingEntity;
    }

    public async Task<bool> IsAggregateModifiedAsync(TEntity entity, CancellationToken token = default)
    {
        var query = Set.AsQueryable();
        query = await IncludeOwnedPropertiesAsync(query, token);

        var ownedPropertiesTree = new OwnedPropertiesTreeExtractor().ExtractOwnedPropertiesTreeNodes(query);
        return IsOwnedPropertiesModifiedAsync(entity, ownedPropertiesTree);
    }

    protected virtual async Task UpdateOwnedPropertiesTreeAsync<T>(T? existingEntity, T entity, OwnedPropertiesTreeNode node,
        CancellationToken token = default) where T : class
    {
        var existingEntry = existingEntity is not null ? Context.Entry(existingEntity) : null;
        var entry = Context.Entry(entity);

        if (existingEntry is null)
            entry.State = EntityState.Added;
        else
            existingEntry.CurrentValues.SetValues(entity);

        foreach (var childOwnedProperty in node.Children.Values)
        {
            var childOwnedPropertyNavigation = entry.Navigation(childOwnedProperty.Name);
            var existingOwnedPropertyNavigation = existingEntry?.Navigation(childOwnedProperty.Name); // TODO: slow

            var childEntities = childOwnedPropertyNavigation.GetCollectionOfEntities();
            var existingChildEntities = existingOwnedPropertyNavigation?.GetCollectionOfEntities() ?? [];

            // adding and modifying
            if (childEntities.Any())
            {
                var existingChildEntitiesPrimaryKeys = existingChildEntities
                    .ToDictionary(k => GetPrimaryKey(k.GetType(), k), v => v, new CollectionOfEntitiesEqualityComparer()); // TODO: slow

                var childOwnedPropertyForeignKey = childOwnedPropertyNavigation is RuntimeSkipNavigation
                    ? (RuntimeForeignKey)((IReadOnlySkipNavigation)childOwnedPropertyNavigation.Metadata).ForeignKey!
                    : ((RuntimeNavigation)childOwnedPropertyNavigation.Metadata).ForeignKey;
                
                var foreignKeyPropertyNames = childOwnedPropertyForeignKey.Properties.Select(x => x.Name);
                var foreignPrincipalKeyPropertyNames = childOwnedPropertyForeignKey.PrincipalKey.Properties.Select(x => x.Name);
                var foreignKeysPropertyNames = foreignKeyPropertyNames.Zip(foreignPrincipalKeyPropertyNames).ToArray();

                foreach (var childEntity in childEntities) // TODO: slow, send request to db
                {
                    foreach (var foreignKeysPropertyName in foreignKeysPropertyNames)
                        Context.Entry(childEntity).Property(foreignKeysPropertyName.First).CurrentValue =
                            entry.Property(foreignKeysPropertyName.Second).CurrentValue;

                    var childEntityPrimaryKey = GetPrimaryKey(childEntity.GetType(), childEntity);
                    await UpdateOwnedPropertiesTreeAsync(existingChildEntitiesPrimaryKeys.GetValueOrDefault(childEntityPrimaryKey),
                        childEntity, childOwnedProperty, token);
                }
            }

            // removing
            if (existingEntry is null) continue;

            var childEntitiesPrimaryKeys =
                childEntities.Select(x => GetPrimaryKey(x.GetType(), x)).ToHashSet(new CollectionOfEntitiesEqualityComparer());

            var modifiedEntities = new List<object>();
            foreach (var existingChildEntity in existingChildEntities) // TODO: slow
            {
                var existingChildEntityPrimaryKey = GetPrimaryKey(existingChildEntity.GetType(), existingChildEntity);
                if (!childEntitiesPrimaryKeys.Contains(existingChildEntityPrimaryKey))
                    Context.Remove(existingChildEntity);
                else
                    modifiedEntities.Add(existingChildEntity);
            }

            // rewrite array of owned properties
            if (existingOwnedPropertyNavigation?.CurrentValue is null || !existingOwnedPropertyNavigation.Metadata.IsCollection)
                continue;

            var modifiedEntitiesPrimaryKeys =
                modifiedEntities.Select(x => GetPrimaryKey(x.GetType(), x))
                    .ToHashSet(new CollectionOfEntitiesEqualityComparer()); // TODO: slow
            foreach (var childEntity in childEntities)
            {
                var childEntityPrimaryKey = GetPrimaryKey(childEntity.GetType(), childEntity);
                if (!modifiedEntitiesPrimaryKeys.Contains(childEntityPrimaryKey))
                    modifiedEntities.Add(childEntity);
            }

            var type = existingOwnedPropertyNavigation.CurrentValue.GetType();
            var clearMethod = type.GetMethod(nameof(ICollection<object>.Clear)) ?? throw new("No found clear method");
            var addMethod = type.GetMethod(nameof(ICollection<object>.Add)) ?? throw new("No found add method");

            clearMethod.Invoke(existingOwnedPropertyNavigation.CurrentValue, null);
            foreach (var modifiedEntity in modifiedEntities)
                addMethod.Invoke(existingOwnedPropertyNavigation.CurrentValue, new[] { modifiedEntity });
        }
    }

    protected object?[] GetPrimaryKey(Type type, object? entity)
    {
        if (entity is null) throw new NullReferenceException(nameof(entity));

        var entry = Context.Entry(entity);
        var keyProperties = Context.GetPrimaryKeyProperties(type); // TODO: caching

        return keyProperties.Select(property => entry.Property(property).CurrentValue).ToArray();
    }

    protected virtual Task<IQueryable<TEntity>> IncludeOwnedPropertiesAsync(IQueryable<TEntity> query, CancellationToken token = default)
    {
        // override this method if additional filtering options are needed.
        return Task.FromResult(query);
    }

    protected virtual Task<IQueryable<TEntity>> IncludeCommonIncludesAsync(IQueryable<TEntity> query, CancellationToken token = default)
    {
        // override this method if additional filtering options are needed.
        return Task.FromResult(query);
    }

    protected virtual Task<IQueryable<TEntity>> IncludeDetailedViewIncludesAsync(IQueryable<TEntity> query,
        CancellationToken token = default)
    {
        // override this method if additional filtering options are needed.
        return Task.FromResult(query);
    }

    protected ValueTask<IQueryable<T>> HandleProjectionAsync<T>(IQueryable<TEntity> query)
    {
        if (!Mapper.IsMappingExists<TEntity, T>())
            throw ExceptionFactory.NotImplemented($"Projections are not available for repository {GetType().Name}.\n" +
                                                  "In order to fix this you can try next solutions:\n" +
                                                  "1. Define EF translatable map in automapper:\n" +
                                                  $"  CreateMap<{typeof(TEntity).Name}, {typeof(T).Name}>();\n" +
                                                  $"2. Override {nameof(HandleProjectionAsync)} method and handle this error manually.");

        return new(query.ProjectTo<T>(Mapper.ConfigurationProvider));
    }

    /// <summary>
    ///     Checks if repository implements all required for request functions.
    /// </summary>
    /// <param name="request">The request to be applied.</param>
    /// <exception cref="NotImplementedDomainException">Thrown if some function not implemented.</exception>
    protected virtual void SortGuard(TQueryParams? request)
    {
        if (!string.IsNullOrEmpty(request?.SortBy) && SortingConfiguration == null)
            throw ExceptionFactory.NotImplemented(
                $"Ordering is not supported for this entity. To enable support you can override {nameof(SortingConfiguration)}.");
    }

    /// <summary>
    ///     Checks if repository implements all required for request functions.
    /// </summary>
    /// <param name="request">The request to be applied.</param>
    /// <exception cref="NotImplementedDomainException">Thrown if some function not implemented.</exception>
    protected virtual void SearchGuard(TQueryParams? request)
    {
        if (!string.IsNullOrEmpty(request?.Search) && SearchPredicate == null)
            throw ExceptionFactory.NotImplemented(
                $"Querying is not supported for this entity. To enable support you can override {nameof(SearchPredicate)}.");
    }

    /// <summary>
    ///     Checks if repository implements all required for request functions.
    /// </summary>
    /// <param name="request">The request to be applied.</param>
    /// <exception cref="NotImplementedDomainException">Thrown if some function not implemented.</exception>
    protected virtual void TypeaheadGuard(TQueryParams? request)
    {
        if (!string.IsNullOrEmpty(request?.Typeahead) && TypeaheadPredicate == null)
            throw ExceptionFactory.NotImplemented(
                $"Typeahead is not supported for this entity. To enable support you can override {nameof(SearchPredicate)}.");
    }

    /// <summary>
    ///     Applies includes, filtering, search and ordering to provided query.
    /// </summary>
    /// <param name="query">The source query.</param>
    /// <param name="request">The request to be applied.</param>
    /// <param name="token">A cancellation token.</param>
    /// <returns>A new instance of query but with repository request parameters applied.</returns>
    protected virtual async Task<IQueryable<TEntity>> ApplyRepositoryRequestAsync(IQueryable<TEntity> query,
        TQueryParams? request,
        CancellationToken token = default)
    {
        query = await ApplyFilteringAsync(query, request, token);
        query = await ApplySearchAsync(query, request, SearchPredicate, token);
        query = await ApplyTypeaheadAsync(query, request, TypeaheadPredicate, token);
        query = await ApplySortingAsync(query, request, SortingConfiguration, DefaultSortingSelector, DefaultSortingDirection, token);
        return query;
    }

    /// <summary>
    ///     Applies filtering to provided query.
    /// </summary>
    /// <param name="query">The source query.</param>
    /// <param name="request">The request to be applied.</param>
    /// <param name="token">A cancellation token.</param>
    /// <returns>A new instance of query but filtering applied.</returns>
    protected virtual Task<IQueryable<TEntity>> ApplyFilteringAsync(IQueryable<TEntity> query,
        TQueryParams? request,
        CancellationToken token = default)
    {
        // override this method if additional filtering options are needed.
        return Task.FromResult(query);
    }

    /// <summary>
    ///     Applies search to provided query.
    /// </summary>
    /// <param name="query">The source query.</param>
    /// <param name="request">The request to be applied.</param>
    /// <param name="typeaheadPredicate">A predicate that used for records filtering.</param>
    /// <param name="token">A cancellation token.</param>
    /// <typeparam name="T">An underlying type of the query.</typeparam>
    /// <exception cref="NotImplementedException">This repository has no defined <see cref="typeaheadPredicate" />.</exception>
    /// <returns>A new instance of query but search applied.</returns>
    protected virtual ValueTask<IQueryable<T>> ApplyTypeaheadAsync<T>(IQueryable<T> query,
        TQueryParams? request,
        Func<string, Expression<Func<T, bool>>>? typeaheadPredicate,
        CancellationToken token = default)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));

        if (string.IsNullOrEmpty(request?.Typeahead))
            return new(query);

        if (typeaheadPredicate == null)
            throw ExceptionFactory.NotImplemented("Typeahead is not supported.");

        var expr = typeaheadPredicate.Invoke(request.Typeahead);

        query = query.Where(expr);

        return new(query);
    }

    /// <summary>
    ///     Applies search to provided query.
    /// </summary>
    /// <param name="query">The source query.</param>
    /// <param name="request">The request to be applied.</param>
    /// <param name="searchPredicate">A predicate that used for records filtering.</param>
    /// <param name="token">A cancellation token.</param>
    /// <typeparam name="T">An underlying type of the query.</typeparam>
    /// <exception cref="NotImplementedException">This repository has no defined <see cref="searchPredicate" />.</exception>
    /// <returns>A new instance of query but search applied.</returns>
    protected virtual ValueTask<IQueryable<T>> ApplySearchAsync<T>(IQueryable<T> query,
        TQueryParams? request,
        Func<string, Expression<Func<T, bool>>>? searchPredicate,
        CancellationToken token = default)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));

        if (string.IsNullOrEmpty(request?.Search))
            return new(query);

        var queryRequestQuery = request.Search;
        var searchTokens = SearchInputTokenizer.TokenizeSearch(queryRequestQuery);

        if (searchPredicate == null)
            throw ExceptionFactory.NotImplemented("Searching is not supported.");

        if (!searchTokens.Any())
            return new(query);

        var expr = searchPredicate.Invoke(searchTokens[0]);
        expr = searchTokens.Skip(1).Aggregate(expr, (accum, searchToken) => accum.AndAlso(searchPredicate.Invoke(searchToken)));

        query = query.Where(expr);

        return new(query);
    }

    /// <summary>
    ///     Applies dynamic (by properties) sorting to provided query.
    /// </summary>
    /// <param name="query">The source query.</param>
    /// <param name="orderBy">A name of property to sort by.</param>
    /// <param name="sortingDirection">The sorting direction.</param>
    /// <typeparam name="T">An underlying type of the query.</typeparam>
    /// <returns>A new instance of query but sorting applied.</returns>
    protected virtual ValueTask<IQueryable<T>> ApplyDynamicSortingAsync<T>(IQueryable<T> query, string? orderBy,
        SortingDirection sortingDirection)
    {
        if (string.IsNullOrWhiteSpace(orderBy))
            return new(query);

        if (sortingDirection == SortingDirection.Desc)
            orderBy += " DESC";

        return new(DynamicQueryableExtensions.OrderBy(query, orderBy));
    }

    /// <summary>
    ///     Applies sorting to provided query.
    /// </summary>
    /// <param name="query">The source query.</param>
    /// <param name="request">The request to be applied.</param>
    /// <param name="sortingConfiguration">A dictionary with allowed sorting parameters. The key is parameter name, the value is property selector.</param>
    /// <param name="defaultSortingSelector">The default parameter for sorting.</param>
    /// <param name="defaultSortingDirection">The default order direction for sorting.</param>
    /// <param name="token">A cancellation token.</param>
    /// <typeparam name="T">An underlying type of the query.</typeparam>
    /// <typeparam name="TSelector">A type of sort selector expression.</typeparam>
    /// <returns>A new instance of query but sorting applied.</returns>
    protected virtual ValueTask<IQueryable<T>> ApplySortingAsync<T, TSelector>(IQueryable<T> query,
        TQueryParams? request,
        IReadOnlyDictionary<string, Expression<Func<T, TSelector?>>>? sortingConfiguration = null,
        Expression<Func<T, TSelector?>>? defaultSortingSelector = null,
        SortingDirection defaultSortingDirection = SortingDirection.Asc,
        CancellationToken token = default)
    {
        var sortBy = ToUpperCamelCase(request?.SortBy); // TODO: JSON helper has the same method
        var sortDirection = request?.SortDirection ?? defaultSortingDirection;

        var hasOrderingProperty = !string.IsNullOrEmpty(sortBy);

        if (hasOrderingProperty && sortingConfiguration?.Any() != true)
            return ApplyDynamicSortingAsync(query, sortBy, sortDirection.DefaultIfNotSet(defaultSortingDirection));

        var sortingSelector = hasOrderingProperty
            ? GetSortingSelector(sortBy, sortingConfiguration)
            : defaultSortingSelector;

        return ApplySortingAsync(query, sortingSelector, sortDirection, defaultSortingSelector, defaultSortingDirection, token);
    }

    protected static string? ToUpperCamelCase(string? str)
    {
        return str is null ? null : $"{str[0].ToString().ToUpper()}{str[1..]}";
    }

    /// <summary>
    ///     Applies sorting to provided query.
    /// </summary>
    /// <param name="query">The source query.</param>
    /// <param name="sortingSelector">The selector to sort by.</param>
    /// <param name="sortingDirection">The sorting direction.</param>
    /// <param name="defaultSortingSelector">The default parameter for sorting.</param>
    /// <param name="defaultSortingDirection">The default order direction for sorting.</param>
    /// <param name="token">A cancellation token.</param>
    /// <typeparam name="T">An underlying type of the query.</typeparam>
    /// <typeparam name="TSelector">A type of sort selector expression.</typeparam>
    /// <returns>A new instance of query but sorting applied.</returns>
    protected virtual ValueTask<IQueryable<T>> ApplySortingAsync<T, TSelector>(IQueryable<T> query,
        Expression<Func<T, TSelector>>? sortingSelector,
        SortingDirection? sortingDirection = null,
        Expression<Func<T, TSelector>>? defaultSortingSelector = null,
        SortingDirection defaultSortingDirection = SortingDirection.Asc,
        CancellationToken token = default)
    {
        sortingDirection = sortingDirection.DefaultIfNotSet(defaultSortingDirection);

        sortingSelector ??= defaultSortingSelector;

        query = query.OptionalOrderBy(sortingSelector, sortingDirection.Value);

        return new(query);
    }

    /// <summary>
    ///     Retrieves a sorting selector from <see cref="SortingConfiguration" />. If not found throws an error.
    /// </summary>
    /// <param name="fieldName">A name of property to sort by.</param>
    /// <param name="sortingConfiguration">A dictionary with allowed sorting parameters. The key is parameter name, the value is property selector.</param>
    /// <typeparam name="T">An underlying type of the query.</typeparam>
    /// <typeparam name="TSelector">A type of sort selector expression.</typeparam>
    /// <returns>A selector for sorting by provided field.</returns>
    /// <exception cref="ArgumentException">A requested sorting parameter was not found.</exception>
    protected virtual Expression<Func<T, TSelector?>> GetSortingSelector<T, TSelector>(string? fieldName,
        IReadOnlyDictionary<string, Expression<Func<T, TSelector?>>>? sortingConfiguration)
    {
        if (fieldName == null) throw new ArgumentNullException(nameof(fieldName));

        if (sortingConfiguration == null) throw new ArgumentNullException(nameof(sortingConfiguration));

        if (sortingConfiguration.ContainsKey(fieldName))
            return sortingConfiguration[fieldName];

        throw ExceptionFactory.NotImplemented(GetSortingOptionNotSupportedErrorMessage(fieldName, sortingConfiguration.Keys));
    }

    /// <summary>
    ///     Returns a message that appears when provided sorting parameter is not supported.
    /// </summary>
    /// <param name="parameterName">A parameter name.</param>
    /// <param name="supportedSortingOptions">A list of supported sorting options.</param>
    /// <returns>The error message.</returns>
    protected virtual string GetSortingOptionNotSupportedErrorMessage(string parameterName, IEnumerable<string> supportedSortingOptions)
    {
        if (supportedSortingOptions == null) throw new ArgumentNullException(nameof(supportedSortingOptions));

        return $"Sorting parameter \"{parameterName}\" is not supported!\n" +
               "Supported parameters: " +
               string.Join(", ", supportedSortingOptions.OrderBy(x => x));
    }

    /// <summary>
    ///     Applies includes, filtering, search, ordering and pagination to provided query and retrieves data.
    /// </summary>
    /// <param name="query">The source query.</param>
    /// <param name="request">The request to be applied.</param>
    /// <param name="token">A cancellation token.</param>
    /// <returns>An instance of <see cref="PagedResult{T}" /> with data of query.</returns>
    protected virtual async Task<PagedResult<TEntity>> GetDataByRepositoryRequestAsync(IQueryable<TEntity> query,
        TQueryParams? request,
        CancellationToken token = default)
    {
        SearchGuard(request);
        SortGuard(request);

        query = await ApplyRepositoryRequestAsync(query, request, token);

        return await ApplyPagination(query, request, token);
    }

    protected virtual async Task<PagedResult<T>> ApplyPagination<T>(IQueryable<T> query,
        TQueryParams? request,
        CancellationToken token)
    {
        return await query.PaginateAsync(request, token);
    }

    protected virtual ValueTask<IQueryable<TEntity>> ApplyAccessRightsPolicy(IQueryable<TEntity> query,
        TAccessRightsPolicyParams accessRightsPolicyParams,
        CancellationToken token = default)
    {
        // override this method if additional access rights policy options are needed.
        return ValueTask.FromResult(query);
    }

    /// <summary>
    ///     Returns a base query to use in all requests within this repository. Disables tracking. Can be overriden to add some logic.
    ///     Calls <see cref="IncludeCommonIncludesAsync" />. Includes all <see cref="OwnedProperties" />.
    /// </summary>
    /// <param name="options">An options for the query.</param>
    /// <param name="token">A cancellation token.</param>
    /// <returns>A query to use in further queries.</returns>
    protected virtual async Task<IQueryable<TEntity>> GetBaseQueryAsync(RepositoryQueryOptions<TAccessRightsPolicyParams>? options = null,
        CancellationToken token = default)
    {
        var query = Set.AsNoTracking();

        // true by default
        if (options?.LoadOwnedProperties ?? true) query = await IncludeOwnedPropertiesAsync(query, token);

        // true by default
        if (options?.LoadCommonIncludes ?? true) query = await IncludeCommonIncludesAsync(query, token);

        // false by default
        if (options?.LoadDetailedViewIncludes ?? false) query = await IncludeDetailedViewIncludesAsync(query, token);

        // false by default
        if (options?.AccessRightsPolicyParams is not null)
            query = await ApplyAccessRightsPolicy(query, options.AccessRightsPolicyParams, token);

        query = await ApplyFilterByTenantIdAsync(query, token);

        return query;
    }

    protected virtual async ValueTask<IQueryable<TEntity>> ApplyFilterByTenantIdAsync(
        IQueryable<TEntity> query,
        CancellationToken token = default)
    {
        if (TenantIdProvider is null || !typeof(ITenant).IsAssignableFrom(typeof(TEntity)))
            return query;

        var tenantId = await TenantIdProvider.GetTenantIdAsync(token);

        var filteredQuery = query.OfType<ITenant>()
            .AsQueryable()
            .Where(x => x.TenantId == tenantId)
            .Cast<TEntity>(); // Cast back to TEntity
        return filteredQuery;
    }

    /// <summary>
    ///     Throws <see cref="NotFoundDomainException" /> if resulting collection is empty.
    /// </summary>
    /// <param name="result">The resulting collection.</param>
    /// <typeparam name="T">A resulting collection type.</typeparam>
    /// <exception cref="NotFoundDomainException">Thrown if collection is empty.</exception>
    protected virtual void EnsureAny<T>(ICollection<T>? result)
    {
        if (result?.Any() != true) throw ExceptionFactory.EntityNotFound<TEntity>();
    }

    protected virtual bool IsEmptyQuery(TQueryParams? request)
    {
        return false;
    }

    private bool IsOwnedPropertiesModifiedAsync<T>(T entity, OwnedPropertiesTreeNode node) where T : class
    {
        var entry = Context.Entry(entity);
        if (entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted) return true;

        foreach (var childOwnedProperty in node.Children.Values)
        {
            var childOwnedPropertyNavigation = entry.Navigation(childOwnedProperty.Name);
            var childEntities = childOwnedPropertyNavigation.GetCollectionOfEntities();

            if (!childEntities.Any()) continue;

            foreach (var childEntity in childEntities)
            {
                var result = IsOwnedPropertiesModifiedAsync(childEntity, childOwnedProperty);
                if (result) return result;
            }
        }

        return false;
    }
}