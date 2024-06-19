namespace Voycar.Api.Web.Tests.Integration.Cars;

using System.Globalization;
using Context;
using Entities;

using C = Features.Cars.Endpoints;

public sealed class State : StateFixture
{
    public Guid StationId { get; set; }
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

        this._state.StationId = this.Context.Stations.First().Id;
    }

    private static C.Get.Available.Request ConstructAvailableRequest(Guid id, string begin, string end)
    {
        return new C.Get.Available.Request
        {
            StationId = id,
            Begin = DateTime.Parse(begin, CultureInfo.InvariantCulture),
            End = DateTime.Parse(end, CultureInfo.InvariantCulture)
        };
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
        var (httpResponse, response) = await this._app.Admin.GETAsync<C.Get.Available.Endpoint, C.Get.Available.Request, IEnumerable<Car>>(requestData);

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
        var (httpResponse, response) = await this._app.Admin.GETAsync<C.Get.Available.Endpoint, C.Get.Available.Request, IEnumerable<Car>>(requestData);

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
        var (httpResponse, response) = await this._app.Admin.GETAsync<C.Get.Available.Endpoint, C.Get.Available.Request, IEnumerable<Car>>(requestData);

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
        var (httpResponse, response) = await this._app.Admin.GETAsync<C.Get.Available.Endpoint, C.Get.Available.Request, IEnumerable<Car>>(requestData);

        // Arrange assertion
        var expectedCars = this.Context.Cars.Where(car => car.StationId == this._state.StationId);

        // Assert
        this.Context.Reservations.Should().BeEmpty("Reservation table needs to be empty for this test");
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().BeEquivalentTo(expectedCars);
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
        // Pick random car at station to reserve
        var reservedCar = this.Context.Cars.First(car => car.StationId == this._state.StationId);
        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            Begin = DateTime.Parse("2000-01-01T10:00:00.000Z", CultureInfo.InvariantCulture).ToUniversalTime(),
            End = DateTime.Parse("2000-01-01T12:00:00.000Z", CultureInfo.InvariantCulture).ToUniversalTime(),
            MemberId = this.Context.Members.First().Id,
            CarId = reservedCar.Id
        };
        this.Context.Reservations.Add(reservation);
        await this.Context.SaveChangesAsync();

        // Act
        var (httpResponse, response) = await this._app.Admin.GETAsync<C.Get.Available.Endpoint, C.Get.Available.Request, IEnumerable<Car>>(requestData);

        // Arrange assertion
        var expectedCars = this.Context.Cars.Where(car =>
            car.StationId == this._state.StationId && car.Id != reservedCar.Id);

        // Assert
        this.Context.Cars.Where(car => car.StationId == this._state.StationId)
            .Should().HaveCountGreaterOrEqualTo(1, "Station needs to have at least one car");
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
