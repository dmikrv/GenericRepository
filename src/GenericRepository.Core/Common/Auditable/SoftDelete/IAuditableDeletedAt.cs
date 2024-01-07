namespace GenericRepository.Core.Common.Auditable.SoftDelete;

public interface IAuditableDeletedAt
{
    DateTime? DeletedAtUtc { get; set; }
}