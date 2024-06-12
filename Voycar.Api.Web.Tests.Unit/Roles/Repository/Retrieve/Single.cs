namespace Voycar.Api.Web.Tests.Unit.Roles.Endpoints.Get;


using Entities;
using FakeItEasy;
using Features.Roles.Repository;

public class Single: TestBase<App>
{
    // Set role id's
    private static readonly Guid AdminRoleId = new Guid("F3D56C5A-693F-48AA-B82D-659F40231C9A");
    private static readonly Guid EmployeeRoleId = new Guid("E9742BB5-FF66-4120-B6EB-88527F50B398");
    private static readonly Guid MemberRoleId = new Guid("25F82181-0C44-42B2-8182-072733A84C05");

    // Set roles
    private readonly Role roleAdmin = new Role { Id = AdminRoleId, Name = "admin" };
    private readonly Role roleEmployee = new Role { Id = EmployeeRoleId, Name = "employee" };
    private readonly Role roleMember = new Role { Id = MemberRoleId, Name = "member" };

    [Fact]
    public async Task GetSingleAdminRoleByIdSuccesful()
    {
        // Arrange
        var fakeRoleRepository = A.Fake<IRoles>();
        A.CallTo(() => fakeRoleRepository.Retrieve(AdminRoleId)).Returns(this.roleAdmin);

        // Act
        var result = fakeRoleRepository.Retrieve(AdminRoleId);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("admin", result.Name);
    }

    [Fact]
    public async Task GetSingleEmployeeRoleByIdSuccesful()
    {
        // Arrange
        var fakeRoleRepository = A.Fake<IRoles>();
        A.CallTo(() => fakeRoleRepository.Retrieve(EmployeeRoleId)).Returns(this.roleEmployee);

        // Act
        var result = fakeRoleRepository.Retrieve(EmployeeRoleId);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("employee", result.Name);
    }


    [Fact]
    public async Task GetSingleMemberRoleByIdSuccesful()
    {
        // Arrange
        var fakeRoleRepository = A.Fake<IRoles>();
        A.CallTo(() => fakeRoleRepository.Retrieve(MemberRoleId)).Returns(this.roleMember);

        // Act
        var result = fakeRoleRepository.Retrieve(MemberRoleId);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("member", result.Name);
    }


    [Fact]
    public async Task GetSingleAdminRoleByNameSuccesful()
    {
        // Arrange
        var fakeRoleRepository = A.Fake<IRoles>();
        A.CallTo(() => fakeRoleRepository.Retrieve(this.roleAdmin.Name)).Returns(this.roleAdmin);

        // Act
        var result = await fakeRoleRepository.Retrieve(this.roleAdmin.Name);

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<Role>(result);
        Assert.Equivalent(this.roleAdmin, result);
    }


    [Fact]
    public async Task GetSingleEmployeeRoleByNameSuccesful()
    {
        // Arrange
        var fakeRoleRepository = A.Fake<IRoles>();
        A.CallTo(() => fakeRoleRepository.Retrieve(this.roleEmployee.Name)).Returns(this.roleEmployee);

        // Act
        var result = await fakeRoleRepository.Retrieve(this.roleEmployee.Name);

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<Role>(result);
        Assert.Equivalent(result, this.roleEmployee);
    }


    [Fact]
    public async Task GetSingleMemberRoleByNameSuccesful()
    {
        // Arrange
        var fakeRoleRepository = A.Fake<IRoles>();
        A.CallTo(() => fakeRoleRepository.Retrieve(this.roleMember.Name)).Returns(this.roleMember);

        // Act
        var result = await fakeRoleRepository.Retrieve(this.roleMember.Name);

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<Role>(result);
        Assert.Equivalent(result, this.roleMember);
    }

}
