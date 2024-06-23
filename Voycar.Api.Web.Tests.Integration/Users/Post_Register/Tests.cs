namespace Voycar.Api.Web.Tests.Integration.Users.Post_Register;

using System.Runtime.CompilerServices;
using Context;
using Generic;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Service;
using R = Features.Users.Endpoints.Post.Register;


public sealed class State : StateFixture
{
    public Guid Id { get; set; }

    /// <summary>
    /// ID of member <see cref="Role"/>
    /// </summary>
    public static Guid RoleId { get; set; }

    public const string RoleName = "member";

    public R.Request Request { get; set; } = new()
    {
        Email = "test@test.de",
        Password = "notsafe987",
        FirstName = "null",
        LastName = "null",
        Street = "null",
        HouseNumber = "null",
        PostalCode = "null",
        City = "null",
        Country = "null",
        BirthDate = new DateOnly(2002, 12, 12),
        BirthPlace = "null",
        PhoneNumber = "null",
        PlanId = RoleId
    };
}


public class Tests : TestBase<App, State>
{
    private readonly App _app;
    private readonly State _state;
    private readonly VoycarDbContext Context;


    // Setup request client
    public Tests(App app, State state)
    {
        this._app = app;
        this.Context = this._app.Context;
        this._state = state;
        State.RoleId = this.Context.Roles.First(r => r.Name == State.RoleName).Id;
    }


    // Validator-Tests
    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToInvalidEmail()
    {
        // Arrange
        this._state.Request.Email = "";

        // Act
        var (httpResponse, _) =
            await this._app.Client.POSTAsync<R.Endpoint, R.Request, Entity>(this._state.Request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Cleanup
        this._state.Request.Email = "test@test.de";
    }

    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToInvalidPassword()
    {
        // Arrange
        // Act
        // Assert
    }

    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToInvalidFirstName()
    {
        // Arrange
        // Act
        // Assert
    }

    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToInvalidLastName()
    {
        // Arrange
        // Act
        // Assert
    }

    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToInvalidStreet()
    {
        // Arrange
        // Act
        // Assert
    }

    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToInvalidHouseNumber()
    {
        // Arrange
        // Act
        // Assert
    }


    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToInvalidPostalCode()
    {
        // Arrange
        // Act
        // Assert
    }


    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToInvalidCity()
    {
        // Arrange
        // Act
        // Assert
    }

    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToInvalidCountry()
    {
        // Arrange
        // Act
        // Assert
    }


    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToInvalidBirthDate()
    {
        // Arrange
        // Act
        // Assert
    }

    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToInvalidBirthPlace()
    {
        // Arrange
        // Act
        // Assert
    }

    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToInvalidPhoneNumber()
    {
        // Arrange
        // Act
        // Assert
    }
}
