namespace Voycar.Api.Web.Tests.Integration.Users.Get_Verify;

using Context;
using Microsoft.EntityFrameworkCore;
using Setup;
using V = Features.Users.Endpoints.Get.Verify;
using Registration = Features.Users.Endpoints.Post.Register;

public sealed class State : StateFixture
{
    public Guid PlanId { get; set; }
    public string PlanName = "basic";
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
        this._state.PlanId = this.Context.Plans.First(plan => plan.Name == this._state.PlanName).Id;
    }


    [Fact]
    public async Task Get_Request_ReturnsOk_And_VerifiesMember()
    {
        // Arrange
        const string email = "user@test.de";
        var memberClient = this._app.CreateClient();

        // Register new member
        var (registerHttpResponse, _) =
            await memberClient.POSTAsync<Registration.Endpoint, Registration.Request, Registration.Response>(
                new Registration.Request
                {
                    Email = email.ToLowerInvariant(),
                    Password = "password",
                    FirstName = "...",
                    LastName = "...",
                    Street = "...",
                    HouseNumber = "...",
                    PostalCode = "...",
                    City = "...",
                    Country = "...",
                    BirthDate = new DateOnly(2000,
                        1,
                        1),
                    BirthPlace = "...",
                    PhoneNumber = "...",
                    PlanId = this._state.PlanId
                }
            );

        var request = new V.Request { VerificationToken = "0B4391E5-7D74-40AB-8BCC-B7FD7B660D87" };

        var userInDb = this.Context.Users.First(u => u.Email == email);
        userInDb.VerificationToken = request.VerificationToken;

        await this.Context.SaveChangesAsync();

        // Act
        var httpResponse = await memberClient.GETAsync<V.Endpoint, V.Request>(request);

        // Arrange assertion
        await this.Context.Entry(userInDb).ReloadAsync();

        // Assert
        registerHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
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
        var httpResponse = await this._app.Member.GETAsync<V.Endpoint, V.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
