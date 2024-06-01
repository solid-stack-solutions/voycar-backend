namespace Voycar.Api.Web.Tests.Features.Roles;

using Microsoft.EntityFrameworkCore;
using Context;
using Entities;
using Moq;
using Web.Features.Roles.Repository;
using Web.Features.Roles.Endpoints.Post;

public class Tests(App app) : TestBase<App>
{
    // TODO add tests and remove example
    [Fact]
    public async Task ExamplePost()
    {
        var httpResponseMessage = await app.Client.POSTAsync<Endpoint, Role>(new Role() { Name = "Hallo" });

        httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
