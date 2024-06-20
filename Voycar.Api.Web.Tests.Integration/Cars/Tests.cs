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
    private IQueryable<Car> CarsAtStation()
    {
        return this.Context.Cars.Where(car => car.StationId == this._state.StationId);
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
        this.Context.Reservations.Should().BeEmpty("Reservation table needs to be empty for this test");
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().BeEquivalentTo(this.CarsAtStation());
    }

    [Fact]
    public async Task Get_Available_ReturnsOkAndAllCars_WithOtherReservation()
    {
        // Arrange
        var requestData = ConstructAvailableRequest(
            this._state.StationId,
            "2000-01-01T08:00:00.000Z",
            "2000-01-01T18:00:00.000Z"
        );
        // Reserve car from different station
        var reservedCarId = this.Context.Cars.First(car => car.StationId != this._state.StationId).Id;
        var reservation = this.ConstructReservation("2000-01-01T10:00:00.000Z", "2000-01-01T12:00:00.000Z", reservedCarId);
        this.Context.Reservations.Add(reservation);
        await this.Context.SaveChangesAsync();

        // Act
        var (httpResponse, response) = await this._app.Admin
            .GETAsync<C.Get.Available.Endpoint, C.Get.Available.Request, IEnumerable<Car>>(requestData);

        // Assert
        this.Context.Reservations.Should()
            .HaveCount(1, "Reservation table should contain only the one created in this test");
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().BeEquivalentTo(this.CarsAtStation());

        // Cleanup
        this.Context.Reservations.Remove(reservation);
        await this.Context.SaveChangesAsync();
        this.Context.Reservations.Should().BeEmpty();
    }

    [Fact]
    public async Task Get_Available_ReturnsOkAndAllCars_WithSurroundingReservations()
    {
        // Arrange
        var requestData = ConstructAvailableRequest(
            this._state.StationId,
            "2000-01-01T08:00:00.000Z",
            "2000-01-01T18:00:00.000Z"
        );
        var reservation1 = this.ConstructReservation("2000-01-01T06:00:00.000Z", "2000-01-01T08:00:00.000Z");
        var reservation2 = this.ConstructReservation("2000-01-01T18:00:00.000Z", "2000-01-01T20:00:00.000Z");
        this.Context.Reservations.Add(reservation1);
        this.Context.Reservations.Add(reservation2);
        await this.Context.SaveChangesAsync();

        // Act
        var (httpResponse, response) = await this._app.Admin
            .GETAsync<C.Get.Available.Endpoint, C.Get.Available.Request, IEnumerable<Car>>(requestData);

        // Assert
        this.Context.Reservations.Should()
            .HaveCount(2, "Reservation table should contain only the two created in this test");
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().BeEquivalentTo(this.CarsAtStation());

        // Cleanup
        this.Context.Reservations.Remove(reservation1);
        this.Context.Reservations.Remove(reservation2);
        await this.Context.SaveChangesAsync();
        this.Context.Reservations.Should().BeEmpty();
    }


    [Fact]
    public async Task Get_Available_ReturnsOkAndAvailableCars()
    {
        // Arrange
        var requestData = ConstructAvailableRequest(
            this._state.StationId,
            "2000-01-01T08:00:00.000Z",
            "2000-01-01T18:00:00.000Z"
        );
        var reservation = this.ConstructReservation("2000-01-01T10:00:00.000Z", "2000-01-01T12:00:00.000Z");
        this.Context.Reservations.Add(reservation);
        await this.Context.SaveChangesAsync();

        // Act
        var (httpResponse, response) = await this._app.Admin
            .GETAsync<C.Get.Available.Endpoint, C.Get.Available.Request, IEnumerable<Car>>(requestData);

        // Arrange assertion
        var expectedCars = this.CarsAtStation().Where(car => car.Id != this._state.CarId);

        // Assert
        this.CarsAtStation().Should()
            .HaveCountGreaterOrEqualTo(1, "Station needs to have at least one car");
        this.Context.Reservations.Should()
            .HaveCount(1, "Reservation table should contain only the one created in this test");
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().BeEquivalentTo(expectedCars);

        // Cleanup
        this.Context.Reservations.Remove(reservation);
        await this.Context.SaveChangesAsync();
        this.Context.Reservations.Should().BeEmpty();
    }
}
