namespace Voycar.Api.Web.Tests.Integration.Reservations.Get_Personal;

using System.Globalization;
using Context;
using Entities;
using P = Features.Reservations.Endpoints.Get.Personal;


public sealed class State : StateFixture
{
    public Guid MemberId { get; set; }
    public Guid CarId { get; set; }
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
        this._state.MemberId = this.Context.Members.First().Id;
        this._state.CarId = this.Context.Cars.First().Id;
    }


    private Reservation CreateReservation(string begin, string end)
    {
        return new Reservation
        {
            Id = Guid.NewGuid(),
            Begin = DateTime.Parse(begin, CultureInfo.InvariantCulture).ToUniversalTime(),
            End = DateTime.Parse(end, CultureInfo.InvariantCulture).ToUniversalTime(),
            MemberId = this._state.MemberId,
            CarId = this._state.CarId
        };
    }

    private static void ValidateReservations(List<Reservation> reservations, List<Reservation> responseReservations)
    {
        foreach (var reservation in reservations)
        {
            var responseReservation = responseReservations.First(r => r.Id == reservation.Id);
            responseReservation.Should().NotBeNull();

            responseReservation!.Begin.ToShortTimeString().Should().Be(reservation.Begin.ToShortTimeString());
            responseReservation.End.ToShortTimeString().Should().Be(reservation.End.ToShortTimeString());
        }
    }

    [Fact]
    public async Task Get_Request_ReturnsOk_And_ListOfReservations()
    {
        // Arrange
        var reservations = new List<Reservation>
        {
            // Expired
            this.CreateReservation("2022-01-01T08:00:00.000Z", "2022-01-01T18:00:00.000Z"),
            // Active
            this.CreateReservation(
                DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture),
                DateTime.UtcNow.AddHours(3).ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture)
            ),
            // Planned
            this.CreateReservation("2025-01-01T08:00:00.000Z", "2025-01-01T18:00:00.000Z")
        };

        await this.Context.Reservations.AddRangeAsync(reservations);
        await this.Context.SaveChangesAsync();

        // Act
        var (httpResponse, response) =
            await this._app.Member.GETAsync<P.Endpoint, P.Request, P.Response>(new P.Request());

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.expired.Count.Should().Be(1);
        response.active.Count.Should().Be(1);
        response.planned.Count.Should().Be(1);

        // Cleanup
        this.Context.RemoveRange(reservations);
        await this.Context.SaveChangesAsync();
    }


    [Fact]
    public async Task Get_Request_ReturnsOk_And_ListOfExpiredReservations()
    {
        // Arrange
        var reservations = new List<Reservation>
        {
            this.CreateReservation("2021-01-01T08:00:00.000Z", "2021-01-01T18:00:00.000Z"),
            this.CreateReservation("2020-01-01T08:00:00.000Z", "2020-01-01T18:00:00.000Z"),
            this.CreateReservation("2019-01-01T08:00:00.000Z", "2019-01-01T18:00:00.000Z")
        };

        await this.Context.Reservations.AddRangeAsync(reservations);
        await this.Context.SaveChangesAsync();

        // Act
        var (httpResponse, response) =
            await this._app.Member.GETAsync<P.Endpoint, P.Request, P.Response>(new P.Request());

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.expired.Count.Should().Be(3);
        response.active.Count.Should().Be(0);
        response.planned.Count.Should().Be(0);

        ValidateReservations(reservations, response.expired);

        // Cleanup
        this.Context.RemoveRange(reservations);
        await this.Context.SaveChangesAsync();
    }


    [Fact]
    public async Task Get_Request_ReturnsOk_And_ListOfActiveReservations()
    {
        // Arrange
        var reservations = new List<Reservation>
        {
            this.CreateReservation(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture),
                DateTime.UtcNow.AddHours(5).ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture)),
            this.CreateReservation(
                DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture),
                DateTime.UtcNow.AddHours(4).ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture)
            ),
            this.CreateReservation(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture),
                DateTime.UtcNow.AddHours(3).ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture))
        };

        await this.Context.Reservations.AddRangeAsync(reservations);
        await this.Context.SaveChangesAsync();

        // Act
        var (httpResponse, response) =
            await this._app.Member.GETAsync<P.Endpoint, P.Request, P.Response>(new P.Request());

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.expired.Count.Should().Be(0);
        response.active.Count.Should().Be(3);
        response.planned.Count.Should().Be(0);

        ValidateReservations(reservations, response.active);

        // Cleanup
        this.Context.RemoveRange(reservations);
        await this.Context.SaveChangesAsync();
    }


    [Fact]
    public async Task Get_Request_ReturnsOk_And_ListOfPlannedReservations()
    {
        // Arrange
        var reservations = new List<Reservation>
        {
            this.CreateReservation("2025-01-01T08:00:00.000Z", "2025-01-01T18:00:00.000Z"),
            this.CreateReservation("2026-01-01T08:00:00.000Z", "2026-01-01T18:00:00.000Z"),
            this.CreateReservation("2027-01-01T08:00:00.000Z", "2027-01-01T18:00:00.000Z")
        };

        await this.Context.Reservations.AddRangeAsync(reservations);
        await this.Context.SaveChangesAsync();

        // Act
        var (httpResponse, response) =
            await this._app.Member.GETAsync<P.Endpoint, P.Request, P.Response>(new P.Request());

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.expired.Count.Should().Be(0);
        response.active.Count.Should().Be(0);
        response.planned.Count.Should().Be(3);

        ValidateReservations(reservations, response.planned);

        // Cleanup
        this.Context.RemoveRange(reservations);
        await this.Context.SaveChangesAsync();
    }


    [Fact]
    public async Task Get_Request_ReturnsOk_And_EmptyList()
    {
        // Act
        var (httpResponse, response) =
            await this._app.Member.GETAsync<P.Endpoint, P.Request, P.Response>(new P.Request());

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.expired.Count.Should().Be(0);
        response.active.Count.Should().Be(0);
        response.planned.Count.Should().Be(0);
    }


    [Fact]
    public async Task Get_Personal_AsEmployee_Returns_Forbidden()
    {
        // Act
        var (httpResponse, _) = await this._app.Employee.GETAsync<P.Endpoint, P.Request, P.Response>(new P.Request());

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }


    [Fact]
    public async Task Get_Personal_AsAdmin_Returns_Forbidden()
    {
        // Act
        var (httpResponse, _) = await this._app.Admin.GETAsync<P.Endpoint, P.Request, P.Response>(new P.Request());

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
