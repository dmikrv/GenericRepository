using GenericRepository.Core.Common;
using GenericRepository.Core.Common.Auditable.SoftDelete;

namespace GenericRepository.Tests.Infrastructure.Entities;

public class Department : BaseAuditableEntityVal<int>, IAuditableIsDeleted
{
    public string Name { get; set; } = default!;

    public int CompanyId { get; set; } = default!;

    public Company Company { get; set; } = default!;

    public int HeadOfDepartmentId { get; set; } = default!;

    public Person HeadOfDepartment { get; set; } = default!;

    public List<Person> Managers { get; set; } = default!;
    
    public bool IsDeleted { get; set; }
}