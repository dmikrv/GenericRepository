using GenericRepository.Contracts;
using GenericRepository.Core.Contracts;
using Microsoft.EntityFrameworkCore;

namespace GenericRepository.Services;

/// <inheritdoc />
public class GenericRepositoryUnitOfWork<TContext, TUserPrimaryKey> : IUnitOfWork where TContext : DbContext
{
    private readonly TContext _context;
    private readonly ICurrentUserIdProvider _currentUserIdProvider;
    private readonly IEnumerable<IEntityAuditService> _entityAuditServices;

    /// <summary>
    ///     Initializes a new instance of the <see cref="GenericRepositoryUnitOfWork{TContext,TUserPrimaryKey}" /> class.
    /// </summary>
    /// <param name="context">The db context.</param>
    /// <param name="entityAuditServices">Services for handling automatic entity auditing.</param>
    /// <param name="currentUserIdProvider">A service for retrieving information about user who performs operation.</param>
    public GenericRepositoryUnitOfWork(TContext context, ICurrentUserIdProvider currentUserIdProvider,
        IEnumerable<IEntityAuditService> entityAuditServices)
    {
        _context = context;
        _currentUserIdProvider = currentUserIdProvider;
        _entityAuditServices = entityAuditServices;
    }

    /// <inheritdoc />
    public virtual async Task SaveChangesAsync(CancellationToken token = default)
    {
        var userId = await _currentUserIdProvider.GetCurrentUserIdAsync<TUserPrimaryKey>(token);
        foreach (var entityAuditService in _entityAuditServices)
            await entityAuditService.ApplyAuditRules(_context, userId, token);
        await _context.SaveChangesAsync(token);
        DetachAll();
    }

    /// <inheritdoc />
    public virtual void Rollback()
    {
        DetachAll();
    }

    private void DetachAll()
    {
        _context.ChangeTracker.Clear();
    }
}