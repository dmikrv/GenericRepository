using GenericRepository.Core.Common.Auditable.Create;
using GenericRepository.Core.Common.Auditable.Update;
using Microsoft.EntityFrameworkCore;

namespace GenericRepository.Contracts;

/// <summary>
///     Handles automatic entity auditing. Affects entities marked with interfaces:
///     <see cref="IAuditable" />, <see cref="IAuditableCreated{TPrimaryKey}" />, <see cref="IAuditableCreatedAt" />,
///     <see cref="IAuditableCreatedBy" />,
///     <see cref="IAuditableModified" />, <see cref="IAuditableModifiedAt" />, <see cref="IAuditableModifiedBy" />.
/// </summary>
public interface IEntityAuditService
{
    /// <summary>
    ///     Applies rules to entities tracked in the <paramref name="context" /> parameter.
    /// </summary>
    /// <param name="context">The context with tracked entities.</param>
    /// <param name="userId">A user who performs this operation.</param>
    /// <param name="tenantId"></param>
    /// <param name="token"></param>
    Task ApplyAuditRulesByRef<TUserPrimaryKey>(
        DbContext context,
        TUserPrimaryKey userId,
        Guid? tenantId,
        CancellationToken token = default)
        where TUserPrimaryKey : class
    {
        return ApplySharedAuditRules(context, userId, tenantId, token);
    }

    /// <summary>
    ///     Applies rules to entities tracked in the <paramref name="context" /> parameter.
    /// </summary>
    /// <param name="context">The context with tracked entities.</param>
    /// <param name="userId">A user who performs this operation.</param>
    /// <param name="tenantId"></param>
    /// <param name="token"></param>
    Task ApplyAuditRulesByVal<TUserPrimaryKey>(
        DbContext context,
        TUserPrimaryKey userId,
        Guid? tenantId,
        CancellationToken token = default)
        where TUserPrimaryKey : struct
    {
        return ApplySharedAuditRules(context, userId, tenantId, token);
    }

    protected Task ApplySharedAuditRules<TUserPrimaryKey>(
        DbContext context,
        TUserPrimaryKey userId,
        Guid? tenantId,
        CancellationToken token = default)
    {
        return Task.CompletedTask;
    }
}