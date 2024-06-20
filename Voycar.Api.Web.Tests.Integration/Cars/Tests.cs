namespace Voycar.Api.Web.Tests.Integration.Cars;

using System.Globalization;
using Context;
using Entities;

using C = Features.Cars.Endpoints;

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

    private static C.Get.Available.Request ConstructAvailableRequest(Guid id, string begin, string end)
    {
        return new C.Get.Available.Request
        {
            StationId = id,
            Begin = DateTime.Parse(begin, CultureInfo.InvariantCulture).ToUniversalTime(),
            End = DateTime.Parse(end, CultureInfo.InvariantCulture).ToUniversalTime()
        };
    }

    /// <summary>
    /// <see cref="ConstructAvailableRequest(System.Guid,string,string)"/>, but using <see cref="State.StationId"/> as <c>id</c>
    /// </summary>
    private C.Get.Available.Request ConstructAvailableRequest(string begin, string end)
    {
        return ConstructAvailableRequest(this._state.StationId, begin, end);
    }

    /// <summary>
    /// Creates and returns new <see cref="Reservation"/> with <see cref="State.MemberId"/> and given <c>carId</c>
    /// </summary>
    private Reservation ConstructReservation(string begin, string end, Guid carId)
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
    private Reservation ConstructReservation(string begin, string end)
    {
        return this.ConstructReservation(begin, end, this._state.CarId);
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
    private async Task RunGetAvailableTest(
        C.Get.Available.Request requestData,
        Reservation[] reservations,
        IEnumerable<Car> expectedAvailableCars)
    {
        // Arrange
        await this.Context.Reservations.AddRangeAsync(reservations);
        await this.Context.SaveChangesAsync();

        // Act
        var (httpResponse, response) = await this._app.Admin
            .GETAsync<C.Get.Available.Endpoint, C.Get.Available.Request, IEnumerable<Car>>(requestData);

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
        var requestData = ConstructAvailableRequest(
            new Guid(), // Just zeros, should not exist in database
            "2000-01-01T08:00:00.000Z",
            "2000-01-01T18:00:00.000Z"
        );

        // Act
        var (httpResponse, response) = await this._app.Admin
            .GETAsync<C.Get.Available.Endpoint, C.Get.Available.Request, IEnumerable<Car>>(requestData);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Get_Available_ReturnsBadRequest_DueToNegativeTime()
    {
        // Arrange
        var requestData = ConstructAvailableRequest(
            this._state.StationId,
            // Begin is later than end => "negative" time
            "2000-01-01T08:00:00.000Z",
            "2000-01-01T04:00:00.000Z"
        );

        // Act
        var (httpResponse, response) = await this._app.Admin
            .GETAsync<C.Get.Available.Endpoint, C.Get.Available.Request, IEnumerable<Car>>(requestData);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Get_Available_ReturnsBadRequest_DueToZeroTime()
    {
        // Arrange
        var requestData = ConstructAvailableRequest(
            this._state.StationId,
            // Begin equal to end => "zero" time
            "2000-01-01T08:00:00.000Z",
            "2000-01-01T08:00:00.000Z"
        );

        // Act
        var (httpResponse, response) = await this._app.Admin
            .GETAsync<C.Get.Available.Endpoint, C.Get.Available.Request, IEnumerable<Car>>(requestData);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Get_Available_ReturnsOkAndAllCars()
    {
        // Arrange
        var requestData = ConstructAvailableRequest(
            this._state.StationId,
            "2000-01-01T08:00:00.000Z",
            "2000-01-01T18:00:00.000Z"
        );

        // Act
        var (httpResponse, response) = await this._app.Admin
            .GETAsync<C.Get.Available.Endpoint, C.Get.Available.Request, IEnumerable<Car>>(requestData);

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
        await this.RunGetAvailableTest(
            this.ConstructAvailableRequest("2000-01-01T08:00:00.000Z", "2000-01-01T18:00:00.000Z"),
            [this.ConstructReservation("2000-01-01T10:00:00.000Z", "2000-01-01T12:00:00.000Z", reservedCarId)],
            this.CarsAtStation()
        );
    }

    [Fact]
    public async Task Get_Available_ReturnsOkAndAllCars_WithSurroundingReservations()
    {
        await this.RunGetAvailableTest(
            this.ConstructAvailableRequest("2000-01-01T08:00:00.000Z", "2000-01-01T18:00:00.000Z"),
            [
                this.ConstructReservation("2000-01-01T06:00:00.000Z", "2000-01-01T08:00:00.000Z"),
                this.ConstructReservation("2000-01-01T18:00:00.000Z", "2000-01-01T20:00:00.000Z")
            ],
            this.CarsAtStation()
        );
    }


    [Fact]
    public async Task Get_Available_ReturnsOkAndAvailableCars()
    {
        await this.RunGetAvailableTest(
            this.ConstructAvailableRequest("2000-01-01T08:00:00.000Z", "2000-01-01T18:00:00.000Z"),
            [this.ConstructReservation("2000-01-01T10:00:00.000Z", "2000-01-01T12:00:00.000Z")],
            this.CarsAtStation().Where(car => car.Id != this._state.CarId)
        );
    }
}
