namespace Voycar.Api.Web.Tests.Integration.Roles;

using System.Text.Json;
using Context;
using Entities;
using Generic;
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
}

public class Tests : TestBase<App, State>
{
    // Always DI the app.cs to access methods
    private readonly App _app;
    private readonly VoycarDbContext _context;
    private readonly State _state;

    private readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };

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
        const string requestName = "JuNiJa(Ke)²";
        var roleRequestData = new Role { Name = requestName };

        // Act
        var httpResponse = await this._app.Admin.POSTAsync<R.Post.SingleUnique, Role>(roleRequestData);


        // Arrange assertion
        var responseMessage = await httpResponse.Content.ReadAsStringAsync();
        var responseEntity = JsonSerializer.Deserialize<Entity>(responseMessage, this.Options);

        var roleInDb = await this._context.Roles.FirstOrDefaultAsync(role => role.Name == roleRequestData.Name);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        responseEntity.Should().NotBeNull();
        responseEntity!.Id.Should().NotBeEmpty();
        this._state.Id = responseEntity.Id;

        roleInDb.Should().NotBeNull();
        roleInDb!.Name.Should().Be(requestName);

        roleInDb.Id.Should().Be(responseEntity.Id);
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
