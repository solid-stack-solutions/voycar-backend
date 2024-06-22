namespace Voycar.Api.Web.Tests.Integration.Cars.Get_Available;

using System.Globalization;
using Context;
using Entities;
using A = Features.Cars.Endpoints.Get.Available;

public sealed class State : StateFixture
{
    /// <summary>
    /// ID of <see cref="Station"/> associated with <see cref="CarId"/>
    /// </summary>
    public Guid StationId { get; set; }
    /// <summary>
    /// ID of first <see cref="Car"/> (to be reserved in reservations)
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

        var car = this.Context.Cars.First();
        this._state.CarId     = car.Id;
        this._state.StationId = car.StationId;
        this._state.MemberId = this.Context.Members.First().Id;
    }

    private static A.Request CreateRequest(Guid id, string begin, string end)
    {
        return new A.Request
        {
            StationId = id,
            Begin = DateTime.Parse(begin, CultureInfo.InvariantCulture).ToUniversalTime(),
            End = DateTime.Parse(end, CultureInfo.InvariantCulture).ToUniversalTime()
        };
    }

    /// <summary>
    /// <see cref="CreateRequest(System.Guid,string,string)"/> but using <see cref="State.StationId"/> as <c>id</c>
    /// </summary>
    private A.Request CreateRequest(string begin, string end)
    {
        return CreateRequest(this._state.StationId, begin, end);
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

    /// <summary>
    /// <see cref="Car"/>s at <see cref="State.StationId"/>
    /// </summary>
    private IEnumerable<Car> CarsAtStation()
    {
        return this.Context.Cars.Where(car => car.StationId == this._state.StationId);
    }

    /// <summary>
    /// Send the given request to <see cref="Voycar.Api.Web.Features.Cars.Endpoints.Get.Available.Endpoint"/>
    /// after adding the given <see cref="Reservation"/>s to the database.
    /// Assert that <see cref="HttpStatusCode.OK"/> and the given <c>expectedAvailableCars</c> were received.
    /// Then remove the given <see cref="Reservation"/>s from the database again.
    /// </summary>
    private async Task RunTest(
        A.Request requestData,
        Reservation[] reservations,
        IEnumerable<Car> expectedAvailableCars)
    {
        // Arrange
        await this.Context.Reservations.AddRangeAsync(reservations);
        await this.Context.SaveChangesAsync();

        // Act
        var (httpResponse, response) = await this._app.Admin
            .GETAsync<A.Endpoint, A.Request, IEnumerable<Car>>(requestData);

        // Assert
        this.Context.Reservations.Should()
            .HaveCount(reservations.Length, "Reservation table should only contain the ones created in this test");
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().BeEquivalentTo(expectedAvailableCars);

        // Cleanup
        this.Context.Reservations.RemoveRange(reservations);
        await this.Context.SaveChangesAsync();
        this.Context.Reservations.Should().BeEmpty();
    }

    [Fact]
    public async Task Get_Available_ReturnsBadRequest_DueToGuid()
    {
        // Arrange
        var requestData = CreateRequest(
            new Guid(), // Just zeros, should not exist in database
            "2000-01-01T08:00:00.000Z",
            "2000-01-01T18:00:00.000Z"
        );

        // Act
        var (httpResponse, _) = await this._app.Admin
            .GETAsync<A.Endpoint, A.Request, IEnumerable<Car>>(requestData);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Get_Available_ReturnsBadRequest_DueToNegativeTime()
    {
        // Arrange
        var requestData = CreateRequest(
            this._state.StationId,
            // Begin is later than end => "negative" time
            "2000-01-01T08:00:00.000Z",
            "2000-01-01T04:00:00.000Z"
        );

        // Act
        var (httpResponse, _) = await this._app.Admin
            .GETAsync<A.Endpoint, A.Request, IEnumerable<Car>>(requestData);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Get_Available_ReturnsBadRequest_DueToZeroTime()
    {
        // Arrange
        var requestData = CreateRequest(
            this._state.StationId,
            // Begin equal to end => "zero" time
            "2000-01-01T08:00:00.000Z",
            "2000-01-01T08:00:00.000Z"
        );

        // Act
        var (httpResponse, _) = await this._app.Admin
            .GETAsync<A.Endpoint, A.Request, IEnumerable<Car>>(requestData);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Get_Available_ReturnsOkAndAllCars()
    {
        // Arrange
        var requestData = CreateRequest(
            this._state.StationId,
            "2000-01-01T08:00:00.000Z",
            "2000-01-01T18:00:00.000Z"
        );

        // Act
        var (httpResponse, response) = await this._app.Admin
            .GETAsync<A.Endpoint, A.Request, IEnumerable<Car>>(requestData);

        // Assert
        this.Context.Reservations.Should().BeEmpty("Reservation table should be empty");
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().BeEquivalentTo(this.CarsAtStation());
    }

    [Fact]
    public async Task Get_Available_ReturnsOkAndAllCars_WithOtherReservation()
    {
        // Reserve car from different station
        var reservedCarId = this.Context.Cars.First(car => car.StationId != this._state.StationId).Id;
        await this.RunTest(
            this.CreateRequest("2000-01-01T08:00:00.000Z", "2000-01-01T18:00:00.000Z"),
            [this.CreateReservation("2000-01-01T10:00:00.000Z", "2000-01-01T12:00:00.000Z", reservedCarId)],
            this.CarsAtStation()
        );
    }

    [Fact]
    public async Task Get_Available_ReturnsOkAndAllCars_WithCloselySurroundingReservations()
    {
        await this.RunTest(
            this.CreateRequest("2000-01-01T08:00:00.000Z", "2000-01-01T18:00:00.000Z"),
            [
                this.CreateReservation("2000-01-01T06:00:00.000Z", "2000-01-01T08:00:00.000Z"),
                this.CreateReservation("2000-01-01T18:00:00.000Z", "2000-01-01T20:00:00.000Z")
            ],
            this.CarsAtStation()
        );
    }

    [Fact]
    public async Task Get_Available_ReturnsOkAndAllCars_WithLooselySurroundingReservations()
    {
        await this.RunTest(
            this.CreateRequest("2000-01-01T08:00:00.000Z", "2000-01-01T18:00:00.000Z"),
            [
                this.CreateReservation("2000-01-01T06:00:00.000Z", "2000-01-01T07:00:00.000Z"),
                this.CreateReservation("2000-01-01T19:00:00.000Z", "2000-01-01T20:00:00.000Z")
            ],
            this.CarsAtStation()
        );
    }

    [Fact]
    public async Task Get_Available_ReturnsOkAndAvailableCars_WithReservationInMiddle()
    {
        await this.RunTest(
            this.CreateRequest("2000-01-01T08:00:00.000Z", "2000-01-01T18:00:00.000Z"),
            [this.CreateReservation("2000-01-01T10:00:00.000Z", "2000-01-01T12:00:00.000Z")],
            this.CarsAtStation().Where(car => car.Id != this._state.CarId)
        );
    }

    [Fact]
    public async Task Get_Available_ReturnsOkAndAvailableCars_WithReservationAtBegin()
    {
        await this.RunTest(
            this.CreateRequest("2000-01-01T08:00:00.000Z", "2000-01-01T18:00:00.000Z"),
            [this.CreateReservation("2000-01-01T07:00:00.000Z", "2000-01-01T09:00:00.000Z")],
            this.CarsAtStation().Where(car => car.Id != this._state.CarId)
        );
    }

    [Fact]
    public async Task Get_Available_ReturnsOkAndAvailableCars_WithReservationAtEnd()
    {
        await this.RunTest(
            this.CreateRequest("2000-01-01T08:00:00.000Z", "2000-01-01T18:00:00.000Z"),
            [this.CreateReservation("2000-01-01T17:00:00.000Z", "2000-01-01T19:00:00.000Z")],
            this.CarsAtStation().Where(car => car.Id != this._state.CarId)
        );
    }

    [Fact]
    public async Task Get_Available_ReturnsOkAndAvailableCars_WithOverlappingReservation()
    {
        await this.RunTest(
            this.CreateRequest("2000-01-01T08:00:00.000Z", "2000-01-01T18:00:00.000Z"),
            [this.CreateReservation("2000-01-01T07:00:00.000Z", "2000-01-01T19:00:00.000Z")],
            this.CarsAtStation().Where(car => car.Id != this._state.CarId)
        );
    }

    [Fact]
    public async Task Get_Available_ReturnsOkAndAvailableCars_WithExactlyOverlappingReservation()
    {
        const string begin = "2000-01-01T08:00:00.000Z";
        const string end   = "2000-01-01T18:00:00.000Z";
        await this.RunTest(
            this.CreateRequest(begin, end),
            [this.CreateReservation(begin, end)],
            this.CarsAtStation().Where(car => car.Id != this._state.CarId)
        );
    }
}
