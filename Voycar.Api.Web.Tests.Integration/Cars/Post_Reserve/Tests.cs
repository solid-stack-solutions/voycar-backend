namespace Voycar.Api.Web.Tests.Integration.Cars.Post_Reserve;

using System.Globalization;
using Context;
using Entities;
using Generic;

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

    /// <summary>
    /// Creates and returns new <see cref="Reservation"/> with <see cref="State.MemberId"/> and given <c>carId</c>
    /// </summary>
    private Reservation CreateReservation(string begin, string end, Guid carId)
    {
        return new Reservation
        {
            Id = Guid.NewGuid(),
            Begin = DateTime.Parse(begin, CultureInfo.InvariantCulture).ToUniversalTime(),
            End = DateTime.Parse(end, CultureInfo.InvariantCulture).ToUniversalTime(),
            MemberId = this._state.MemberId,
            CarId = carId
        };
    }

    /// <summary>
    /// Creates and returns new <see cref="Reservation"/> with <see cref="State.MemberId"/> and <see cref="State.CarId"/>
    /// </summary>
    private Reservation CreateReservation(string begin, string end)
    {
        return this.CreateReservation(begin, end, this._state.CarId);
    }

    private static R.Request CreateRequest(Guid carId, string begin, string end)
    {
        return new R.Request
        {
            CarId = carId,
            Begin = DateTime.Parse(begin, CultureInfo.InvariantCulture).ToUniversalTime(),
            End = DateTime.Parse(end, CultureInfo.InvariantCulture).ToUniversalTime()
        };
    }

    /// <summary>
    /// <see cref="CreateRequest(System.Guid,string,string)"/> but using <see cref="State.CarId"/>
    /// </summary>
    private R.Request CreateRequest(string begin, string end)
    {
        return CreateRequest(this._state.CarId, begin, end);
    }

    /// <summary>
    /// Send the given request to <see cref="Voycar.Api.Web.Features.Cars.Endpoints.Post.Reserve.Endpoint"/>
    /// after adding the given <see cref="Reservation"/>s to the database.
    /// Use assertions corresponding to the value of <c>expectConflict</c>, either expecting
    /// <see cref="HttpStatusCode.Conflict"/> or <see cref="HttpStatusCode.OK"/>.
    /// Then removes all rows of the <c>Reservations</c> table in the database.
    /// </summary>
    private async Task RunTest(
        R.Request requestData,
        Reservation[] reservations,
        bool expectConflict)
    {
        // Arrange
        await this.Context.Reservations.AddRangeAsync(reservations);
        await this.Context.SaveChangesAsync();

        // Act
        var (httpResponse, response) = await this._app.Member
            .POSTAsync<R.Endpoint, R.Request, Entity>(requestData);

        // Assert
        if (expectConflict)
        {
            httpResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
            this.Context.Reservations.Should().HaveCount(reservations.Length, "Reservations table should only contain the ones created in this test");
        }
        else
        {
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Id.Should().NotBe(new Guid());
            this.Context.Reservations.Should().HaveCount(reservations.Length + 1, "Reservations table should only contain the ones created in this test");
        }

        // Cleanup
        this.Context.Reservations.RemoveRange(this.Context.Reservations);
        await this.Context.SaveChangesAsync();
        this.Context.Reservations.Should().BeEmpty();
    }

    [Fact]
    public async Task Post_Reserve_ReturnsBadRequest_DueToCarId()
    {
        // Arrange
        var requestData = CreateRequest(
            new Guid(), // Just zeros, should not exist in database
            "2000-01-01T08:00:00.000Z",
            "2000-01-01T18:00:00.000Z"
        );

        // Act
        var (httpResponse, _) = await this._app.Member
            .POSTAsync<R.Endpoint, R.Request, Entity>(requestData);

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
        var (httpResponse, _) = await this._app.Member
            .POSTAsync<R.Endpoint, R.Request, Entity>(requestData);

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
        var (httpResponse, _) = await this._app.Member
            .POSTAsync<R.Endpoint, R.Request, Entity>(requestData);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_Reserve_ReturnsOkAndGuid_WithNoReservations()
    {
        await this.RunTest(
            this.CreateRequest("2000-01-01T08:00:00.000Z", "2000-01-01T18:00:00.000Z"),
            [],
            false
        );
    }

    [Fact]
    public async Task Post_Reserve_ReturnsOkAndGuid_WithOtherReservation()
    {
        // Reserve different car
        var reservedCarId = this.Context.Cars.First(car => car.Id != this._state.CarId).Id;
        await this.RunTest(
            this.CreateRequest("2000-01-01T08:00:00.000Z", "2000-01-01T18:00:00.000Z"),
            [this.CreateReservation("2000-01-01T10:00:00.000Z", "2000-01-01T12:00:00.000Z", reservedCarId)],
            false
        );
    }

    [Fact]
    public async Task Post_Reserve_ReturnsOkAndGuid_WithCloselySurroundingReservations()
    {
        await this.RunTest(
            this.CreateRequest("2000-01-01T08:00:00.000Z", "2000-01-01T18:00:00.000Z"),
            [
                this.CreateReservation("2000-01-01T06:00:00.000Z", "2000-01-01T08:00:00.000Z"),
                this.CreateReservation("2000-01-01T18:00:00.000Z", "2000-01-01T20:00:00.000Z")
            ],
            false
        );
    }

    [Fact]
    public async Task Post_Reserve_ReturnsOkAndGuid_WithLooselySurroundingReservations()
    {
        await this.RunTest(
            this.CreateRequest("2000-01-01T08:00:00.000Z", "2000-01-01T18:00:00.000Z"),
            [
                this.CreateReservation("2000-01-01T06:00:00.000Z", "2000-01-01T07:00:00.000Z"),
                this.CreateReservation("2000-01-01T19:00:00.000Z", "2000-01-01T20:00:00.000Z")
            ],
            false
        );
    }

    [Fact]
    public async Task Post_Reserve_ReturnsConflict_WithReservationInMiddle()
    {
        await this.RunTest(
            this.CreateRequest("2000-01-01T08:00:00.000Z", "2000-01-01T18:00:00.000Z"),
            [this.CreateReservation("2000-01-01T10:00:00.000Z", "2000-01-01T12:00:00.000Z")],
            true
        );
    }

    [Fact]
    public async Task Post_Reserve_ReturnsConflict_WithReservationAtBegin()
    {
        await this.RunTest(
            this.CreateRequest("2000-01-01T08:00:00.000Z", "2000-01-01T18:00:00.000Z"),
            [this.CreateReservation("2000-01-01T07:00:00.000Z", "2000-01-01T09:00:00.000Z")],
           true
        );
    }

    [Fact]
    public async Task Post_Reserve_ReturnsConflict_WithReservationAtEnd()
    {
        await this.RunTest(
            this.CreateRequest("2000-01-01T08:00:00.000Z", "2000-01-01T18:00:00.000Z"),
            [this.CreateReservation("2000-01-01T17:00:00.000Z", "2000-01-01T19:00:00.000Z")],
            true
        );
    }

    [Fact]
    public async Task Post_Reserve_ReturnsConflict_WithOverlappingReservation()
    {
        await this.RunTest(
            this.CreateRequest("2000-01-01T08:00:00.000Z", "2000-01-01T18:00:00.000Z"),
            [this.CreateReservation("2000-01-01T07:00:00.000Z", "2000-01-01T19:00:00.000Z")],
            true
        );
    }

    [Fact]
    public async Task Post_Reserve_ReturnsConflict_WithExactlyOverlappingReservation()
    {
        const string begin = "2000-01-01T08:00:00.000Z";
        const string end   = "2000-01-01T18:00:00.000Z";
        await this.RunTest(
            this.CreateRequest(begin, end),
            [this.CreateReservation(begin, end)],
            true
        );
    }
}
