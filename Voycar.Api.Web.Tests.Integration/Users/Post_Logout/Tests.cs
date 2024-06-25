namespace Voycar.Api.Web.Tests.Integration.Users.Post_Logout;

public class Tests : TestBase<App>
{
    private readonly App _app;


    // Setup request client
    public Tests(App app)
    {
        this._app = app;
    }


    [Fact]
    public async Task Post_ReturnsOK_And_MemberIsNoLonger_LoggedIn()
    {
        // Act
        var firstHttpResponse = await this._app.Member.PostAsync("/auth/logout", default);
        var secondHttpResponse = await this._app.Member.PostAsync("/auth/logout", default); // Check if logout succeeded

        // Assert
        firstHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        secondHttpResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }


    [Fact]
    public async Task Post_ReturnsOK_And_EmployeeIsNoLonger_LoggedIn()
    {
        // Act
        var firstHttpResponse = await this._app.Employee.PostAsync("/auth/logout", default);
        var secondHttpResponse =
            await this._app.Employee.PostAsync("/auth/logout", default); // Check if logout succeeded

        // Assert
        firstHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        secondHttpResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }


    [Fact]
    public async Task Post_ReturnsOK_And_AdminIsNoLonger_LoggedIn()
    {
        // Act
        var firstHttpResponse = await this._app.Admin.PostAsync("/auth/logout", default);
        var secondHttpResponse = await this._app.Admin.PostAsync("/auth/logout", default); // Check if logout succeeded

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
