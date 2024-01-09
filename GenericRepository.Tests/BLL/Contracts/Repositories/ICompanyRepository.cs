using GenericRepository.Core.Contracts.Repositories;
using GenericRepository.Core.Models.Repositories;
using GenericRepository.Tests.BLL.Contracts.Models.Company;
using GenericRepository.Tests.Infrastructure.Entities;

namespace GenericRepository.Tests.BLL.Contracts.Repositories;

public interface ICompanyRepository : IEntityRepository<Company, int, IListItem<Guid>, CompanyQueryParams>
{
}