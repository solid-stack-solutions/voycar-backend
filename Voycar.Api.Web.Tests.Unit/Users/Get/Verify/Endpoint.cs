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
    [Fact]
    public async Task VerifyUserSuccessful()
    {
        // Arrange
        var fakeUserRepository = A.Fake<IUsers>();
        var fakeLogger = A.Fake<ILogger<Endpoint>>();

        var req = new Request{VerificationToken = "randomToken"};
        var user = new User
        {
            Id = new Guid("47EB02FB-AA5B-4769-8B34-D23EC48DE506"),
            Email = "test@test.de",
            PasswordHash = "passwordHash",
            VerificationToken = "randomToken",
            VerifiedAt = null,
        };

        var ep = Factory.Create<Features.Users.Endpoints.Get.Verify.Endpoint>(ctx =>
        {
            ctx.AddTestServices(s =>
            {
                s.AddSingleton(fakeUserRepository);
                s.AddSingleton(fakeLogger);
            });
        });

        A.CallTo(() => fakeUserRepository.RetrieveByVerificationToken(req.VerificationToken)).Returns(user);
        A.CallTo(() => fakeUserRepository.Update(user)).Returns(true);

        // Act
        await ep.HandleAsync(req, default);
        var rsp = ep.HttpContext.Response;

        // Assert
        Assert.NotNull(rsp);
        Assert.Equal(StatusCodes.Status200OK, rsp.StatusCode);
    }


    [Fact]
    public async Task VerifyUserFailure()
    {
        // Arrange
        var fakeUserRepository = A.Fake<IUsers>();
        var fakeLogger = A.Fake<ILogger<Post.Register.Endpoint>>();

        var req = new Request{VerificationToken = "randomToken"};

        var ep = Factory.Create<Features.Users.Endpoints.Get.Verify.Endpoint>(ctx =>
        {
            ctx.AddTestServices(s =>
            {
                s.AddSingleton(fakeUserRepository);
                s.AddSingleton(fakeLogger);
            });
        });

        A.CallTo(() => fakeUserRepository.RetrieveByVerificationToken(req.VerificationToken)).Returns((User?)(null));

        // Act
        await ep.HandleAsync(req, default);
        var rsp = ep.HttpContext.Response;

        // Assert
        Assert.NotNull(rsp);
        Assert.Equal(StatusCodes.Status400BadRequest, rsp.StatusCode);
    }
}
