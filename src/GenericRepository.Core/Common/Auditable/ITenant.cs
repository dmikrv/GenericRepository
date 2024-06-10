namespace GenericRepository.Core.Common.Auditable;

public interface ITenant
{
    public Guid TenantId { get; set; }
}