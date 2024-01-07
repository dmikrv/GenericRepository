namespace GenericRepository.Core.Common.Auditable.Create;

/// <summary>
///     Indicates that this entity should store data about the author and when it was created.
/// </summary>
public interface IAuditableCreated<TPrimaryKey> : IAuditableCreatedBy<TPrimaryKey>, IAuditableCreatedAt;