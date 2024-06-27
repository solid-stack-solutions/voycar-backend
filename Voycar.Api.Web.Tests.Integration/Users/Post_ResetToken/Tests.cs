namespace Voycar.Api.Web.Tests.Integration.Users.Post_ResetToken;

using Context;
using Setup;
using R = Features.Users.Endpoints.Post.ResetToken;


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
    public async Task Post_Request_AsMember_ReturnsOk_And_SetsResetToken()
    {
        // Arrange
        const string email = "member@test.de";
        var memberClient = await ClientFactory.CreateMemberClient(this._app, this.Context, email, "password");
        var request = new R.Request { Email = email };

        // Act
        var httpResponse = await memberClient.POSTAsync<R.Endpoint, R.Request>(request);

        // Arrange assertion
        var userInDb = this.Context.Users.First(u => u.Email == email);
        await this.Context.Entry(userInDb).ReloadAsync();

        // Assert
        userInDb.Should().NotBeNull();
        userInDb.PasswordResetToken.Should().NotBeNull();
        userInDb.ResetTokenExpires.Should().NotBeNull();
        userInDb.ResetTokenExpires.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(30), TimeSpan.FromMinutes(1));
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }


    [Fact]
    public async Task Post_Request_AsEmployee_ReturnsOk_And_SetsResetToken()
    {
        // Arrange
        const string email = "employee@test.de";
        var employeeClient = await ClientFactory.CreateEmployeeClient(this._app, this.Context, email, "password");
        var request = new R.Request { Email = email };

        // Act
        var httpResponse = await employeeClient.POSTAsync<R.Endpoint, R.Request>(request);

        // Arrange assertion
        var userInDb = this.Context.Users.First(u => u.Email == email);
        await this.Context.Entry(userInDb).ReloadAsync();

        // Assert
        userInDb.Should().NotBeNull();
        userInDb.PasswordResetToken.Should().NotBeNull();
        userInDb.ResetTokenExpires.Should().NotBeNull();
        userInDb.ResetTokenExpires.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(30), TimeSpan.FromMinutes(1));
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }


    [Fact]
    public async Task Post_Request_AsAdmin_ReturnsOk_And_SetsResetToken()
    {
        // Arrange
        const string email = "admin@test.de";
        var adminClient = await ClientFactory.CreateAdminClient(this._app, this.Context, email, "password");
        var request = new R.Request { Email = email };

        // Act
        var httpResponse = await adminClient.POSTAsync<R.Endpoint, R.Request>(request);

        // Arrange assertion
        var userInDb = this.Context.Users.First(u => u.Email == email);
        await this.Context.Entry(userInDb).ReloadAsync();

        // Assert
        userInDb.Should().NotBeNull();
        userInDb.PasswordResetToken.Should().NotBeNull();
        userInDb.ResetTokenExpires.Should().NotBeNull();
        userInDb.ResetTokenExpires.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(30), TimeSpan.FromMinutes(1));
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }


    // Validator-Test
    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToInvalidEmail()
    {
        // Arrange
        var request = new R.Request { Email = null! };

        // Act
        var httpResponse = await this._app.Member.POSTAsync<R.Endpoint, R.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
