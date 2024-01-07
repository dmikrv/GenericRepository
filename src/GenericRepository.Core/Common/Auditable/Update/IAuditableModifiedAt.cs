namespace GenericRepository.Core.Common.Auditable.Update;

/// <summary>
///     Gets or sets last entity modification timestamp.
/// </summary>
public interface IAuditableModifiedAt
{
    /// <summary>
    ///     Gets or sets last entity modification timestamp.
    /// </summary>
    DateTime? ModifiedAtUtc { get; set; }
}