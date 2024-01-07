namespace GenericRepository.Core.Common.Auditable.Create;

/// <summary>
///     Indicates that this entity should store data about when it was created.
/// </summary>
public interface IAuditableCreatedAt
{
    /// <summary>
    ///     Gets or sets value of entity creation timestamp.
    /// </summary>
    DateTime CreatedAtUtc { get; set; }
}