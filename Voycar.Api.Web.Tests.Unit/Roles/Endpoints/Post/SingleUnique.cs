namespace Voycar.Api.Web.Tests.Unit.Roles.Endpoints.Post;

using System.Text.Json;
using Entities;
using FakeItEasy;
using Features.Roles.Repository;
using Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic;


public class SingleUnique : TestBase<App>
{
    private readonly JsonSerializerOptions options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public async Task CreateUniqueSuccessful()
    {
        // Arrange
        var fakeRole = new Role { Id = new Guid("F2E3156F-BC43-45F5-B8EE-024743E8BD2A"), Name = "fakeRole" };
        var fakeRoleRepository = A.Fake<IRoles>();

        A.CallTo(() => fakeRoleRepository.CreateUnique(fakeRole)).Returns(fakeRole.Id);
        var ep = Factory.Create<Features.Roles.Endpoints.Post.SingleUnique>(fakeRoleRepository);

        var responseBodyStream = new MemoryStream();
        ep.HttpContext.Response.Body = responseBodyStream;

        // Act
        await ep.HandleAsync(fakeRole, default);

        responseBodyStream.Seek(0, SeekOrigin.Begin);
        var responseBodyText = await new StreamReader(responseBodyStream).ReadToEndAsync();
        var uniqueId = JsonSerializer.Deserialize<Entity>(responseBodyText, this.options);
        var rsp = ep.HttpContext.Response;

        // Assert
        A.CallTo(() => fakeRoleRepository.CreateUnique(fakeRole)).MustHaveHappenedOnceExactly();
        Assert.NotNull(rsp);
        Assert.Equal(fakeRole.Id, uniqueId.Id);
        Assert.Equal(StatusCodes.Status200OK, rsp.StatusCode);
    }

    [Fact]
    public async Task CreateUniqueFailure()
    {
        // Arrange
        var fakeRole = new Role { Id = new Guid("F2E3156F-BC43-45F5-B8EE-024743E8BD2A"), Name = "fakeRole" };
        var fakeRoleRepository = A.Fake<IRoles>();

        A.CallTo(() => fakeRoleRepository.CreateUnique(fakeRole)).Returns(null);
        var ep = Factory.Create<Features.Roles.Endpoints.Post.SingleUnique>(fakeRoleRepository);

        // Act
        await ep.HandleAsync(fakeRole, default);
        var rsp = ep.HttpContext.Response;

        // Assert
        A.CallTo(() => fakeRoleRepository.CreateUnique(fakeRole)).MustHaveHappenedOnceExactly();
        Assert.NotNull(rsp);
        Assert.Equal(StatusCodes.Status404NotFound, rsp.StatusCode);
    }
}
