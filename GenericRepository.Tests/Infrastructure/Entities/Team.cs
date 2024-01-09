using GenericRepository.Core.Common;

namespace GenericRepository.Tests.Infrastructure.Entities;

public class Team : BaseAuditableEntityVal<int>
{
    public string Name { get; set; } = default!;

    public int ManagerId { get; set; } = default!;

    public Person Manager { get; set; } = default!;

    public List<TeamMember> TeamMembers { get; set; } = default!;
}