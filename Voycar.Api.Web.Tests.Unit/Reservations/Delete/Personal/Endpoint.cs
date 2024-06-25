namespace Voycar.Api.Web.Tests.Unit.Reservations.Delete.Personal;

using System.Globalization;
using Entities;
using FakeItEasy;
using Features.Reservations.Endpoints.Delete.Personal;
using Features.Reservations.Repository;
using Features.Users.Repository;
using Microsoft.Extensions.DependencyInjection;


public class Endpoint : TestBase<App>
{
    private readonly IReservations FakeReservationsRepository = A.Fake<IReservations>();
    private readonly IUsers FakeUserRepository = A.Fake<IUsers>();


    private readonly Request Request = new() { Id = new Guid("12246146-BB17-4379-83E7-84B9275E3BCD") };


    private Features.Reservations.Endpoints.Delete.Personal.Endpoint SetupEndpoint()
    {
        return Factory.Create<Features.Reservations.Endpoints.Delete.Personal.Endpoint>(ctx =>
        {
            ctx.AddTestServices(s =>
            {
                s.AddSingleton(this.FakeReservationsRepository);
                s.AddSingleton(this.FakeUserRepository);
            });
        });
    }


    [Fact]
    public async Task Put_Request_Throws_ValidationsFailure_DueToNullUser()
    {
        // Arrange
        var ep = this.SetupEndpoint();

        A.CallTo(() => this.FakeUserRepository.Retrieve(this.Request.UserId)).Returns(null);

        // Act
        async Task Act() => await ep.HandleAsync(this.Request, default);

        // Assert
        var exception = await Assert.ThrowsAnyAsync<ValidationFailureException>(Act);
        Assert.NotNull(exception);
        Assert.Equal("ThrowError() called! - User does not exist", exception.Message);
    }


    [Fact]
    public async Task Put_Request_Throws_ValidationsFailure_DueToNullMember()
    {
        // Arrange
        var ep = this.SetupEndpoint();
        var user = new User { Email = "test@test.de", PasswordHash = "notsafe987", MemberId = null };


        A.CallTo(() => this.FakeUserRepository.Retrieve(this.Request.UserId)).Returns(user);

        // Act
        async Task Act() => await ep.HandleAsync(this.Request, default);

        // Assert
        var exception = await Assert.ThrowsAnyAsync<ValidationFailureException>(Act);
        Assert.NotNull(exception);
        Assert.Equal("ThrowError() called! - Member does not exist", exception.Message);
    }


    [Fact]
    public async Task Put_Request_Throws_ValidationsFailure_DueToDeletionError()
    {
        // Arrange
        var ep = this.SetupEndpoint();
        var user = new User
        {
            Email = "test@test.de",
            PasswordHash = "notsafe987",
            MemberId = new Guid("860EA6EF-9931-435C-A78F-9B8A75419C01")
        };

        var reservation = new Reservation
        {
            Id = new Guid("C0189910-9670-41E8-A01B-F4C967AF8204"),
            Begin = DateTime.Parse("2025-01-01T08:00:00.000Z", CultureInfo.InvariantCulture).ToUniversalTime(),
            End = DateTime.Parse("2025-01-01T18:00:00.000Z", CultureInfo.InvariantCulture).ToUniversalTime(),
            MemberId = new Guid("860EA6EF-9931-435C-A78F-9B8A75419C01")
        };


        A.CallTo(() => this.FakeUserRepository.Retrieve(this.Request.UserId)).Returns(user);
        A.CallTo(() => this.FakeReservationsRepository.Retrieve(this.Request.Id)).Returns(reservation);
        A.CallTo(() => this.FakeReservationsRepository.Delete(this.Request.Id)).Returns(false);

        // Act
        async Task Act() => await ep.HandleAsync(this.Request, default);

        // Assert
        var exception = await Assert.ThrowsAnyAsync<ValidationFailureException>(Act);
        Assert.NotNull(exception);
        Assert.Equal("ThrowError() called! - Failed to delete reservation", exception.Message);
    }
}
