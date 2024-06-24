namespace Voycar.Api.Web.Tests.Integration.Users.Post_Logout;

using Context;
using Setup;


public class Tests : TestBase<App>
{
    private readonly App _app;
    private readonly VoycarDbContext Context;


    // Setup request client
    public Tests(App app)
    {
        this._app = app;
        this.Context = this._app.Context;
    }


    [Fact]
    public async Task Post_ReturnsOK_And_MemberIsNoLonger_LoggedIn()
    {
        // Arrange
        var memberClient = await ClientFactory.CreateMemberClient(this._app, this.Context, "memberClient@test.de");

        // Act
        var firstHttpResponse = await memberClient.PostAsync("/auth/logout", default);
        var secondHttpResponse = await memberClient.PostAsync("/auth/logout", default); // Check if logout succeeded

        // Assert
        firstHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        secondHttpResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }


    [Fact]
    public async Task Post_ReturnsOK_And_EmployeeIsNoLonger_LoggedIn()
    {
        // Arrange
        var employeeClient = await ClientFactory.CreateMemberClient(this._app, this.Context, "employeeClient@test.de");

        // Act
        var firstHttpResponse = await employeeClient.PostAsync("/auth/logout", default);
        var secondHttpResponse = await employeeClient.PostAsync("/auth/logout", default); // Check if logout succeeded

        // Assert
        firstHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        secondHttpResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }


    [Fact]
    public async Task Post_ReturnsOK_And_AdminIsNoLonger_LoggedIn()
    {
        // Arrange
        var adminClient = await ClientFactory.CreateMemberClient(this._app, this.Context, "adminClient@test.de");

        // Act
        var firstHttpResponse = await adminClient.PostAsync("/auth/logout", default);
        var secondHttpResponse = await adminClient.PostAsync("/auth/logout", default); // Check if logout succeeded

        // Assert
        firstHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        secondHttpResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }


    [Fact]
    public async Task Post_ReturnsUnauthorized_If_NotLoggedIn()
    {
        // Arrange
        var client = this._app.CreateClient();

        // Act
        var httpResponse = await client.PostAsync("/auth/logout", default);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
