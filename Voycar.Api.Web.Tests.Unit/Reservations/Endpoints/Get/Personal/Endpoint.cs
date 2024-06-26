namespace Voycar.Api.Web.Tests.Unit.Reservations.Endpoints.Get.Personal;

using Entities;
using FakeItEasy;
using Features.Reservations.Endpoints.Get.Personal;
using Features.Reservations.Repository;
using Features.Users.Repository;
using Microsoft.Extensions.DependencyInjection;


public class Endpoint : TestBase<App>
{
    private readonly IReservations FakeReservationsRepository = A.Fake<IReservations>();
    private readonly IUsers FakeUserRepository = A.Fake<IUsers>();


    private readonly Request Request = new();


    private Features.Reservations.Endpoints.Get.Personal.Endpoint SetupEndpoint()
    {
        return Factory.Create<Features.Reservations.Endpoints.Get.Personal.Endpoint>(ctx =>
        {
            ctx.AddTestServices(s =>
            {
                s.AddSingleton(this.FakeReservationsRepository);
                s.AddSingleton(this.FakeUserRepository);
            });
        });
    }


    [Fact]
    public async Task Get_Request_Throws_ValidationsFailure_DueToNullUser()
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
    public async Task Get_Request_Throws_ValidationsFailure_DueToNullMember()
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
}
