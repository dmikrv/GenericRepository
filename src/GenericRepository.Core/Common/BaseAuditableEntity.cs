namespace GenericRepository.Core.Common;

// public abstract class BaseAuditableEntity<TPrimaryKey> : BaseEntity<TPrimaryKey>, 
//     IAuditableCreated<TPrimaryKey>, IAuditableModified<TPrimaryKey>
//     where TPrimaryKey : IEquatable<TPrimaryKey>
// {
//     public virtual DateTime CreatedAtUtc { get; set; }
//     public virtual TPrimaryKey CreatedByUserId { get; set; } = default!;
//
//     public virtual DateTime? ModifiedAtUtc { get; set; }
//     public virtual TPrimaryKey? ModifiedByUserId { get; set; } = default!;
// }