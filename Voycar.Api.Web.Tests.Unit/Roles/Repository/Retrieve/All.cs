namespace Voycar.Api.Web.Tests.Unit.Roles.Endpoints.Get;

using System.Collections;
using Bogus.DataSets;
using Entities;
using Features.Roles.Repository;
using FakeItEasy;


public class All : TestBase<App>
{
    [Fact]
    public async Task GetAllRolesSuccessful()
    {
        // Arrange
        var fakeRoleRepository = A.Fake<IRoles>();
        var roleAdmin = new Role { Name = "admin" };
        var roleEmployee = new Role { Name = "employee" };
        var roleMember = new Role { Name = "member" };
        A.CallTo(() => fakeRoleRepository.RetrieveAll())
            .Returns(new List<Role> { roleAdmin, roleEmployee, roleMember }.AsEnumerable());

        // Act
        var result =  fakeRoleRepository.RetrieveAll();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3,result.Count());
    }


    [Fact]
    public async Task GetAllRolesHandlesEmptyResult()
    {
        // Arrange
        var fakeRoleRepository = A.Fake<IRoles>();
        A.CallTo(() => fakeRoleRepository.RetrieveAll())
            .Returns(new List<Role>().AsEnumerable());

        // Act
        var result =  fakeRoleRepository.RetrieveAll();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }


    [Fact]
    public async Task GetAllRolesCorrectType()
    {
        // Arrange
        var fakeRoleRepository = A.Fake<IRoles>();
        A.CallTo(() => fakeRoleRepository.RetrieveAll())
            .Returns(new List<Role> { new Role { Name = "admin" } });

        // Act
        var result =  fakeRoleRepository.RetrieveAll();

        // Assert
        Assert.IsAssignableFrom<IEnumerable<Role>>(result);
    }


    [Fact]
    public async Task GetAllRolesRetrieveMustBeCalledOnce()
    {
        // Arrange
        var fakeRoleRepository = A.Fake<IRoles>();

        // Act
        var result = fakeRoleRepository.RetrieveAll();

        // Assert
        A.CallTo(() => fakeRoleRepository.RetrieveAll()).MustHaveHappenedOnceExactly();
    }


    [Fact]
    public async Task GetAllRolesWhenCalledParallel()
    {
        // Arrange
        var fakeRoleRepository = A.Fake<IRoles>();
        A.CallTo(() => fakeRoleRepository.RetrieveAll())
            .Returns(new List<Role> { new Role { Name = "admin" } });

        // Act
        var results = await Task.WhenAll(
            Task.Run(() => fakeRoleRepository.RetrieveAll()),
            Task.Run(() => fakeRoleRepository.RetrieveAll()),
            Task.Run(() => fakeRoleRepository.RetrieveAll())
        );

        // Assert
        Assert.All(results, r => Assert.NotNull(r));
        Assert.All(results, r => Assert.IsAssignableFrom<IEnumerable<Role>>(r));
        Assert.All(results, r => Assert.Contains("admin", r.Select(role => role.Name)));
    }
}



