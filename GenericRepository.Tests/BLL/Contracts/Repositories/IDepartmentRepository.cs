using GenericRepository.Core.Contracts.Repositories;
using GenericRepository.Core.Models.Repositories;
using GenericRepository.Tests.BLL.Contracts.Models.Department;
using GenericRepository.Tests.Infrastructure.Entities;

namespace GenericRepository.Tests.BLL.Contracts.Repositories;

public interface IDepartmentRepository : IEntityRepository<Department, int, IListItem<Guid>, DepartmentQueryParams>
{
}