namespace Voycar.Api.Web.Tests.Unit.Users.Get.WhoAmI;

using Entities;
using FakeItEasy;
using Features.Users.Endpoints.Get.WhoAmI;
using Features.Users.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;


public class Endpoint : TestBase<App>
{
    private readonly IUsers FakeUserRepository = A.Fake<IUsers>();
    private readonly Request Request = new();


    private Features.Users.Endpoints.Get.WhoAmI.Endpoint SetupEndpoint()
    {
        return Factory.Create<Features.Users.Endpoints.Get.WhoAmI.Endpoint>(ctx =>
        {
            ctx.AddTestServices(s =>
            {
                s.AddSingleton(this.FakeUserRepository);
            });
        });
    }


    [Fact]
    public async Task Get_Request_ReturnsOk()
    {
        // Arrange
        var user = new User
        {
            Id = new Guid("47EB02FB-AA5B-4769-8B34-D23EC48DE506"),
            Email = "test@test.de",
            PasswordHash = "passwordHash",
            VerificationToken = "randomToken",
            VerifiedAt = null
        };

        var ep = this.SetupEndpoint();

        A.CallTo(() => this.FakeUserRepository.Retrieve(user.Id)).Returns(user);

        // Act
        await ep.HandleAsync(this.Request, default);
        var rsp = ep.HttpContext.Response;

        // Assert
        Assert.NotNull(rsp);
        Assert.Equal(StatusCodes.Status200OK, rsp.StatusCode);
    }


    [Fact]
    public async Task Get_Request_Throws_ValidationsFailure_DueToNullUser()
    {
        // Arrange
        var ep = this.SetupEndpoint();
        A.CallTo(() => this.FakeUserRepository.Retrieve(this.Request.UserId)).Returns(null);

        // Act - local function
        async Task Act() => await ep.HandleAsync(this.Request, default);

        // Assert
        var exception = await Assert.ThrowsAnyAsync<ValidationFailureException>(Act);
        Assert.NotNull(exception);
        Assert.Equal("ThrowError() called! - User does not exist", exception.Message);
    }
}
