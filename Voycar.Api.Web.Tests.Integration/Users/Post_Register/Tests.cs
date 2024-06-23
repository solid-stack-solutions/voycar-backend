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

    public R.Request Request { get; set; } = CreateValidRequest();


    public static R.Request CreateValidRequest()
    {
        return new R.Request
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
        var request = State.CreateValidRequest();
        request.Email = "";

        // Act
        var httpResponse = await this._app.Client.POSTAsync<R.Endpoint, R.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToInvalidPassword()
    {
        // Arrange
        var request = State.CreateValidRequest();
        request.Password = ""; // Set invalid Password

        // Act
        var httpResponse = await this._app.Client.POSTAsync<R.Endpoint, R.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToInvalidFirstName()
    {
        // Arrange
        var request = State.CreateValidRequest();
        request.FirstName = ""; // Set invalid FirstName

        // Act
        var httpResponse = await this._app.Client.POSTAsync<R.Endpoint, R.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToInvalidLastName()
    {
        // Arrange
        var request = State.CreateValidRequest();
        request.LastName = "";

        // Act
        var httpResponse = await this._app.Client.POSTAsync<R.Endpoint, R.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToInvalidStreet()
    {
        // Arrange
        var request = State.CreateValidRequest();
        request.Street = "";

        // Act
        var httpResponse = await this._app.Client.POSTAsync<R.Endpoint, R.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToInvalidHouseNumber()
    {
        // Arrange
        var request = State.CreateValidRequest();
        request.HouseNumber = "";

        // Act
        var httpResponse = await this._app.Client.POSTAsync<R.Endpoint, R.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToInvalidPostalCode()
    {
        // Arrange
        var request = State.CreateValidRequest();
        request.PostalCode = "";

        // Act
        var httpResponse = await this._app.Client.POSTAsync<R.Endpoint, R.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToInvalidCity()
    {
        // Arrange
        var request = State.CreateValidRequest();
        request.City = "";

        // Act
        var httpResponse = await this._app.Client.POSTAsync<R.Endpoint, R.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToInvalidCountry()
    {
        // Arrange
        var request = State.CreateValidRequest();
        request.Country = "";

        // Act
        var httpResponse = await this._app.Client.POSTAsync<R.Endpoint, R.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToInvalidBirthDate()
    {
        // Arrange
        var request = State.CreateValidRequest();
        request.BirthDate = new DateOnly(2024, 1, 1);

        // Act
        var httpResponse = await this._app.Client.POSTAsync<R.Endpoint, R.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToInvalidBirthPlace()
    {
        // Arrange
        var request = State.CreateValidRequest();
        request.BirthPlace = "";

        // Act
        var httpResponse = await this._app.Client.POSTAsync<R.Endpoint, R.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToInvalidPhoneNumber()
    {
        // Arrange
        var request = State.CreateValidRequest();
        request.PhoneNumber = "";

        // Act
        var httpResponse = await this._app.Client.POSTAsync<R.Endpoint, R.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
