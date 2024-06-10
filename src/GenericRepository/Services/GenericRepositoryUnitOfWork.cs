using GenericRepository.Contracts;
using GenericRepository.Core.Contracts;
using Microsoft.EntityFrameworkCore;

namespace GenericRepository.Services;

/// <inheritdoc />
public class GenericRepositoryUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
{
    protected readonly TContext Context;
    protected readonly ICurrentUserIdProvider CurrentUserIdProvider;
    protected readonly IReadOnlyCollection<IEntityAuditService> EntityAuditServices;
    protected readonly ITenantIdProvider? TenantIdProvider;

    /// <summary>
    ///     Initializes a new instance of the <see cref="GenericRepositoryUnitOfWork{TContext}" /> class.
    /// </summary>
    /// <param name="context">The db context.</param>
    /// <param name="tenantIdProvider"></param>
    /// <param name="entityAuditServices">Services for handling automatic entity auditing.</param>
    /// <param name="currentUserIdProvider">A service for retrieving information about user who performs operation.</param>
    public GenericRepositoryUnitOfWork(
        TContext context,
        ICurrentUserIdProvider currentUserIdProvider,
        ITenantIdProvider? tenantIdProvider,
        IEnumerable<IEntityAuditService> entityAuditServices)
    {
        Context = context;
        CurrentUserIdProvider = currentUserIdProvider;
        TenantIdProvider = tenantIdProvider;
        EntityAuditServices = entityAuditServices.ToArray();
    }

    /// <inheritdoc />
    public virtual async Task SaveChangesAsync(CancellationToken token = default)
    {
        await Context.SaveChangesAsync(token);
        Rollback();
    }

    /// <inheritdoc />
    public virtual void Rollback()
    {
        DetachAll();
    }

    private void DetachAll()
    {
        Context.ChangeTracker.Clear();
    }
}