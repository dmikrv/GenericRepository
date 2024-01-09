using GenericRepository.Core.Common.Auditable.Create;
using GenericRepository.Core.Common.Auditable.Update;

namespace GenericRepository.Core.Common;

public abstract class BaseAuditableEntityVal<TPrimaryKey> : BaseEntity<TPrimaryKey>,
    IAuditableCreated<TPrimaryKey>,
    IAuditableModifiedAt, IAuditableModifiedByVal<TPrimaryKey>
    where TPrimaryKey : struct, IEquatable<TPrimaryKey>
{
    public virtual DateTime CreatedAtUtc { get; set; }
    public virtual TPrimaryKey CreatedByUserId { get; set; } = default!;

    public virtual DateTime? ModifiedAtUtc { get; set; }
    public virtual TPrimaryKey? ModifiedByUserId { get; set; } = default!;
}