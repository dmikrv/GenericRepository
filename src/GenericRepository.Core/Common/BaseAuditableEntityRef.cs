using GenericRepository.Core.Common.Auditable.Create;
using GenericRepository.Core.Common.Auditable.Update;

namespace GenericRepository.Core.Common;

public abstract class BaseAuditableEntityRef<TPrimaryKey> : BaseEntity<TPrimaryKey>,
    IAuditableCreated<TPrimaryKey>,
    IAuditableModifiedAt, IAuditableModifiedByRef<TPrimaryKey>
    where TPrimaryKey : class, IEquatable<TPrimaryKey>
{
    public virtual DateTime CreatedAtUtc { get; set; }
    public virtual TPrimaryKey CreatedByUserId { get; set; } = default!;

    public virtual DateTime? ModifiedAtUtc { get; set; }
    public virtual TPrimaryKey? ModifiedByUserId { get; set; } = default!;
}