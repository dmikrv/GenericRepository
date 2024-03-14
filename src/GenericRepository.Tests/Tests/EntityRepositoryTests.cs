using FluentAssertions;
using GenericRepository.Core.Exceptions;
using GenericRepository.Tests.BLL.Contracts.Models.Department;
using GenericRepository.Tests.BLL.Contracts.Repositories;
using GenericRepository.Tests.TestHelpers;
using Microsoft.Extensions.DependencyInjection;

namespace GenericRepository.Tests.Tests;

[TestFixture]
public class EntityRepositoryTests
{
    [SetUp]
    public void SetUp()
    {
        DatabaseInitializer.InitDatabaseWithExampleData();

        _serviceCollection = ServiceCollectionFactory.CreateServiceCollection();
    }

    private ServiceCollection _serviceCollection = null!;

    // [Test]
    // public async Task CreateAsync_ValidEntity_ReturnsCreateEntityResult()
    // {
    //     // Arrange
    // var sp = _serviceCollection.BuildServiceProvider();
    // var repository = sp.GetRequiredService<ICompanyRepository>();
    // var uow = sp.GetRequiredService<IUnitOfWork>();
    //
    //     var entity = DatabaseInitializer.GetMicrosoftCompany();
    //
    //     // Act
    //     var result = await repository.CreateAsync(entity);
    //     await uow.SaveChangesAsync();
    //
    //     // Assert
    //     using (new AssertionScope())
    //     {
    //         result.Should().NotBeNull();
    //         result.Entity.Should().NotBeNull();
    //         result.ValueAccessor.Value.Should().BeGreaterThan(0);
    //         result.Entity.Should().BeEquivalentTo(entity);
    //     }
    // }

    // [Test]
    // public async Task CreateAsync_ValidEntity_CreatesDependentEntities()
    // {
    //     // Arrange
    //     var sp = _serviceCollection.BuildServiceProvider();
    //     var repository = sp.GetRequiredService<ICompanyRepository>();
    //     var uow = sp.GetRequiredService<IUnitOfWork>();
    //
    //     var entity = DatabaseInitializer.GetMicrosoftCompany();
    //
    //     // Act
    //     var result = await repository.CreateAsync(entity);
    //     await uow.SaveChangesAsync();
    //
    //     // Assert
    //     using (new AssertionScope())
    //     {
    //         result.Should().NotBeNull();
    //         result.Entity.Should().NotBeNull();
    //         result.ValueAccessor.Value.Should().BeGreaterThan(0);
    //         result.Entity.Should().BeEquivalentTo(entity);
    //     }
    // }

    // [Test]
    // public async Task GetByIdAsync_ExistingEntity_ReturnsEntity()
    // {
    //     // Arrange
    //     var sp = _serviceCollection.BuildServiceProvider();
    //     var repository = sp.GetRequiredService<ICompanyRepository>();
    //     var uow = sp.GetRequiredService<IUnitOfWork>();
    //
    //     var entity = DatabaseInitializer.GetMicrosoftCompany();
    //     var primaryKey = await repository.CreateAsync(entity);
    //     await uow.SaveChangesAsync();
    //
    //     // Act
    //     var result = await repository.GetByIdAsync(primaryKey.ValueAccessor.Value);
    //
    //     // Assert
    //     using (new AssertionScope())
    //     {
    //         result.Should().NotBeNull();
    //         entity.Should().BeEquivalentTo(result);
    //     }
    // }

    [Test]
    public async Task GetByIdAsync_NonexistentEntity_ReturnsNull()
    {
        // Arrange
        var sp = _serviceCollection.BuildServiceProvider();
        var repository = sp.GetRequiredService<ICompanyRepository>();

        const int primaryKey = 123123123;

        // Act
        var result = await repository.GetByIdAsync(primaryKey);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task GetByIdAsync_NonexistentEntity_ThrowsWhenRequired()
    {
        // Arrange
        var sp = _serviceCollection.BuildServiceProvider();
        var repository = sp.GetRequiredService<ICompanyRepository>();

        const int primaryKey = 123123123;

        // Act
        var act = async () => { await repository.GetByIdAsync(primaryKey, new() { Required = true }); };

        // Assert
        await act.Should().ThrowAsync<NotFoundDomainException>();
    }
    
    [Test]
    public async Task GetAsync_DeletedFilter_ReturnsDeletedEntities()
    {
        // Arrange
        var sp = _serviceCollection.BuildServiceProvider();
        var repository = sp.GetRequiredService<IDepartmentRepository>();

        // Act
        var all = await repository.GetAsync(new DepartmentQueryParams{PageSize = int.MaxValue});
        var deleted = await repository.GetAsync(new DepartmentQueryParams{PageSize = int.MaxValue, IsDeleted = true});
        var notDeleted = await repository.GetAsync(new DepartmentQueryParams{PageSize = int.MaxValue, IsDeleted = false});

        // Assert
        all.ItemsCount.Should().Be(9);
        deleted.ItemsCount.Should().Be(2);
        notDeleted.ItemsCount.Should().Be(7);
    }

    // ----------------------------- Helper methods -----------------------------
}