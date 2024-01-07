namespace GenericRepository.Core.Common.Auditable.Create;

/// <summary>
///     Indicates that this entity should store data about it's author.
/// </summary>
public interface IAuditableCreatedBy<TPrimaryKey>
{
    /// <summary>
    ///     Gets or sets entity author id.
    /// </summary>
    TPrimaryKey CreatedByUserId { get; set; }
}