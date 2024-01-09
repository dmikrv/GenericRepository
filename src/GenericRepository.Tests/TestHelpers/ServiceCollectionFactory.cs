using AutoMapper;
using GenericRepository.Core.Contracts;
using GenericRepository.Extensions.ServiceProviders;
using GenericRepository.Tests.BLL.Contracts.Repositories;
using GenericRepository.Tests.BLL.Contracts.UnitsOfWork;
using GenericRepository.Tests.Infrastructure;
using GenericRepository.Tests.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace GenericRepository.Tests.TestHelpers;

public static class ServiceCollectionFactory
{
    public static ServiceCollection CreateServiceCollection()
    {
        var services = new ServiceCollection();

        // Common services

        // Repositories
        services.AddSingleton<ICompanyRepository, CompanyRepository>();
        services.AddSingleton<IDepartmentRepository, DepartmentRepository>();


        services.AddSingleton(DatabaseInitializer.GetMemoryContext());

        // Common services
        services.AddGenericRepositoryCommonDependencies();
        services.AddDefaultEntityAuditService(ServiceLifetime.Singleton);
        // services.AddSingleton<IAggregateUnitOfWork, AggregateUnitOfWork>();
        services.AddUnitOfWork<ITestsUnitOfWork, TestsUnitOfWork>(ServiceLifetime.Singleton);
        services.AddSingleton(TimeProvider.System);
        services.AddRepositoryExceptionFactory(ServiceLifetime.Singleton);

        services.AddLogging();

        var iCurrentUserServiceMock = new Mock<ICurrentUserIdProvider>();

        iCurrentUserServiceMock.Setup(x => x.GetCurrentUserIdAsync<int>(It.IsAny<CancellationToken>()))
            .ReturnsAsync(456);

        services.AddSingleton(iCurrentUserServiceMock.Object);
        services.AddSingleton(iCurrentUserServiceMock);

        // Mapper
        services.AddSingleton<IMapper>(new Mapper(new MapperConfiguration(_ => { }))); // TODO

        return services;
    }
}