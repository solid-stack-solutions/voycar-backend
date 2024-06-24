namespace Voycar.Api.Web.Tests.Unit.Users.Get.Verify;

using Entities;
using FakeItEasy;
using Features.Users.Endpoints.Get.Verify;
using Features.Users.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


public class Endpoint : TestBase<App>
{
    private readonly IUsers FakeUserRepository = A.Fake<IUsers>();
    private readonly ILogger FakeLogger = A.Fake<ILogger<Endpoint>>();
    private readonly Request Request = new() { VerificationToken = "randomToken" };


    private Features.Users.Endpoints.Get.Verify.Endpoint SetupEndpoint()
    {
        return Factory.Create<Features.Users.Endpoints.Get.Verify.Endpoint>(ctx =>
        {
            ctx.AddTestServices(s =>
            {
                s.AddSingleton(this.FakeUserRepository);
                s.AddSingleton(this.FakeLogger);
            });
        });
    }


    [Fact]
    public async Task Verify_User_Successful_And_Return_Ok()
    {
        // Arrange
        var user = new User
        {
            Id = new Guid("47EB02FB-AA5B-4769-8B34-D23EC48DE506"),
            Email = "test@test.de",
            PasswordHash = "passwordHash",
            VerificationToken = "randomToken",
            VerifiedAt = null,
        };

        var ep = this.SetupEndpoint();

        A.CallTo(() => this.FakeUserRepository.RetrieveByVerificationToken(this.Request.VerificationToken))
            .Returns(user);
        A.CallTo(() => this.FakeUserRepository.Update(user)).Returns(true);

        // Act
        await ep.HandleAsync(this.Request, default);
        var rsp = ep.HttpContext.Response;

        // Assert
        Assert.NotNull(rsp);
        Assert.Equal(StatusCodes.Status200OK, rsp.StatusCode);
    }


    [Fact]
    public async Task Verify_User_Fails_And_Returns_BadRequest()
    {
        // Arrange
        var ep = this.SetupEndpoint();
        A.CallTo(() => this.FakeUserRepository.RetrieveByVerificationToken(this.Request.VerificationToken))
            .Returns((User?)null);

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => ep.HandleAsync(this.Request, default));

    }
}
