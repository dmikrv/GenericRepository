namespace GenericRepository.Core.Common.Auditable.SoftDelete;

public interface IAuditableIsDeleted
{
    public bool IsDeleted { get; set; }
}