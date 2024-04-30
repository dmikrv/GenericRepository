using GenericRepository.Core.Common.Auditable.Create;
using GenericRepository.Core.Common.Auditable.Update;

namespace GenericRepository.Core.Common;

public abstract class BaseAuditableVersionedEntityRef<TPrimaryKey> : BaseAuditableVersionedEntityRef<TPrimaryKey, TPrimaryKey>
    where TPrimaryKey : class, IEquatable<TPrimaryKey>;

public abstract class BaseAuditableVersionedEntityRef<TPrimaryKey, TUserPrimaryKey> : BaseVersionedEntity<TPrimaryKey>,
    IAuditableCreated<TUserPrimaryKey>, IAuditableModifiedAt, IAuditableModifiedByRef<TUserPrimaryKey>
    where TPrimaryKey : IEquatable<TPrimaryKey>
    where TUserPrimaryKey : class
{
    public virtual DateTime CreatedAtUtc { get; set; }
    public virtual TUserPrimaryKey CreatedByUserId { get; set; } = default!;

    public virtual DateTime? ModifiedAtUtc { get; set; }
    public virtual TUserPrimaryKey? ModifiedByUserId { get; set; } = default!;
}