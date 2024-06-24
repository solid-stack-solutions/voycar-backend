namespace Voycar.Api.Web.Tests.Integration.Users.Post_Login;

using Context;
using Setup;
using R = Features.Users.Endpoints.Post.Login;


public sealed class State : StateFixture
{
    public Guid Id { get; set; }

    public static Guid RoleId { get; set; }
    public const string RoleName = "member";

    public static Guid PlanId { get; set; }
    public const string PlanName = "basic";


    public static Features.Users.Endpoints.Post.Register.Request CreateValidRequest()
    {
        return new Features.Users.Endpoints.Post.Register.Request
        {
            Email = "unverified@test.de",
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
        this._state = state;
        this.Context = this._app.Context;
    }


    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToUnverifiedUser()
    {
        // Arrange
        var request = new R.Request { Email = "unverified@test.de", Password = "notsafe987" };

        await this._app.Client
            .POSTAsync<Features.Users.Endpoints.Post.Register.Endpoint, Features.Users.Endpoints.Post.Register.Request>(
                State.CreateValidRequest());

        // Act
        var httpResponse = await this._app.Client.POSTAsync<R.Endpoint, R.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToPasswordMismatch()
    {
        // Arrange
        var memberClient = ClientFactory.CreateMemberClient(this._app, this.Context, "member@test.de");

        // Act
        // Assert
    }


    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToNonExistingUser()
    {
        // Arrange
        // Act
        // Assert
    }


    [Fact]
    public async Task Post_Request_ReturnsOk_And_MemberIsLoggedIn()
    {
        // Arrange
        // Act
        // Assert
    }


    [Fact]
    public async Task Post_Request_ReturnsOk_And_EmployeeIsLoggedIn()
    {
        // Arrange
        // Act
        // Assert
    }


    [Fact]
    public async Task Post_Request_ReturnsOk_And_AdminIsLoggedIn()
    {
        // Arrange
        // Act
        // Assert
    }
}
