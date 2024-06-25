namespace Voycar.Api.Web.Tests.Integration.Members.Get_Personal;

using Context;
using Microsoft.EntityFrameworkCore;
using P = Features.Members.Endpoints.Get.Personal;


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
        this._state.MemberId = this.Context.Members.First().Id;
    }


    [Fact]
    public async Task Get_Personal_ReturnsOk_And_MemberInformation()
    {
        // Arrange
        var member = await this.Context.Members.FirstOrDefaultAsync(mem => mem.Id == this._state.MemberId);
        var user = await this.Context.Users.FirstOrDefaultAsync(user => user.MemberId == this._state.MemberId);
        var plan = await this.Context.Plans.FirstOrDefaultAsync(plan => plan.Id == member.PlanId);

        // Act
        var (httpResponse, response) =
            await this._app.Member.GETAsync<P.Endpoint, P.Request, P.Response>(new P.Request());

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        member.Should().NotBeNull();

        member.Id.Should().Be(response.MemberId);
        member.FirstName.Should().Be(response.FirstName);
        member.LastName.Should().Be(response.LastName);
        member.Street.Should().Be(response.Street);
        member.HouseNumber.Should().Be(response.HouseNumber);
        member.PostalCode.Should().Be(response.PostalCode);
        member.City.Should().Be(response.City);
        member.Country.Should().Be(response.Country);
        member.BirthDate.Should().Be(response.BirthDate);
        member.BirthPlace.Should().Be(response.BirthPlace);

        member.PhoneNumber.Should().Be(response.PhoneNumber);
        user.Email.Should().Be(response.Email);

        plan.Name.Should().Be(response.PlanName);
    }


    [Fact]
    public async Task Get_Personal_AsEmployee_Returns_Forbidden()
    {
        // Act
        var (httpResponse, _) =
            await this._app.Employee.GETAsync<P.Endpoint, P.Request, P.Response>(new P.Request());

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Get_Personal_AsAdmin_Returns_Forbidden()
    {
        // Act
        var (httpResponse, _) =
            await this._app.Admin.GETAsync<P.Endpoint, P.Request, P.Response>(new P.Request());

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
