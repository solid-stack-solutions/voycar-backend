namespace Voycar.Api.Web.Tests.Unit.Roles.Endpoints.Put;

using System.Text.Json;
using Entities;
using FakeItEasy;
using Features.Roles.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;


public class Single : TestBase<App>
{
    [Fact]
    public async Task UpdateRoleSuccessful()
    {
        // Arrange
        var fakeRole = new Role
        {
            Id = new Guid("F2E3156F-BC43-45F5-B8EE-024743E8BD2A"),
            Name = "fakeRole"
        };
        var fakeRoleRepository = A.Fake<IRoles>();
        A.CallTo(() => fakeRoleRepository.Update(fakeRole)).Returns(true);

        var ep = Factory.Create<Features.Roles.Endpoints.Put.Single>(fakeRoleRepository);
        fakeRole.Name = "newRoleName";
        var req = fakeRole;

        // Act
        await ep.HandleAsync(req, default);
        var rsp = ep.HttpContext.Response;

        // Assert
        A.CallTo(() => fakeRoleRepository.Update(fakeRole)).MustHaveHappenedOnceExactly();
        Assert.NotNull(rsp);
        Assert.Equal(StatusCodes.Status200OK, rsp.StatusCode);
    }


    [Fact]
    public async Task UpdateRoleFailure()
    {
        // Arrange
        var fakeRole = new Role
        {
            Id = new Guid("F2E3156F-BC43-45F5-B8EE-024743E8BD2A"),
            Name = "fakeRole"
        };
        var fakeRoleRepository = A.Fake<IRoles>();
        A.CallTo(() => fakeRoleRepository.Update(fakeRole)).Returns(false);

        var ep = Factory.Create<Features.Roles.Endpoints.Put.Single>(fakeRoleRepository);
        fakeRole.Name = "newRoleName";
        var req = fakeRole;

        // Act
        await ep.HandleAsync(req, default);
        var rsp = ep.HttpContext.Response;

        // Assert
        A.CallTo(() => fakeRoleRepository.Update(fakeRole)).MustHaveHappenedOnceExactly();
        Assert.NotNull(rsp);
        Assert.Equal(StatusCodes.Status404NotFound, rsp.StatusCode);
    }
}
