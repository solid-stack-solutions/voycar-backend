namespace Voycar.Api.Web.Tests.Unit.Roles.Endpoints.Delete;

using Entities;
using FakeItEasy;
using Features.Roles.Repository;
using Generic;
using Microsoft.AspNetCore.Http;


public class Single : TestBase<App>
{
    [Fact]
    public async Task DeleteRoleSuccessful()
    {
        // Arrange
        var fakeRole = new Role
        {
            Id = new Guid("F2E3156F-BC43-45F5-B8EE-024743E8BD2A"),
            Name = "fakeRole"
        };
        var fakeRoleRepository = A.Fake<IRoles>();
        A.CallTo(() => fakeRoleRepository.Delete(fakeRole.Id)).Returns(true);

        var ep = Factory.Create<Features.Roles.Endpoints.Delete.Single>(fakeRoleRepository);

        // Act
        await ep.HandleAsync(fakeRole, default);
        var rsp = ep.HttpContext.Response;

        // Assert
        A.CallTo(() => fakeRoleRepository.Delete(fakeRole.Id)).MustHaveHappenedOnceExactly();
        Assert.NotNull(rsp);
        Assert.Equal(StatusCodes.Status200OK, rsp.StatusCode);
    }


    [Fact]
    public async Task DeleteRoleFailure()
    {
        // Arrange
        var fakeRole = new Role
        {
            Id = new Guid("F2E3156F-BC43-45F5-B8EE-024743E8BD2A"),
            Name = "fakeRole"
        };
        var fakeRoleRepository = A.Fake<IRoles>();
        A.CallTo(() => fakeRoleRepository.Delete(fakeRole.Id)).Returns(false);

        var ep = Factory.Create<Features.Roles.Endpoints.Delete.Single>(fakeRoleRepository);

        // Act
        await ep.HandleAsync(fakeRole, default);
        var rsp = ep.HttpContext.Response;

        // Assert
        A.CallTo(() => fakeRoleRepository.Delete(fakeRole.Id)).MustHaveHappenedOnceExactly();
        Assert.NotNull(rsp);
        Assert.Equal(StatusCodes.Status404NotFound, rsp.StatusCode);
    }

}
