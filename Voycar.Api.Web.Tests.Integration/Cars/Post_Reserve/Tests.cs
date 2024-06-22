namespace Voycar.Api.Web.Tests.Integration.Cars.Post_Reserve;

using System.Globalization;
using Context;
using Entities;

using R = Features.Cars.Endpoints.Post.Reserve;

public sealed class State : StateFixture
{
    /// <summary>
    /// ID of first <see cref="Car"/>
    /// </summary>
    public Guid CarId { get; set; }
    /// <summary>
    /// ID of first <see cref="Member"/>
    /// </summary>
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
        this._state = state;
        this.Context = this._app.Context;

        this._state.CarId = this.Context.Cars.First().Id;
        this._state.MemberId = this.Context.Members.First().Id;
    }

    private static R.Request CreateRequest(Guid carId, Guid memberId, string begin, string end)
    {
        return new R.Request
        {
            CarId = carId,
            MemberId = memberId,
            Begin = DateTime.Parse(begin, CultureInfo.InvariantCulture).ToUniversalTime(),
            End = DateTime.Parse(end, CultureInfo.InvariantCulture).ToUniversalTime()
        };
    }

    /// <summary>
    /// <see cref="CreateRequest(System.Guid,System.Guid,string,string)"/> but using <see cref="State.CarId"/> and <see cref="State.MemberId"/>
    /// </summary>
    private R.Request CreateRequest(string begin, string end)
    {
        return CreateRequest(this._state.CarId, this._state.MemberId, begin, end);
    }

    [Fact]
    public async Task Post_Reserve_ReturnsBadRequest_DueToCarId()
    {
        // Arrange
        var requestData = CreateRequest(
            new Guid(), // Just zeros, should not exist in database
            this._state.MemberId,
            "2000-01-01T08:00:00.000Z",
            "2000-01-01T18:00:00.000Z"
        );

        // Act
        var (httpResponse, _) = await this._app.Admin
            .GETAsync<R.Endpoint, R.Request, Guid>(requestData);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_Reserve_ReturnsBadRequest_DueToMemberId()
    {
        // Arrange
        var requestData = CreateRequest(
            this._state.CarId,
            new Guid(), // Just zeros, should not exist in database
            "2000-01-01T08:00:00.000Z",
            "2000-01-01T18:00:00.000Z"
        );

        // Act
        var (httpResponse, _) = await this._app.Admin
            .GETAsync<R.Endpoint, R.Request, Guid>(requestData);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_Reserve_ReturnsBadRequest_DueToNegativeTime()
    {
        // Arrange
        var requestData = this.CreateRequest(
            // Begin is later than end => "negative" time
            "2000-01-01T08:00:00.000Z",
            "2000-01-01T04:00:00.000Z"
        );

        // Act
        var (httpResponse, _) = await this._app.Admin
            .GETAsync<R.Endpoint, R.Request, Guid>(requestData);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_Reserve_ReturnsBadRequest_DueToZeroTime()
    {
        // Arrange
        var requestData = this.CreateRequest(
            // Begin equal to end => "zero" time
            "2000-01-01T08:00:00.000Z",
            "2000-01-01T08:00:00.000Z"
        );

        // Act
        var (httpResponse, _) = await this._app.Admin
            .GETAsync<R.Endpoint, R.Request, Guid>(requestData);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_Reserve_ReturnsOkAndGuid()
    {
        // Arrange
        var requestData = this.CreateRequest(
            "2000-01-01T08:00:00.000Z",
            "2000-01-01T18:00:00.000Z"
        );

        // Act
        var (httpResponse, response) = await this._app.Admin
            .GETAsync<R.Endpoint, R.Request, Guid>(requestData);

        // Assert
        this.Context.Reservations.Should().BeEmpty("Reservation table should be empty");
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().NotBe(new Guid());
    }
}
