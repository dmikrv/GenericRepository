using FluentAssertions;
using FluentAssertions.Execution;
using GenericRepository.OwnedPropertiesTree;
using GenericRepository.Tests.TestHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GenericRepository.Tests.Tests;

[TestFixture]
public class OwnedPropertiesTreeExtractorTests
{
    [SetUp]
    public void SetUp()
    {
        DatabaseInitializer.InitDatabaseWithExampleData();

        _serviceCollection = ServiceCollectionFactory.CreateServiceCollection();
        _sut = new OwnedPropertiesTreeExtractor();
    }

    private ServiceCollection _serviceCollection = null!;

    private OwnedPropertiesTreeExtractor _sut = null!;

    [Test]
    public void ExtractOwnedPropertiesTreeNodes_Extract_CorrectTree()
    {
        // Arrange
        var sp = _serviceCollection.BuildServiceProvider();
        var context = DatabaseInitializer.GetMemoryContext();

        var query = context.Companies
            .Include(x => x.Departments)
            .ThenInclude(x => x.HeadOfDepartment)
            //
            .Include(x => x.Departments)
            .ThenInclude(x => x.Managers)
            .ThenInclude(x => x.TeamsUnderManagement)
            .ThenInclude(x => x.TeamMembers)
            .ThenInclude(x => x.Member);

        // Act
        var ownedPropertiesTree = _sut.ExtractOwnedPropertiesTreeNodes(query);

        // Assert
        using (new AssertionScope())
        {
            ownedPropertiesTree.Should().NotBeNull();
            ownedPropertiesTree.Children.Should().HaveCount(1);
            // TODO: check other
        }
    }
}