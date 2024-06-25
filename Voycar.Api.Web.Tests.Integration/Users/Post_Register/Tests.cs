namespace Voycar.Api.Web.Tests.Integration.Users.Post_Register;

using Context;
using Microsoft.EntityFrameworkCore;
using R = Features.Users.Endpoints.Post.Register;


public sealed class State : StateFixture
{
    public Guid RoleId { get; set; }
    public const string RoleName = "member";

    public Guid PlanId { get; set; }
    public const string PlanName = "basic";
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
        this._state.RoleId = this.Context.Roles.First(r => r.Name == State.RoleName).Id;
        this._state.PlanId = this.Context.Plans.First(p => p.Name == State.PlanName).Id;
    }


    public R.Request CreateValidRequest()
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
            BirthDate = new DateOnly(2002, 06, 25),
            BirthPlace = "null",
            PhoneNumber = "null",
            PlanId = (Guid)this._state.PlanId
        };
    }


    [Fact]
    public async Task Post_Request_ReturnsOk_And_SavesUserInDb()
    {
        // Arrange
        var request = this.CreateValidRequest();

        // Act
        var httpResponse = await this._app.Client.POSTAsync<R.Endpoint, R.Request>(request);

        // Arrange assertion
        var userInDb = await this.Context.Users.FirstOrDefaultAsync(user => user.Email == request.Email);
        var memberInDb = await this.Context.Members.FirstOrDefaultAsync(member => member.Id == userInDb.MemberId);

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
    }


    [Fact]
    public async Task Post_Request_JustTurned18_ReturnsOk_And_SavesUserInDb()
    {
        // Arrange
        var request = this.CreateValidRequest();
        request.Email = "test2@test2.de";
        request.BirthDate = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-18);

        // Act
        var httpResponse = await this._app.Client.POSTAsync<R.Endpoint, R.Request>(request);

        // Arrange assertion
        var userInDb = await this.Context.Users.FirstOrDefaultAsync(user => user.Email == request.Email);
        var memberInDb = await this.Context.Members.FirstOrDefaultAsync(member => member.Id == userInDb.MemberId);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        userInDb.Should().NotBeNull();
        memberInDb.Should().NotBeNull();
    }


    [Fact]
    public async Task Post_Request_ReturnsOk_And_SavesUserInDb_WithLowerInvariantEmail()
    {
        // Arrange
        var request = this.CreateValidRequest();
        request.Email = "TEST@CAPSLOCK.de";

        // Act
        var httpResponse = await this._app.Client.POSTAsync<R.Endpoint, R.Request>(request);

        // Arrange assertion
        var userInDb =
            await this.Context.Users.FirstOrDefaultAsync(user => user.Email == request.Email.ToLowerInvariant());

        // Assert
        userInDb.Should().NotBeNull();
        userInDb!.Email.Should().Be("test@capslock.de");
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }


    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToExistingUser()
    {
        // Arrange
        var request = this.CreateValidRequest();
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
        var request = this.CreateValidRequest();
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
        var request = this.CreateValidRequest();
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
        var request = this.CreateValidRequest();
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
        var request = this.CreateValidRequest();
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
        var request = this.CreateValidRequest();
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
        var request = this.CreateValidRequest();
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
        var request = this.CreateValidRequest();
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
        var request = this.CreateValidRequest();
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
        var request = this.CreateValidRequest();
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
        var request = this.CreateValidRequest();
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
        var request = this.CreateValidRequest();

        request.BirthDate = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var httpResponse = await this._app.Client.POSTAsync<R.Endpoint, R.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToInvalidBirthPlace()
    {
        // Arrange
        var request = this.CreateValidRequest();
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
        var request = this.CreateValidRequest();
        request.PhoneNumber = "";

        // Act
        var httpResponse = await this._app.Client.POSTAsync<R.Endpoint, R.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
