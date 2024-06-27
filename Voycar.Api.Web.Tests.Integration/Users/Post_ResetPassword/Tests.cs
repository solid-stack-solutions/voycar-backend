namespace Voycar.Api.Web.Tests.Integration.Users.Post_ResetPassword;

using Context;
using Setup;
using R = Features.Users.Endpoints.Post.ResetPassword;
using Login = Features.Users.Endpoints.Post.Login;


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
    public async Task Post_Request_AsMember_ReturnsOk_And_ResetPassword()
    {
        // Arrange
        const string email = "member2@test.de";
        var memberClient = await ClientFactory.CreateMemberClient(this._app, this.Context, email, "password");
        var request = new R.Request
        {
            PasswordResetToken = "F01E0980-AB43-47E2-9300-DA69838E8797s",
            Password = "newPassword"
        };

        var userInDb = this.Context.Users.First(user => user.Email == email);
        userInDb.PasswordResetToken = request.PasswordResetToken;
        userInDb.ResetTokenExpires = DateTime.UtcNow.AddHours(3);

        await this.Context.SaveChangesAsync();

        // Act
        var firstHttpResponse = await memberClient.POSTAsync<R.Endpoint, R.Request>(request);
        var secondHttpResponse = await memberClient
            .POSTAsync<Login.Endpoint, Login.Request>(new Login.Request { Email = email, Password = request.Password });

        // Assert
        userInDb.Should().NotBeNull();

        firstHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        secondHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }


    [Fact]
    public async Task Post_Request_AsEmployee_ReturnsOk_And_ResetPassword()
    {
        // Arrange
        const string email = "employee@test.de";
        var employeeClient = await ClientFactory.CreateEmployeeClient(this._app, this.Context, email, "password");
        var request = new R.Request
        {
            PasswordResetToken = "2CE2B362-926C-43A9-87CE-5DADE92E1DBF",
            Password = "newPassword"
        };

        var userInDb = this.Context.Users.First(user => user.Email == email);
        userInDb.PasswordResetToken = request.PasswordResetToken;
        userInDb.ResetTokenExpires = DateTime.UtcNow.AddHours(3);

        await this.Context.SaveChangesAsync();

        // Act
        var firstHttpResponse = await employeeClient.POSTAsync<R.Endpoint, R.Request>(request);
        var secondHttpResponse = await employeeClient
            .POSTAsync<Login.Endpoint, Login.Request>(new Login.Request { Email = email, Password = request.Password });

        // Assert
        userInDb.Should().NotBeNull();

        firstHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        secondHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }


    [Fact]
    public async Task Post_Request_AsAdmin_ReturnsOk_And_ResetPassword()
    {
        // Arrange
        const string email = "admin@test.de";
        var adminClient = await ClientFactory.CreateAdminClient(this._app, this.Context, email, "password");
        var request = new R.Request
        {
            PasswordResetToken = "799A354C-7AD6-4305-8F34-72DC4ABFDD2F",
            Password = "newPassword"
        };

        var userInDb = this.Context.Users.First(user => user.Email == email);
        userInDb.PasswordResetToken = request.PasswordResetToken;
        userInDb.ResetTokenExpires = DateTime.UtcNow.AddHours(3);

        await this.Context.SaveChangesAsync();

        // Act
        var firstHttpResponse = await adminClient.POSTAsync<R.Endpoint, R.Request>(request);
        var secondHttpResponse = await adminClient
            .POSTAsync<Login.Endpoint, Login.Request>(new Login.Request { Email = email, Password = request.Password });

        // Assert
        userInDb.Should().NotBeNull();

        firstHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        secondHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }


    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToExpiredResetToken()
    {
        // Arrange
        const string email = "member@test.de";
        var memberClient = await ClientFactory.CreateMemberClient(this._app, this.Context, email, "password");
        var request = new R.Request
        {
            PasswordResetToken = "F01E0980-AB43-47E2-9300-DA69838E8797",
            Password = "newPassword"
        };

        var userInDb = this.Context.Users.First(user => user.Email == email);
        userInDb.PasswordResetToken = request.PasswordResetToken;
        userInDb.ResetTokenExpires = DateTime.UtcNow.AddSeconds(-10);

        await this.Context.SaveChangesAsync();

        // Act
        var httpResponse = await memberClient.POSTAsync<R.Endpoint, R.Request>(request);


        // Assert
        userInDb.Should().NotBeNull();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    // Validator-Tests
    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToInvalidResetToken()
    {
        // Arrange
        var request = new R.Request
        {
            PasswordResetToken = null!,
            Password = "password"
        };

        // Act
        var httpResponse = await this._app.Member.POSTAsync<R.Endpoint, R.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToInvalidPassword()
    {
        // Arrange
        const string email = "member3@test.de";
        var memberClient = await ClientFactory.CreateMemberClient(this._app, this.Context, email, "password");
        var request = new R.Request {
            PasswordResetToken = "F01E0980-AB43-47E2-9300-DA69838E8797",
            Password = "" };

        var userInDb = this.Context.Users.First(user => user.Email == email);
        userInDb.PasswordResetToken = request.PasswordResetToken;
        userInDb.ResetTokenExpires = DateTime.UtcNow.AddHours(3);

        await this.Context.SaveChangesAsync();
        // Act
        var httpResponse = await memberClient.POSTAsync<R.Endpoint, R.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
