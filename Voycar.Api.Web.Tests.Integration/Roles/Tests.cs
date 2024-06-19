namespace Voycar.Api.Web.Tests.Integration.Roles;

using Context;
using Entities;
using Generic;
using Microsoft.EntityFrameworkCore;
using Xunit.Priority;

// Include namespace alias of feature to test here
using R = Features.Roles.Endpoints;


/// <summary>
/// StateFixture responsible for sharing the role ID between all tests and
/// constant values.
/// </summary>
public sealed class State : StateFixture
{
    public Guid Id { get; set; }
    public const string RoleName = "JuNiJa(Ke)²";
}

public class Tests : TestBase<App, State>
{
    // Always DI the app.cs to access methods
    private readonly App _app;
    private readonly State _state;

    private readonly VoycarDbContext Context;

    // Setup request client
    public Tests(App app, State state)
    {
        this._app = app;
        this._state = state;
        this.Context = this._app.Context;
    }

    [Fact, Priority(0)]
    public async Task Post_NewRole_ReturnsOkAndID_And_SavesInDb()
    {
        // Arrange
        var roleRequestData = new Role { Name = State.RoleName };

        // Act
        var (httpResponse, response) = await this._app.Admin.POSTAsync<R.Post.SingleUnique, Role, Entity>(roleRequestData);


        // Arrange assertion
        var roleInDb = await this.Context.Roles.FirstOrDefaultAsync(role => role.Name == roleRequestData.Name);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Id.Should().NotBeEmpty();
        this._state.Id = response.Id; // Save ID for later tests

        roleInDb.Should().NotBeNull();
        roleInDb!.Id.Should().Be(response.Id);
    }

    [Fact, Priority(1)]
    public async Task Post_ExistingRole_ReturnsNoContent_And_IsAlreadyInDb()
    {
        // Arrange
        var roleRequestData = new Role { Name = State.RoleName };

        // Act
        var httpResponse = await this._app.Admin.POSTAsync<R.Post.SingleUnique, Role>(roleRequestData);

        // Arrange assertion
        var roleInDb = await this.Context.Roles.FirstOrDefaultAsync(role => role.Name == roleRequestData.Name);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        roleInDb.Should().NotBeNull();
    }

    [Fact, Priority(2)]
    public async Task Get_SingleRole_ReturnsOkAndRole()
    {
        // Arrange
        var requestEntityData = new Entity { Id = this._state.Id };

        // Act
        var (httpResponse, response) = await this._app.Admin.GETAsync<R.Get.Single, Entity, Role>(requestEntityData);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Id.Should().Be(requestEntityData.Id);
        response.Name.Should().Be(State.RoleName);
    }

    [Fact, Priority(2)]
    public async Task Get_AllRoles_ReturnsOkAndRoles()
    {
        // Act
        var (httpResponse, response) = await this._app.Admin.GETAsync<R.Get.All, IEnumerable<Role>>();

        // Arrange assert
        response = response as Role[] ?? response.ToArray();
        var expectedRole = new Role { Id = this._state.Id, Name = State.RoleName };

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        response.Should().HaveCount(4,
            "1 Role was added during test, while 3 roles were created at startup"
        );
        response.Contains(expectedRole).Should().BeTrue();
    }

    [Fact, Priority(2)]
    public async Task Put_Role_ReturnsOk_And_UpdatesInDb()
    {
        // Arrange
        var roleRequestData = new Role { Id = this._state.Id, Name =  "NewRoleName"};

        // Act
        var httpResponse = await this._app.Admin.PUTAsync<R.Put.Single, Role>(roleRequestData);

        // Arrange assert
        var roleInDb = await this.Context.Roles.FirstOrDefaultAsync(role => role.Name == roleRequestData.Name);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        roleInDb.Should().NotBeNull();
        roleInDb!.Id.Should().Be(this._state.Id);
    }

    [Fact, Priority(3)]
    public async Task Delete_Role_ReturnsOk_And_DeletesInDb()
    {
        // Arrange
        var requestID = this._state.Id;

        // Act
        var httpResponse = await this._app.Admin.DeleteAsync($"role/{requestID}");

        // Arrange assertion
        var roleInDb = await this.Context.Roles.FirstOrDefaultAsync(role => role.Name == State.RoleName);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        roleInDb.Should().BeNull();
    }

}
