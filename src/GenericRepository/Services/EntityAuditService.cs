using GenericRepository.Contracts;
using GenericRepository.Core.Common.Auditable.Create;
using GenericRepository.Core.Common.Auditable.SoftDelete;
using GenericRepository.Core.Common.Auditable.Update;
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
    public Task ApplyAuditRules<TUserPrimaryKey>(DbContext context, TUserPrimaryKey userId, CancellationToken token = default)
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
            }

            if (entry.Entity is IAuditableModifiedAt autoAuditUpdatedAt &&
                (entry.State == EntityState.Modified || entry.Entity is not IAuditableCreatedAt))
                autoAuditUpdatedAt.ModifiedAtUtc = DateTimeNow;

            if (entry is { State: EntityState.Deleted, Entity: IAuditableIsDeleted deletedEntity })
            {
                entry.State = EntityState.Modified;
                deletedEntity.IsDeleted = true;

                if (entry.Entity is IAuditableDeletedAt autoAuditDeletedAt) autoAuditDeletedAt.DeletedAtUtc = DateTimeNow;

                if (entry.Entity is IAuditableDeletedBy<TUserPrimaryKey> autoAuditDeletedBy)
                    autoAuditDeletedBy.DeletedByUserId = userId;
            }
        }

        return Task.CompletedTask;
    }

    public Task ApplyAuditRulesByRef<TUserPrimaryKey>(DbContext context, TUserPrimaryKey userId, CancellationToken token = default)
        where TUserPrimaryKey : class
    {
        foreach (var entry in context.ChangeTracker.Entries()
                     .Where(x => x.State is EntityState.Modified or EntityState.Added or EntityState.Deleted))
            if (entry.Entity is IAuditableModifiedByRef<TUserPrimaryKey> autoAuditUpdatedBy &&
                (entry.State == EntityState.Modified || entry.Entity is not IAuditableCreatedBy<TUserPrimaryKey>))
                autoAuditUpdatedBy.ModifiedByUserId = userId;

        return Task.CompletedTask;
    }

    public Task ApplyAuditRulesByVal<TUserPrimaryKey>(DbContext context, TUserPrimaryKey userId, CancellationToken token = default)
        where TUserPrimaryKey : struct
    {
        foreach (var entry in context.ChangeTracker.Entries()
                     .Where(x => x.State is EntityState.Modified or EntityState.Added or EntityState.Deleted))
            if (entry.Entity is IAuditableModifiedByVal<TUserPrimaryKey> autoAuditUpdatedBy &&
                (entry.State == EntityState.Modified || entry.Entity is not IAuditableCreatedBy<TUserPrimaryKey>))
                autoAuditUpdatedBy.ModifiedByUserId = userId;

        return Task.CompletedTask;
    }
}