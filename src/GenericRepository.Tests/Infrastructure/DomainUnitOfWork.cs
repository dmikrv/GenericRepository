using GenericRepository.Contracts;
using GenericRepository.Core.Contracts;
using GenericRepository.Services;
using GenericRepository.Tests.BLL.Contracts.UnitsOfWork;

namespace GenericRepository.Tests.Infrastructure;

public class TestsUnitOfWork : GenericRepositoryUnitOfWorkVal<TestsDbContext, int>, ITestsUnitOfWork
{
    public TestsUnitOfWork(
        TestsDbContext context,
        ICurrentUserIdProvider currentUserIdProvider,
        IEnumerable<IEntityAuditService> entityAuditServices,
        ITenantIdProvider? tenantIdProvider = null)
        : base(context, currentUserIdProvider, entityAuditServices, tenantIdProvider)
    {
    }
}