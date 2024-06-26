using GenericRepository.Contracts;
using GenericRepository.Core.Common.Auditable;
using GenericRepository.Core.Common.Auditable.Create;
using GenericRepository.Core.Common.Auditable.SoftDelete;
using GenericRepository.Core.Common.Auditable.Update;
using GenericRepository.Core.Common.Auditable.Versioned;
using Microsoft.EntityFrameworkCore;

namespace GenericRepository.Services;

/// <inheritdoc />
public class EntityAuditService : IEntityAuditService
{
    protected readonly DateTime DateTimeNow;

    /// <summary>
    ///     Initializes a new instance of the <see cref="EntityAuditService" /> class.
    /// </summary>
    /// <param name="timeProvider">The TimeProvider service.</param>
    public EntityAuditService(TimeProvider timeProvider)
    {
        DateTimeNow = timeProvider.GetUtcNow().UtcDateTime;
    }

    /// <inheritdoc />
    public Task ApplySharedAuditRules<TUserPrimaryKey>(
        DbContext context,
        TUserPrimaryKey userId,
        Guid? tenantId,
        CancellationToken token = default)
    {
        foreach (var entry in context.ChangeTracker.Entries()
                     .Where(x => x.State is EntityState.Modified or EntityState.Added or EntityState.Deleted))
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity is IAuditableCreatedAt autoAuditCreatedAt)
                    autoAuditCreatedAt.CreatedAtUtc = DateTimeNow;

                if (entry.Entity is IAuditableCreatedBy<TUserPrimaryKey> autoAuditCreatedBy)
                    autoAuditCreatedBy.CreatedByUserId = userId;

                if (tenantId is not null && entry.Entity is ITenant tenantEntity)
                    tenantEntity.TenantId = tenantId.Value;
            }

            if (entry.Entity is IAuditableModifiedAt autoAuditUpdatedAt &&
                (entry.State == EntityState.Modified || entry.Entity is not IAuditableCreatedAt))
                autoAuditUpdatedAt.ModifiedAtUtc = DateTimeNow;

            if (entry is { State: EntityState.Deleted, Entity: IAuditableIsDeleted deletedEntity })
            {
                entry.State = EntityState.Modified;
                deletedEntity.IsDeleted = true;

                if (entry.Entity is IAuditableDeletedAt autoAuditDeletedAt) autoAuditDeletedAt.DeletedAtUtc = DateTimeNow;
            }

            if (entry is { State: EntityState.Modified, Entity: IVersionedEntity versionedEntity })
                entry.OriginalValues[nameof(IVersionedEntity.RowVersion)] = versionedEntity.RowVersion;
        }

        return Task.CompletedTask;
    }

    public Task ApplyAuditRulesByRef<TUserPrimaryKey>(
        DbContext context,
        TUserPrimaryKey userId,
        Guid? tenantId,
        CancellationToken token = default)
        where TUserPrimaryKey : class
    {
        ApplySharedAuditRules(context, userId, tenantId, token);

        foreach (var entry in context.ChangeTracker.Entries()
                     .Where(x => x.State is EntityState.Modified or EntityState.Added))
        {
            if (entry.Entity is IAuditableModifiedByRef<TUserPrimaryKey> autoAuditUpdatedBy &&
                (entry.State == EntityState.Modified || entry.Entity is not IAuditableCreatedBy<TUserPrimaryKey>))
                autoAuditUpdatedBy.ModifiedByUserId = userId;

            // method ApplySharedAuditRules changes EntityState to Modified 
            if (entry is
                {
                    State: EntityState.Modified,
                    Entity: IAuditableIsDeleted,
                    Entity: IAuditableDeletedByRef<TUserPrimaryKey> autoAuditDeletedBy
                })
                autoAuditDeletedBy.DeletedByUserId = userId;
        }

        return Task.CompletedTask;
    }

    public Task ApplyAuditRulesByVal<TUserPrimaryKey>(
        DbContext context,
        TUserPrimaryKey userId,
        Guid? tenantId,
        CancellationToken token = default)
        where TUserPrimaryKey : struct
    {
        ApplySharedAuditRules(context, userId, tenantId, token);

        foreach (var entry in context.ChangeTracker.Entries()
                     .Where(x => x.State is EntityState.Modified or EntityState.Added))
        {
            if (entry.Entity is IAuditableModifiedByVal<TUserPrimaryKey> autoAuditUpdatedBy &&
                (entry.State == EntityState.Modified || entry.Entity is not IAuditableCreatedBy<TUserPrimaryKey>))
                autoAuditUpdatedBy.ModifiedByUserId = userId;

            // method ApplySharedAuditRules changes EntityState to Modified 
            if (entry is
                {
                    State: EntityState.Modified,
                    Entity: IAuditableIsDeleted,
                    Entity: IAuditableDeletedByVal<TUserPrimaryKey> autoAuditDeletedBy
                })
                autoAuditDeletedBy.DeletedByUserId = userId;
        }

        return Task.CompletedTask;
    }
}