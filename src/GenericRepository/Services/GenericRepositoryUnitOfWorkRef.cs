﻿using GenericRepository.Contracts;
using GenericRepository.Core.Contracts;
using Microsoft.EntityFrameworkCore;

namespace GenericRepository.Services;

/// <inheritdoc />
public class GenericRepositoryUnitOfWorkRef<TContext, TUserPrimaryKey> : GenericRepositoryUnitOfWork<TContext>
    where TContext : DbContext where TUserPrimaryKey : class
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="GenericRepositoryUnitOfWorkRef{TContext,TUserPrimaryKey}" /> class.
    /// </summary>
    /// <param name="context">The db context.</param>
    /// <param name="entityAuditServices">Services for handling automatic entity auditing.</param>
    /// <param name="currentUserIdProvider">A service for retrieving information about user who performs operation.</param>
    public GenericRepositoryUnitOfWorkRef(TContext context, ICurrentUserIdProvider currentUserIdProvider,
        IEnumerable<IEntityAuditService> entityAuditServices) : base(context, currentUserIdProvider, entityAuditServices)
    {
    }

    /// <inheritdoc />
    public override async Task SaveChangesAsync(CancellationToken token = default)
    {
        var userId = await CurrentUserIdProvider.GetCurrentUserIdAsync<TUserPrimaryKey>(token);
        foreach (var entityAuditService in EntityAuditServices) await entityAuditService.ApplyAuditRulesByRef(Context, userId, token);
        await Context.SaveChangesAsync(token);
        Rollback();
    }
}