namespace Voycar.Api.Web.Tests.Integration.Users.Post_Login;

using Context;
using Setup;
using R = Features.Users.Endpoints.Post.Login;


public sealed class State : StateFixture
{
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
        State.PlanId = this.Context.Plans.First(p => p.Name == State.PlanName).Id;
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
    public async Task Post_Request_ReturnsBadRequest_DueToIncorrectPassword()
    {
        // Arrange
        var memberClient =
            await ClientFactory.CreateMemberClient(this._app, this.Context, "member@test.de", "password");
        var request = new R.Request { Email = "member@test.de", Password = "diffPassword" };

        // Act
        var firstHttpResponse = await memberClient.PostAsync("/auth/logout", default); // Logout
        var secondHttpResponse = await this._app.Client.POSTAsync<R.Endpoint, R.Request>(request); // Login

        // Assert
        firstHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        secondHttpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToNonExistingUser()
    {
        // Arrange
        var client = this._app.CreateClient();
        var request = new R.Request { Email = "doesNotExist@test.de", Password = "diffPassword" };

        // Act
        var httpResponse = await client.POSTAsync<R.Endpoint, R.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Post_Request_ReturnsOk_And_MemberIsLoggedIn()
    {
        // Arrange
        var memberClient =
            await ClientFactory.CreateMemberClient(this._app, this.Context, "memberClient@test.de", "password");
        var request = new R.Request { Email = "memberClient@test.de", Password = "password" };

        // Act
        var firstHttpResponse = await memberClient.PostAsync("/auth/logout", default); // Logout
        var secondHttpResponse = await this._app.Client.POSTAsync<R.Endpoint, R.Request>(request); // Login
        var thirdHttpResponse = await this._app.Client.GetAsync("/user/whoami"); // Test if login was successful

        // Assert
        firstHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        secondHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        thirdHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }


    [Fact]
    public async Task Post_Request_ReturnsOk_And_EmployeeIsLoggedIn()
    {
        // Arrange
        var memberClient =
            await ClientFactory.CreateEmployeeClient(this._app, this.Context, "employeeClient@test.de", "password");
        var request = new R.Request { Email = "employeeClient@test.de", Password = "password" };

        // Act
        var firstHttpResponse = await memberClient.PostAsync("/auth/logout", default); // Logout
        var secondHttpResponse = await this._app.Client.POSTAsync<R.Endpoint, R.Request>(request); // Login
        var thirdHttpResponse =
            await this._app.Client.GetAsync(
                "/reservation/all"); // Test if login was successful / requires employee role

        // Assert
        firstHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        secondHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        thirdHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }


    [Fact]
    public async Task Post_Request_ReturnsOk_And_AdminIsLoggedIn()
    {
        // Arrange
        var memberClient =
            await ClientFactory.CreateAdminClient(this._app, this.Context, "adminClient@test.de", "password");
        var request = new R.Request { Email = "adminClient@test.de", Password = "password" };

        // Act
        var firstHttpResponse = await memberClient.PostAsync("/auth/logout", default); // Logout
        var secondHttpResponse = await this._app.Client.POSTAsync<R.Endpoint, R.Request>(request); // Login
        var thirdHttpResponse =
            await this._app.Client.GetAsync("/role/all"); // Test if login was successful / requires admin role

        // Assert
        firstHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        secondHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        thirdHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // Test login if member is already logged in
    [Fact]
    public async Task Post_Request_ReturnsOk_And_MemberIsStillLoggedIn()
    {
        // Arrange
        var memberClient =
            await ClientFactory.CreateMemberClient(this._app, this.Context, "memberClient@test.de", "password");
        var request = new R.Request { Email = "memberClient@test.de", Password = "password" };

        // Act
        var firstHttpResponse = await this._app.Client.POSTAsync<R.Endpoint, R.Request>(request); // Login
        var secondHttpResponse = await this._app.Client.POSTAsync<R.Endpoint, R.Request>(request); // Login
        var thirdHttpResponse = await this._app.Client.GetAsync("/user/whoami"); // Test if member is still logged in

        // Assert
        firstHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        secondHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        thirdHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
