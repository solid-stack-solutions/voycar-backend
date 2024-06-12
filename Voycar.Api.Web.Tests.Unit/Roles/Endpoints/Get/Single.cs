namespace Voycar.Api.Web.Tests.Unit.Roles.Endpoints.Get;

using System.Text.Json;
using Entities;
using FakeItEasy;
using Features.Roles.Repository;
using Microsoft.AspNetCore.Http;


public class Single : TestBase<App>
{

    private readonly JsonSerializerOptions options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public async Task RetrieveRoleSuccessful()
    {
        // Arrange
        var fakeRole = new Role { Id = new Guid("4BAB396C-8E26-4DC5-95E4-913F427E42F2"), Name = "fakeRole"};
        var fakeRoleRepository = A.Fake<IRoles>();

        A.CallTo(() => fakeRoleRepository.Retrieve(fakeRole.Id)).Returns(fakeRole);
        var ep = Factory.Create<Features.Roles.Endpoints.Get.Single>(fakeRoleRepository);

        var responseBodyStream = new MemoryStream();
        ep.HttpContext.Response.Body = responseBodyStream;

        // Act
        await ep.HandleAsync(fakeRole, default);

        responseBodyStream.Seek(0, SeekOrigin.Begin);
        var responseBodyText  = await new StreamReader(responseBodyStream).ReadToEndAsync();


        var retrievedRole = JsonSerializer.Deserialize<Role>(responseBodyText, this.options);
        var rsp = ep.HttpContext.Response;

        // Assert
        Assert.NotNull(rsp);
        Assert.Equal(fakeRole.Name, retrievedRole!.Name);
        A.CallTo(() => fakeRoleRepository.Retrieve(fakeRole.Id)).MustHaveHappenedOnceExactly();
        Assert.Equal(StatusCodes.Status200OK, rsp.StatusCode);
    }


    [Fact]
    public async Task RetrieveRoleFailure()
    {
        // Arrange
        var fakeRole = new Role { Id = new Guid("4BAB396C-8E26-4DC5-95E4-913F427E42F2"), Name = "fakeRole"};
        var fakeRoleRepository = A.Fake<IRoles>();

        A.CallTo(() => fakeRoleRepository.Retrieve(fakeRole.Id)).Returns(null);
        var ep = Factory.Create<Features.Roles.Endpoints.Get.Single>(fakeRoleRepository);

        // Act
        await ep.HandleAsync(fakeRole, default);
        var rsp = ep.HttpContext.Response;

        // Assert
        Assert.NotNull(rsp);
        A.CallTo(() => fakeRoleRepository.Retrieve(fakeRole.Id)).MustHaveHappenedOnceExactly();
        Assert.Equal(StatusCodes.Status404NotFound, rsp.StatusCode);
    }
}
