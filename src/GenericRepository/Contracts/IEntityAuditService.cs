using Microsoft.EntityFrameworkCore;

namespace GenericRepository.Contracts;

/// <summary>
///     Handles automatic entity auditing. Affects entities marked with interfaces:
///     <see cref="IAuditable" />, <see cref="IAuditableCreated" />, <see cref="IAuditableCreatedAt" />, <see cref="IAuditableCreatedBy" />,
///     <see cref="IAuditableModified" />, <see cref="IAuditableModifiedAt" />, <see cref="IAuditableModifiedBy" />.
/// </summary>
public interface IEntityAuditService
{
    /// <summary>
    ///     Applies rules to entities tracked in the <paramref name="context" /> parameter.
    /// </summary>
    /// <param name="context">The context with tracked entities.</param>
    /// <param name="userId">A user who performs this operation.</param>
    /// <param name="token"></param>
    Task ApplyAuditRules<TUserPrimaryKey>(DbContext context, TUserPrimaryKey userId, CancellationToken token = default);

    Task ApplyAuditRulesByRef<TUserPrimaryKey>(DbContext context, TUserPrimaryKey userId, CancellationToken token = default)
        where TUserPrimaryKey : class
    {
        return Task.CompletedTask;
    }

    Task ApplyAuditRulesByVal<TUserPrimaryKey>(DbContext context, TUserPrimaryKey userId, CancellationToken token = default)
        where TUserPrimaryKey : struct
    {
        return Task.CompletedTask;
    }
}