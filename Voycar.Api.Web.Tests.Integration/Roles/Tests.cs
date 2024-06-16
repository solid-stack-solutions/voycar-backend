namespace Voycar.Api.Web.Tests.Integration.Roles;

using Context;
using Entities;
using Microsoft.EntityFrameworkCore;
using Xunit.Priority;

// Include namespace alias of feature to test here
using R = Features.Roles.Endpoints;

public class Tests : TestBase<App>
{
    // Always DI the app.cs to access methods
    private readonly App _app;
    private readonly VoycarDbContext _context;

    private Guid Id;
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

    [Fact, Priority(0)]
    public async Task Post_NewRole_ReturnsOk_And_SavesInDb()
    {
        // Arrange
        const string requestName = "JuNiJa(Ke)²";
        var roleRequestData = new Role { Name = requestName };

        this.Id = roleRequestData.Id; //pass down id to be used in other test

        // Act
        var httpResponseMessage = await this._app.Admin.POSTAsync<R.Post.SingleUnique, Role>(roleRequestData);

        // Assert
        httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

        var roleInDb = await this._context.Roles.FirstOrDefaultAsync(role => role.Name == roleRequestData.Name);
        roleInDb.Should().NotBeNull();
        // ToDo check name in db element
    }

    [Fact, Priority(1)]
    public async Task Post_ExistingRole_ReturnsNoContent_And_IsAlreadyInDb()
    {
        // Arrange
        const string requestName = "JuNiJa(Ke)²";
        var roleRequestData = new Role { Name = requestName };

        // Act
        var httpResponseMessage = await this._app.Admin.POSTAsync<R.Post.SingleUnique, Role>(roleRequestData);

        // Assert
        httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var roleInDb = await this._context.Roles.FirstOrDefaultAsync(role => role.Name == roleRequestData.Name);
        roleInDb.Should().NotBeNull();
        // ToDo check name in db element
    }

    [Fact, Priority(2)]
    public async Task GetIsValid()
    {
        //Arrange
        var roleRequestData = this.Id;
        //Act

        //Assert
        httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact, Priority(3)]
    public async Task DeleteIsValid()
    {
        //Arrange
        var roleRequestData = this.Id;

        //Act
        var httpResponseMessage = await this._app.Client.GETAsync<R.Delete.Single, Guid>(roleRequestData);

        //Assert
        httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
    }

}
