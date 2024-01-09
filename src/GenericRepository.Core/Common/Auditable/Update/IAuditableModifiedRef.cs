namespace GenericRepository.Core.Common.Auditable.Update;

/// <summary>
///     Indicates that this entity should store data about latest modification author and timestamp.
/// </summary>
public interface IAuditableModifiedRef<TPrimaryKey> : IAuditableModifiedByRef<TPrimaryKey>, IAuditableModifiedAt where TPrimaryKey : class;