using GenericRepository.Contracts;
using GenericRepository.Core.Contracts;
using Microsoft.EntityFrameworkCore;

namespace GenericRepository.Services;

/// <inheritdoc />
public class GenericRepositoryUnitOfWork<TContext, TUserPrimaryKey> : IUnitOfWork where TContext : DbContext
{
    protected readonly TContext Context;
    protected readonly ICurrentUserIdProvider CurrentUserIdProvider;
    protected readonly IReadOnlyCollection<IEntityAuditService> EntityAuditServices;

    /// <summary>
    ///     Initializes a new instance of the <see cref="GenericRepositoryUnitOfWork{TContext,TUserPrimaryKey}" /> class.
    /// </summary>
    /// <param name="context">The db context.</param>
    /// <param name="entityAuditServices">Services for handling automatic entity auditing.</param>
    /// <param name="currentUserIdProvider">A service for retrieving information about user who performs operation.</param>
    public GenericRepositoryUnitOfWork(TContext context, ICurrentUserIdProvider currentUserIdProvider,
        IEnumerable<IEntityAuditService> entityAuditServices)
    {
        Context = context;
        CurrentUserIdProvider = currentUserIdProvider;
        EntityAuditServices = entityAuditServices.ToArray();
    }

    /// <inheritdoc />
    public virtual async Task SaveChangesAsync(CancellationToken token = default)
    {
        var userId = await CurrentUserIdProvider.GetCurrentUserIdAsync<TUserPrimaryKey>(token);
        foreach (var entityAuditService in EntityAuditServices) await entityAuditService.ApplyAuditRules(Context, userId, token);
        await Context.SaveChangesAsync(token);
        DetachAll();
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