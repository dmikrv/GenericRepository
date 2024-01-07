namespace GenericRepository.Core.Common.Auditable.SoftDelete;

public interface IAuditableDeletedBy<TPrimaryKey>
{
    /// <summary>
    ///     Gets or sets entity editor id.
    /// </summary>
    TPrimaryKey? DeletedByUserId { get; set; }
}