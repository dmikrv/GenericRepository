namespace GenericRepository.Core.Common.Auditable.Update;

/// <summary>
///     Gets or sets value of last entity modification author.
/// </summary>
public interface IAuditableModifiedByRef<TPrimaryKey> where TPrimaryKey : class
{
    /// <summary>
    ///     Gets or sets entity editor id.
    /// </summary>
    TPrimaryKey? ModifiedByUserId { get; set; }
}