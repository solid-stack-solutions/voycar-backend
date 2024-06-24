namespace Voycar.Api.Web.Tests.Integration.Users.Post_Register;

using Context;
using Entities;
using Microsoft.EntityFrameworkCore;
using R = Features.Users.Endpoints.Post.Register;


public sealed class State : StateFixture
{
    public Guid Id { get; set; }

    public static Guid RoleId { get; set; }
    public const string RoleName = "member";

    public static Guid PlanId { get; set; }
    public const string PlanName = "basic";

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
            PlanId = PlanId
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
        State.PlanId = this.Context.Plans.First(p => p.Name == State.PlanName).Id;
    }


    [Fact]
    public async Task Post_Request_ReturnsOk_And_SavesUserInDb()
    {
        // Arrange
        var request = State.CreateValidRequest();

        // Act
        var httpResponse = await this._app.Client.POSTAsync<R.Endpoint, R.Request>(request);

        // Arrange assertion
        var userInDb = await this.Context.Users.FirstOrDefaultAsync(user => user.Email == request.Email);
        var memberInDb = await this.Context.Members.FirstOrDefaultAsync(member => member.Id == userInDb!.MemberId);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        userInDb.Should().NotBeNull();
        memberInDb.Should().NotBeNull();

        userInDb.Email.Should().Be(request.Email);

        memberInDb.FirstName.Should().Be(request.FirstName);
        memberInDb.LastName.Should().Be(request.LastName);
        memberInDb.Street.Should().Be(request.Street);
        memberInDb.HouseNumber.Should().Be(request.HouseNumber);
        memberInDb.PostalCode.Should().Be(request.PostalCode);
        memberInDb.City.Should().Be(request.City);
        memberInDb.Country.Should().Be(request.Country);
        memberInDb.BirthDate.Should().Be(request.BirthDate);
        memberInDb.BirthPlace.Should().Be(request.BirthPlace);
        memberInDb.PhoneNumber.Should().Be(request.PhoneNumber);
        memberInDb.PlanId.Should().Be(request.PlanId);
        this._state.Id = userInDb.Id; // Save ID for later tests
    }


    [Fact]
    public async Task Post_Request_ReturnsOk_And_SavesUserInDb_WithLowerInvariantEmail()
    {
        // Arrange
        var request = State.CreateValidRequest();
        request.Email = "TEST@CAPSLOCK.de";

        // Act
        var httpResponse = await this._app.Client.POSTAsync<R.Endpoint, R.Request>(request);

        // Arrange assertion
        var userInDb =
            await this.Context.Users.FirstOrDefaultAsync(user => user.Email == request.Email.ToLowerInvariant());

        // Assert
        userInDb!.Email.Should().Be("test@capslock.de");
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }


    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToExistingUser()
    {
        // Arrange
        var request = State.CreateValidRequest();
        request.Email = "member.integration@test.de";

        // Act
        var httpResponse = await this._app.Client.POSTAsync<R.Endpoint, R.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToInvalidPlanId()
    {
        // Arrange
        var request = State.CreateValidRequest();
        request.Email = "invalidPlanId@test.de";
        request.PlanId = new Guid("538B96BC-6149-420E-9B08-4952AE5DDA72"); // Some random Guid

        // Act
        var httpResponse = await this._app.Client.POSTAsync<R.Endpoint, R.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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
