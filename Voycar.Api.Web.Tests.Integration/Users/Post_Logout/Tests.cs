namespace Voycar.Api.Web.Tests.Integration.Users.Post_Logout;

using Context;
using R = Features.Users.Endpoints.Post.Logout;

public sealed class State : StateFixture
{

}

public class Tests : TestBase<App>
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
    public async Task Post_ReturnsOK_And_MemberIsNoLonger_LoggedIn()
    {
        // Arrange

       var httpResponse =  await this._app.Member.PostAsync("/auth/logout", default);
        // Act
        // Assert
    }

    [Fact]
    public async Task Post_ReturnsOK_And_EmployeeIsNoLonger_LoggedIn()
    {
        // Arrange
        // Act
        // Assert
    }


    [Fact]
    public async Task Post_ReturnsOK_And_AdminIsNoLonger_LoggedIn()
    {
        // Arrange
        // Act
        // Assert
    }



}
