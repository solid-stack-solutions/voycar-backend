namespace Voycar.Api.Web.Tests.Features.Roles;

using System.Data.Common;
using Context;
using Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Web.Context;
using Web.Features.Roles.Endpoints.Post;
using Web.Features.Roles.Repository;

public class Tests(Sut app) : TestBase<Sut>
{
    [Fact]
    public async Task ExamplePost()
    {
        var httpResponseMessage = await app.Client.POSTAsync<Endpoint, Role>(new Role() { Name = "Hallo" });

        httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
