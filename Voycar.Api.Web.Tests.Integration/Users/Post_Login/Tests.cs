namespace Voycar.Api.Web.Tests.Integration.Users.Post_Login;

using Context;
using Setup;
using L = Features.Users.Endpoints.Post.Login;
using Register = Features.Users.Endpoints.Post.Register;


public sealed class State : StateFixture
{
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
        this._state = state;
        this.Context = this._app.Context;
        this._state.PlanId = this.Context.Plans.First(p => p.Name == State.PlanName).Id;
    }


    public Register.Request CreateValidRequest()
    {
        return new Register.Request
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
            PlanId = this._state.PlanId
        };
    }


    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToUnverifiedUser()
    {
        // Arrange
        var request = new L.Request { Email = "unverified@test.de", Password = "notsafe987" };

        await this._app.Client.POSTAsync<Register.Endpoint, Register.Request>(this.CreateValidRequest()); // Register new user without verifying

        // Act
        var httpResponse = await this._app.Client.POSTAsync<L.Endpoint, L.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToIncorrectPassword()
    {
        // Arrange
        var memberClient = await ClientFactory.CreateMemberClient(this._app, this.Context, "member@test.de", "password");
        var request = new L.Request { Email = "member@test.de", Password = "diffPassword" };

        // Act
        var firstHttpResponse = await memberClient.PostAsync("/auth/logout", default); // Logout
        var secondHttpResponse = await memberClient.POSTAsync<L.Endpoint, L.Request>(request); // Login

        // Assert
        firstHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK,
            "factory method produced logged in member who should be able to log out");
        ;
        secondHttpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToNonExistingUser()
    {
        // Arrange
        var client = this._app.CreateClient();
        var request = new L.Request { Email = "doesNotExist@test.de", Password = "diffPassword" };

        // Act
        var httpResponse = await client.POSTAsync<L.Endpoint, L.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Post_Request_ReturnsOk_And_MemberIsLoggedIn()
    {
        // Arrange
        var memberClient = await ClientFactory.CreateMemberClient(this._app, this.Context, "memberClient@test.de", "password");
        var request = new L.Request { Email = "memberClient@test.de", Password = "password" };

        // Act
        var firstHttpResponse = await memberClient.PostAsync("/auth/logout", default); // Logout
        var secondHttpResponse = await memberClient.POSTAsync<L.Endpoint, L.Request>(request); // Login
        var thirdHttpResponse = await memberClient.GetAsync("/user/whoami"); // Test if login was successful

        // Assert
        firstHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK,
            "factory method produced logged in member who should be able to log out");
        secondHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        thirdHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }


    [Fact]
    public async Task Post_Request_ReturnsOk_And_EmployeeIsLoggedIn()
    {
        // Arrange
        var employeeClient = await ClientFactory.CreateEmployeeClient(this._app, this.Context, "employeeClient@test.de", "password");
        var request = new L.Request { Email = "employeeClient@test.de", Password = "password" };

        // Act
        var firstHttpResponse = await employeeClient.PostAsync("/auth/logout", default); // Logout
        var secondHttpResponse = await employeeClient.POSTAsync<L.Endpoint, L.Request>(request); // Login
        var thirdHttpResponse = await employeeClient.GetAsync("/reservation/all"); // Test if login was successful / requires employee role

        // Assert
        firstHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK,
            "factory method produced logged in employee who should be able to log out");
        secondHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        thirdHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }


    [Fact]
    public async Task Post_Request_ReturnsOk_And_AdminIsLoggedIn()
    {
        // Arrange
        var adminClient = await ClientFactory.CreateAdminClient(this._app, this.Context, "adminClient@test.de", "password");
        var request = new L.Request { Email = "adminClient@test.de", Password = "password" };

        // Act
        var firstHttpResponse = await adminClient.PostAsync("/auth/logout", default); // Logout
        var secondHttpResponse = await adminClient.POSTAsync<L.Endpoint, L.Request>(request); // Login
        var thirdHttpResponse = await adminClient.GetAsync("/role/all"); // Test if login was successful / requires admin role

        // Assert
        firstHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK,
            "factory method produced logged in admin who should be able to log out");
        secondHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        thirdHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }


    // Test login if member is already logged in
    [Fact]
    public async Task Post_Request_ReturnsOk_And_MemberIsStillLoggedIn()
    {
        // Arrange
        var memberClient = await ClientFactory.CreateMemberClient(this._app, this.Context, "testtest@test.de", "password");
        var request = new L.Request { Email = "testtest@test.de", Password = "password" };

        // Act
        var firstHttpResponse = await memberClient.POSTAsync<L.Endpoint, L.Request>(request); // Login
        var secondHttpResponse = await memberClient.POSTAsync<L.Endpoint, L.Request>(request); // Login
        var thirdHttpResponse = await memberClient.GetAsync("/user/whoami"); // Test if member is still logged in

        // Assert
        firstHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        secondHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        thirdHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }


    [Fact]
    public async Task Post_Multiple_Request_ReturnOk_And_MembersAreLoggedIn()
    {
        // Arrange
        var client1 = await ClientFactory.CreateMemberClient(this._app, this.Context, "memberClient1@test.de", "password");
        var client2 = await ClientFactory.CreateMemberClient(this._app, this.Context, "memberClient2@test.de", "password");
        var request1 = new L.Request { Email = "memberClient1@test.de", Password = "password" };
        var request2 = new L.Request { Email = "memberClient2@test.de", Password = "password" };

        // Act
        var logoutResponse1 = await client1.PostAsync("/auth/logout", default);
        var logoutResponse2 = await client2.PostAsync("/auth/logout", default);

        var loginResponse1 = await client1.POSTAsync<L.Endpoint, L.Request>(request1);
        var loginResponse2 = await client2.POSTAsync<L.Endpoint, L.Request>(request2);

        // Assert
        logoutResponse1.StatusCode.Should().Be(HttpStatusCode.OK,
            "factory method produced logged in member who should be able to log out");
        logoutResponse2.StatusCode.Should().Be(HttpStatusCode.OK,
            "factory method produced logged in member who should be able to log out");
        loginResponse1.StatusCode.Should().Be(HttpStatusCode.OK);
        loginResponse2.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
