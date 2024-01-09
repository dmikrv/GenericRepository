using GenericRepository.Core.Common;

namespace GenericRepository.Tests.Infrastructure.Entities;

public class Department : BaseAuditableEntityVal<int>
{
    public string Name { get; set; } = default!;

    public int CompanyId { get; set; } = default!;

    public Company Company { get; set; } = default!;

    public int HeadOfDepartmentId { get; set; } = default!;

    public Person HeadOfDepartment { get; set; } = default!;

    public List<Person> Managers { get; set; } = default!;
}