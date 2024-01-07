namespace GenericRepository.Core.Common.Auditable.SoftDelete;

public interface IAuditableDeleted<TPrimaryKey> : IAuditableIsDeleted, IAuditableDeletedAt, IAuditableDeletedBy<TPrimaryKey>;