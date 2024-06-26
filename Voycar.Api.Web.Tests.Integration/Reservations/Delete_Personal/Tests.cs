namespace Voycar.Api.Web.Tests.Integration.Reservations.Delete_Personal;

using System.Globalization;
using Context;
using Entities;
using Microsoft.EntityFrameworkCore;
using P = Features.Reservations.Endpoints.Delete.Personal;


public sealed class State : StateFixture
{
    public Guid MemberId { get; set; }
    public Guid CarId { get; set; }

    public Guid PlanId { get; set; }
    public const string PlanName = "basic";
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
        this._state.PlanId = this.Context.Plans.First(p => p.Name == State.PlanName).Id;
    }


    private Reservation CreateReservation(string begin, string end)
    {
        return new Reservation
        {
            Id = new Guid("BAEF195B-A808-448A-BCB0-C0E83DCCF128"),
            Begin = DateTime.Parse(begin, CultureInfo.InvariantCulture).ToUniversalTime(),
            End = DateTime.Parse(end, CultureInfo.InvariantCulture).ToUniversalTime(),
            MemberId = this._state.MemberId,
            CarId = this._state.CarId
        };
    }


    [Fact]
    public async Task Delete_Request_ReturnsOk_And_RemovesReservationFromDb()
    {
        // Arrange
        var reservation = this.CreateReservation("2025-01-01T08:00:00.000Z", "2025-01-01T18:00:00.000Z");
        await this.Context.Reservations.AddAsync(reservation);
        await this.Context.SaveChangesAsync();

        var request = new P.Request { Id = reservation.Id };

        // Act
        var httpResponse = await this._app.Member.DeleteAsync($"/reservation/personal/{request.Id}");

        // Arrange assertion
        var reservationFromDb = await this.Context.Reservations.FirstOrDefaultAsync(res => res.Id == request.Id);

        // Assert
        reservationFromDb.Should().BeNull();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }


    [Fact]
    public async Task Delete_Request_ReturnsBadRequest_DueToExpiredReservation()
    {
        // Arrange
        var reservation = this.CreateReservation("2022-01-01T08:00:00.000Z", "2022-01-01T18:00:00.000Z");
        await this.Context.Reservations.AddAsync(reservation);
        await this.Context.SaveChangesAsync();

        var request = new P.Request { Id = reservation.Id };

        // Act
        var httpResponse = await this._app.Member.DeleteAsync($"/reservation/personal/{request.Id}");

        // Arrange assertion
        var reservationFromDb = await this.Context.Reservations.FirstOrDefaultAsync(res => res.Id == request.Id);

        // Assert
        reservationFromDb.Should().NotBeNull();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Cleanup
        this.Context.Reservations.Remove(reservation);
        await this.Context.SaveChangesAsync();
    }


    [Fact]
    public async Task Delete_Request_ReturnsBadRequest_DueToActiveReservation()
    {
        // Arrange
        var reservation =
            this.CreateReservation(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture),
                DateTime.UtcNow.AddHours(3).ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture));
        await this.Context.Reservations.AddAsync(reservation);
        await this.Context.SaveChangesAsync();

        var request = new P.Request { Id = reservation.Id };

        // Act
        var httpResponse = await this._app.Member.DeleteAsync($"/reservation/personal/{request.Id}");

        // Arrange assertion
        var reservationFromDb = await this.Context.Reservations.FirstOrDefaultAsync(res => res.Id == request.Id);

        // Assert
        reservationFromDb.Should().NotBeNull();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Cleanup
        this.Context.Reservations.Remove(reservation);
        await this.Context.SaveChangesAsync();
    }


    [Fact]
    public async Task Delete_Request_ReturnsBadRequest_DueToNonExistingReservation()
    {
        // Arrange
        var request = new P.Request { Id = new Guid("D0D4ED04-05A5-4B8D-8E69-8FDFDA1DD83A") };

        // Act
        var httpResponse = await this._app.Member.DeleteAsync($"/reservation/personal/{request.Id}");

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Delete_Request_ReturnsBadRequest_DueToDifferentIds()
    {
        // Arrange
        // Add member to database
        var member = new Member
        {
            Id = new Guid("6102DC6F-BDA7-4848-A71C-EE58B8208F3C"),
            FirstName = "null",
            LastName = "null",
            Street = "null",
            HouseNumber = "null",
            PostalCode = "null",
            City = "null",
            Country = "null",
            BirthDate = new DateOnly(2000,12,12),
            BirthPlace = "null",
            PhoneNumber = "null",
            PlanId = this._state.PlanId
        };
        await this.Context.Members.AddAsync(member);
        await this.Context.SaveChangesAsync();

        // Add reservation to database
        var reservation = this.CreateReservation("2025-01-01T08:00:00.000Z", "2025-01-01T18:00:00.000Z");
        reservation.MemberId = member.Id; // Change Id
        await this.Context.Reservations.AddAsync(reservation);
        await this.Context.SaveChangesAsync();

        var request = new P.Request { Id = reservation.Id };

        // Act
        var httpResponse = await this._app.Member.DeleteAsync($"/reservation/personal/{request.Id}");

        // Arrange assertion
        var reservationFromDb = await this.Context.Reservations.FirstOrDefaultAsync(res => res.Id == request.Id);

        // Assert
        reservationFromDb.Should().NotBeNull();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Cleanup
        this.Context.Reservations.Remove(reservation);
        this.Context.Members.Remove(member);
        await this.Context.SaveChangesAsync();
    }


    [Fact]
    public async Task Delete_Request_AsEmployee_Returns_Forbidden()
    {
        // Arrange
        var reservation = this.CreateReservation("2022-01-01T08:00:00.000Z", "2022-01-01T18:00:00.000Z");
        await this.Context.Reservations.AddAsync(reservation);
        await this.Context.SaveChangesAsync();

        var request = new P.Request { Id = reservation.Id };

        // Act
        var httpResponse = await this._app.Employee.DeleteAsync($"/reservation/personal/{request.Id}");

        // Arrange assertion
        var reservationFromDb = await this.Context.Reservations.FirstOrDefaultAsync(res => res.Id == request.Id);

        // Assert
        reservationFromDb.Should().NotBeNull();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        // Cleanup
        this.Context.Reservations.Remove(reservation);
        await this.Context.SaveChangesAsync();
    }


    [Fact]
    public async Task Delete_Request_AsAdmin_Returns_Forbidden()
    {
        // Arrange
        var reservation = this.CreateReservation("2022-01-01T08:00:00.000Z", "2022-01-01T18:00:00.000Z");
        await this.Context.Reservations.AddAsync(reservation);
        await this.Context.SaveChangesAsync();

        var request = new P.Request { Id = reservation.Id };

        // Act
        var httpResponse = await this._app.Admin.DeleteAsync($"/reservation/personal/{request.Id}");

        // Arrange assertion
        var reservationFromDb = await this.Context.Reservations.FirstOrDefaultAsync(res => res.Id == request.Id);

        // Assert
        reservationFromDb.Should().NotBeNull();
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        // Cleanup
        this.Context.Reservations.Remove(reservation);
        await this.Context.SaveChangesAsync();
    }
}
