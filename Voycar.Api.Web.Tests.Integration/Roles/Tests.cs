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

    private Guid id;
    // Setup request client
    public Tests(App app)
    {
        this._app = app;
        this._context = this._app.GetContext();
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

        id = roleRequestData.Id; //pass down id to be used in other test
        // Act
        var httpResponseMessage = await this._app.Client.POSTAsync<R.Post.SingleUnique, Role>(roleRequestData);
        var roleInDb = await this._context.Roles.FirstOrDefaultAsync(r => r.Name == roleRequestData.Name);

        // Assert
        httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
        roleInDb.Should().NotBeNull();
    }

    [Fact]
    public async Task GetIsValid()
    {
        const string requestName = "JuNiJa(Ke)²";
        var role = new Role { Name = requestName };
        role.Id = this.id;
        var roleRequestData = role;
        var httpResponseMessage = await this._app.Client.GETAsync<R.Get.Single, Role>(roleRequestData);

        httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PutIsValid()
    {
        const string requestName = "JuNiJa(Ke)²";
        var role = new Role { Name = requestName };
        role.Id = this.id;
        var roleRequestData = role;
        var httpResponseMessage = await this._app.Client.PUTAsync<R.Put.Single, Role>(roleRequestData);

        httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
