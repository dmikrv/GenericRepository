using System.Linq.Expressions;
using GenericRepository.Core.Enums;
using GenericRepository.Core.Models.Repositories;
using GenericRepository.Extensions;
using GenericRepository.Repositories;
using GenericRepository.Tests.BLL.Contracts.Models.Department;
using GenericRepository.Tests.BLL.Contracts.Repositories;
using GenericRepository.Tests.Infrastructure.Entities;

namespace GenericRepository.Tests.Infrastructure.Repositories;

public class DepartmentRepository :
    UnitOfWorkEntityRepositoryBase<Department, int, TestsDbContext, IListItem<Guid>, DepartmentQueryParams>, IDepartmentRepository
{
    public DepartmentRepository(TestsDbContext context, RepositoryCommonDependencies dependencies)
        : base(context, dependencies)
    {
    }

    protected override Func<string, Expression<Func<Department, bool>>> SearchPredicate =>
        query => x => x.Name.Contains(query);

    protected override Func<string, Expression<Func<Department, bool>>> TypeaheadPredicate =>
        query => x => x.Name.StartsWith(query);

    protected override SortingDirection DefaultSortingDirection => SortingDirection.Desc;

    protected override Expression<Func<Department, object?>> DefaultSortingSelector { get; } = x => x.CreatedAtUtc;

    protected override IReadOnlyDictionary<string, Expression<Func<Department, object?>>> SortingConfiguration { get; } =
        new Dictionary<string, Expression<Func<Department, object?>>>(StringComparer.OrdinalIgnoreCase)
        {
            [nameof(Department.Id)] = x => x.Id,
            [nameof(Department.Name)] = x => x.Name,
            [nameof(Company.CreatedByUserId)] = x => x.CreatedByUserId,
            [nameof(Company.CreatedAtUtc)] = x => x.CreatedAtUtc,
            [nameof(Company.ModifiedByUserId)] = x => x.ModifiedByUserId,
            [nameof(Company.ModifiedAtUtc)] = x => x.ModifiedAtUtc
        };

    protected override async Task<IQueryable<Department>> ApplyFilteringAsync(IQueryable<Department> query,
        DepartmentQueryParams? request,
        CancellationToken token = default)
    {
        if (request is null)
            return query;

        query = await base.ApplyFilteringAsync(query, request, token);

        query = query.ApplyStringPropertyFilter(x => x.Name, request.Filters?.Name);

        return query;
    }
}