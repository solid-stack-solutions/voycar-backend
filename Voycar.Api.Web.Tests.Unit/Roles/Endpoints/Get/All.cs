namespace Voycar.Api.Web.Tests.Unit.Roles.Endpoints.Get;

using System.Text.Json;
using Entities;
using FakeItEasy;
using Features.Roles.Repository;
using Microsoft.AspNetCore.Http;


public class All : TestBase<App>
{
    private readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public async Task RetrieveAllRolesSuccessful()
    {
        // Arrange
        var fakeRoles = new List<Role>
        {
            new Role { Id = Guid.NewGuid(), Name = "admin" },
            new Role { Id = Guid.NewGuid(), Name = "employee" },
            new Role { Id = Guid.NewGuid(), Name = "member" }
        };

        var fakeRoleRepository = A.Fake<IRoles>();

        A.CallTo(() => fakeRoleRepository.RetrieveAll()).Returns(fakeRoles);
        var ep = Factory.Create<Features.Roles.Endpoints.Get.All>(fakeRoleRepository);

        var responseBodyStream = new MemoryStream();
        ep.HttpContext.Response.Body = responseBodyStream;

        // Act
        await ep.HandleAsync(default);

        responseBodyStream.Seek(0, SeekOrigin.Begin);
        var responseBodyText = await new StreamReader(responseBodyStream).ReadToEndAsync();

        var retrievedRoles = JsonSerializer.Deserialize<IEnumerable<Role>>(responseBodyText, this.Options);
        var rsp = ep.HttpContext.Response;

        // Assert
        A.CallTo(() => fakeRoleRepository.RetrieveAll()).MustHaveHappenedOnceExactly();
        Assert.NotNull(rsp);
        Assert.Equal(3, retrievedRoles.Count());

        foreach (var expectedRole in fakeRoles)
        {
            Assert.Contains(retrievedRoles, r => r.Name == expectedRole.Name);
        }

        Assert.Equal(StatusCodes.Status200OK, rsp.StatusCode);
    }
}
