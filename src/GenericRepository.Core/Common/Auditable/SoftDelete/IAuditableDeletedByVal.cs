namespace GenericRepository.Core.Common.Auditable.SoftDelete;

public interface IAuditableDeletedByVal<TPrimaryKey> where TPrimaryKey : struct
{
    /// <summary>
    ///     Gets or sets entity editor id.
    /// </summary>
    TPrimaryKey? DeletedByUserId { get; set; }
}