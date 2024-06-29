namespace Voycar.Api.Web.Tests.Integration.Plans.Put_Personal;

using Context;
using Setup;
using P = Features.Plans.Endpoints.Put.Personal;


public sealed class State : StateFixture
{
    public Guid PlanId { get; set; }
    public const string PlanName = "reduced";
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


    private P.Request CreateValidRequest()
    {
        return new P.Request { PlanId = this._state.PlanId };
    }


    [Fact]
    public async Task Put_Request_ReturnsOk_And_UpdatePlanInDb()
    {
        // Arrange
        const string email = "abc@test.de";
        var request = this.CreateValidRequest();
        var memberClient = await ClientFactory.CreateMemberClient(this._app, this.Context, email, "password");

        // Act
        var httpResponse = await memberClient.PUTAsync<P.Endpoint, P.Request>(request);

        // Arrange assertion
        var userInDb = this.Context.Users.First(user => user.Email == email.ToLowerInvariant());
        var memberInDb = this.Context.Members.First(member => member.Id == userInDb.MemberId);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        userInDb.Should().NotBeNull();
        memberInDb.Should().NotBeNull();
        memberInDb.PlanId.Should().Be(request.PlanId);
    }


    [Fact]
    public async Task Put_Request_ReturnsBadRequest_DueToPlans_WithSameId()
    {
        // Arrange
        const string email = "cba@test.de";
        var request = this.CreateValidRequest();
        request.PlanId = this.Context.Plans.First(p => p.Name == "basic").Id;
        var memberClient = await ClientFactory.CreateMemberClient(this._app, this.Context, email, "password");

        // Act
        var httpResponse = await memberClient.PUTAsync<P.Endpoint, P.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Put_Request_ReturnsBadRequest_DueToInvalidPlanId()
    {
        // Arrange
        var request = this.CreateValidRequest();
        request.PlanId = new Guid("8FC8C386-42B3-44DC-8163-2197F626A290");

        // Act
        var httpResponse = await this._app.Member.PUTAsync<P.Endpoint, P.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Get_Personal_AsEmployee_Returns_Forbidden()
    {
        // Act
        var httpResponse = await this._app.Employee.PUTAsync<P.Endpoint, P.Request>(this.CreateValidRequest());

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }


    [Fact]
    public async Task Get_Personal_AsAdmin_Returns_Forbidden()
    {
        // Act
        var httpResponse = await this._app.Admin.PUTAsync<P.Endpoint, P.Request>(this.CreateValidRequest());

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
