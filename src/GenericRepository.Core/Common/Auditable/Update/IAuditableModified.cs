namespace GenericRepository.Core.Common.Auditable.Update;

/// <summary>
///     Indicates that this entity should store data about latest modification author and timestamp.
/// </summary>
public interface IAuditableModified<TPrimaryKey> : IAuditableModifiedBy<TPrimaryKey>, IAuditableModifiedAt;