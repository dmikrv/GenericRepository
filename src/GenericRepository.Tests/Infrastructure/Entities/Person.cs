﻿using GenericRepository.Core.Common;

namespace GenericRepository.Tests.Infrastructure.Entities;

public class Person : BaseAuditableEntityVal<int>
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;

    public List<Department> DepartmentsUnderManagement { get; set; } = default!;

    public List<Team> TeamsUnderManagement { get; set; } = default!;

    public List<TeamMember> TeamsMembership { get; set; } = default!;
}