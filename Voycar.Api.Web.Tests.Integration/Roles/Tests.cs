namespace Voycar.Api.Web.Tests.Integration.Roles;

using Context;
using Entities;
using Microsoft.EntityFrameworkCore;

// Include namespace alias of feature to test here
using R = Features.Roles.Endpoints;

public class Tests : TestBase<App>
{
    // Always DI the app.cs to access methods
    private readonly App _app;
    private readonly VoycarDbContext _context;

    // Setup request client
    public Tests(App app)
    {
        this._app = app;
        this._context = this._app.Context;
    }

    protected override async Task SetupAsync()
    {
        // Place one-time setup code here
    }

    protected override async Task TearDownAsync()
    {
        // Do cleanups here
    }

    [Fact]
    public async Task Post_NewRole_ReturnsOk_And_SavesInDb()
    {
        // Arrange
        const string requestName = "JuNiJa(Ke)²";
        var roleRequestData = new Role { Name = requestName };

        // Act
        var httpResponseMessage = await this._app.Admin.POSTAsync<R.Post.SingleUnique, Role>(roleRequestData);

        // Assert
        httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

        var roleInDb = await this._context.Roles.FirstOrDefaultAsync(role => role.Name == roleRequestData.Name);
        roleInDb.Should().NotBeNull();
    }
}
