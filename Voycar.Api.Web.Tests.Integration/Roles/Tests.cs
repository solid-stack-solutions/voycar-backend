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
    public async Task PostIsValid()
    {
        // Arrange
        const string requestName = "JuNiJa(Ke)²";
        var roleRequestData = new Role { Name = requestName };

        // Act
        var httpResponseMessage = await this._app.Client.POSTAsync<R.Post.SingleUnique, Role>(roleRequestData);
        var roleInDb = await this._context.Roles.FirstOrDefaultAsync(r => r.Name == roleRequestData.Name);

        // Assert
        httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
        roleInDb.Should().NotBeNull();
    }
}
