namespace Voycar.Api.Web.Tests.Integration.Users.Get_Verify;

using Context;
using Microsoft.EntityFrameworkCore;
using Setup;
using V = Features.Users.Endpoints.Get.Verify;


public sealed class State : StateFixture
{
    public Guid MemberId { get; set; }
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
    }


    [Fact]
    public async Task Get_Request_ReturnsOk_And_VerifiesMember()
    {
        // Arrange
        const string email = "member@test.de";
        var memberClient = await ClientFactory.CreateMemberClient(this._app, this.Context, email, "password");
        var request = new V.Request { VerificationToken = "0B4391E5-7D74-40AB-8BCC-B7FD7B660D87" };

        var userInDb = this.Context.Users.First(user => user.Email == email);
        userInDb.VerificationToken = "0B4391E5-7D74-40AB-8BCC-B7FD7B660D87";

        await this.Context.SaveChangesAsync();

        // Act
        var httpResponse = await memberClient.GETAsync<V.Endpoint, V.Request>(request);

        // Arrange assertion
        await this.Context.Entry(userInDb).ReloadAsync();

        // Assert
        userInDb.Should().NotBeNull();
        userInDb.VerifiedAt.Should().NotBeNull();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }


    // Validator-Tests
    [Fact]
    public async Task Post_Request_ReturnsBadRequest_DueToInvalidResetToken()
    {
        // Arrange
        var request = new V.Request { VerificationToken = null! };

        // Act
        var httpResponse = await this._app.Member.POSTAsync<V.Endpoint, V.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
