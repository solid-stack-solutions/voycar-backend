namespace Voycar.Api.Web.Tests.Integration.Roles;

using Common;
using Context;
using Entities;
using Microsoft.EntityFrameworkCore;
using Xunit.Priority;

// Include namespace alias of feature to test here
using R = Features.Roles.Endpoints;


/// <summary>
/// StateFixture responsible for sharing the role ID between all tests.
/// </summary>
public sealed class State : StateFixture
{
    public Guid Id { get; set; }
    public const string RoleName1 = "JuNiJa(Ke)²";
    public const string RoleName2 = "Role2";
}

public class Tests : TestBase<App, State>
{
    // Always DI the app.cs to access methods
    private readonly App _app;
    private readonly VoycarDbContext _context;
    private readonly State _state;

    // Setup request client
    public Tests(App app, State state)
    {
        this._app = app;
        this._context = this._app.Context;
        this._state = state;
    }

    [Fact, Priority(0)]
    public async Task Post_NewRole_ReturnsOkAndID_And_SavesInDb()
    {
        // Arrange
        var roleRequestData = new Role { Name = State.RoleName1 };

        // Act
        var httpResponse = await this._app.Admin.POSTAsync<R.Post.SingleUnique, Role>(roleRequestData);


        // Arrange assertion
        var responseEntity = await ResponseResolver.ResolveResponse<Role>(httpResponse);
        var roleInDb = await this._context.Roles.FirstOrDefaultAsync(role => role.Name == roleRequestData.Name);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        responseEntity.Id.Should().NotBeEmpty();
        this._state.Id = responseEntity.Id; // save ID for later tests

        roleInDb.Should().NotBeNull();
        roleInDb!.Name.Should().Be(State.RoleName1);

        roleInDb.Id.Should().Be(responseEntity.Id);
    }

    [Fact, Priority(1)]
    public async Task Post_ExistingRole_ReturnsNoContent_And_IsAlreadyInDb()
    {
        // Arrange
        var roleRequestData = new Role { Name = State.RoleName1 };

        // Act
        var httpResponse = await this._app.Admin.POSTAsync<R.Post.SingleUnique, Role>(roleRequestData);

        // Arrange assertion
        var roleInDb = await this._context.Roles.FirstOrDefaultAsync(role => role.Name == roleRequestData.Name);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        roleInDb.Should().NotBeNull();
        roleInDb!.Name.Should().Be(State.RoleName1);
    }

    [Fact, Priority(2)]
    public async Task Get_SingleRole_ReturnsOkAndRole()
    {
        // Arrange
        var requestID = this._state.Id;

        // Act
        var httpResponse = await this._app.Admin.GetAsync($"role/{requestID}");

        // Arrange assertion
        var responseEntity = await ResponseResolver.ResolveResponse<Role>(httpResponse);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        responseEntity.Id.Should().Be(requestID);
        responseEntity.Name.Should().Be(State.RoleName1);
    }

    [Fact, Priority(3)]
    public async Task Delete_Role_ReturnsOk_And_DeletesInDb()
    {
        // Arrange
        var requestID = this._state.Id;

        // Act
        var httpResponse = await this._app.Admin.DeleteAsync($"role/{requestID}");

        // Arrange assertion
        var roleInDb = await this._context.Roles.FirstOrDefaultAsync(role => role.Name == State.RoleName1);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        roleInDb.Should().BeNull();
    }

}
