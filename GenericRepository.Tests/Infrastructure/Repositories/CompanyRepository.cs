using System.Linq.Expressions;
using GenericRepository.Core.Enums;
using GenericRepository.Core.Models.Repositories;
using GenericRepository.Extensions;
using GenericRepository.Repositories;
using GenericRepository.Tests.BLL.Contracts.Models.Company;
using GenericRepository.Tests.BLL.Contracts.Repositories;
using GenericRepository.Tests.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace GenericRepository.Tests.Infrastructure.Repositories;

public class CompanyRepository : UnitOfWorkEntityRepositoryBase<Company, int, TestsDbContext, IListItem<Guid>, CompanyQueryParams>,
    ICompanyRepository
{
    public CompanyRepository(TestsDbContext context, RepositoryCommonDependencies dependencies)
        : base(context, dependencies)
    {
    }

    protected override Func<string, Expression<Func<Company, bool>>> SearchPredicate =>
        query => x => x.Name.Contains(query);

    protected override Func<string, Expression<Func<Company, bool>>> TypeaheadPredicate =>
        query => x => x.Name.StartsWith(query);

    protected override SortingDirection DefaultSortingDirection => SortingDirection.Desc;

    protected override Expression<Func<Company, object?>> DefaultSortingSelector { get; } = x => x.CreatedAtUtc;

    protected override IReadOnlyDictionary<string, Expression<Func<Company, object?>>> SortingConfiguration { get; } =
        new Dictionary<string, Expression<Func<Company, object?>>>(StringComparer.OrdinalIgnoreCase)
        {
            [nameof(Company.Id)] = x => x.Id,
            [nameof(Company.Name)] = x => x.Name,
            [nameof(Company.CreatedByUserId)] = x => x.CreatedByUserId,
            [nameof(Company.CreatedAtUtc)] = x => x.CreatedAtUtc,
            [nameof(Company.ModifiedByUserId)] = x => x.ModifiedByUserId,
            [nameof(Company.ModifiedAtUtc)] = x => x.ModifiedAtUtc
        };

    protected override async Task<IQueryable<Company>> IncludeOwnedPropertiesAsync(IQueryable<Company> query,
        CancellationToken token = default)
    {
        await base.IncludeOwnedPropertiesAsync(query, token);

        query = query
            .Include(x => x.Departments)
            .ThenInclude(x => x.HeadOfDepartment)
            //
            .Include(x => x.Departments)
            .ThenInclude(x => x.Managers)
            .ThenInclude(x => x.TeamsUnderManagement)
            .ThenInclude(x => x.TeamMembers);
        // .ThenInclude(x => x.Member);

        return query;
    }

    protected override async Task<IQueryable<Company>> ApplyFilteringAsync(IQueryable<Company> query,
        CompanyQueryParams? request,
        CancellationToken token = default)
    {
        if (request is null)
            return query;

        query = await base.ApplyFilteringAsync(query, request, token);

        query = query.ApplyStringPropertyFilter(x => x.Name, request.Filters?.Name);

        return query;
    }
}