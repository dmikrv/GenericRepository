namespace GenericRepository.Core.Common.Auditable.SoftDelete;

public interface IAuditableDeletedByRef<TPrimaryKey> where TPrimaryKey : class
{
    /// <summary>
    ///     Gets or sets entity editor id.
    /// </summary>
    TPrimaryKey? DeletedByUserId { get; set; }
}