namespace Voycar.Api.Web.Tests.Unit.Users.Post.ResetToken;

using System.Security.Cryptography;
using Entities;
using FakeItEasy;
using Features.Users.Endpoints.Post.ResetToken;
using Features.Users.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Service;


public class Endpoint : TestBase<App>
{
    [Fact]
    public async Task ResetTokenSuccessful()
    {
        // Arrange
        var fakeUserRepository = A.Fake<IUsers>();
        var fakeEmailService = A.Fake<IEmailService>();
        var fakeLogger = A.Fake<ILogger<Endpoint>>();

        var req = new Request { Email = "test@test.de" };
        var user = new User
        {
            Id = new Guid("47EB02FB-AA5B-4769-8B34-D23EC48DE506"),
            Email = "test@test.de",
            PasswordHash = "passwordHash",
            PasswordResetToken = "resetToken",
            ResetTokenExpires = new DateTime(DateTime.UtcNow.Ticks).AddDays(1)
        };

        var ep = Factory.Create<Features.Users.Endpoints.Post.ResetToken.Endpoint>(ctx =>
        {
            ctx.AddTestServices(s =>
            {
                s.AddSingleton(fakeUserRepository);
                s.AddSingleton(fakeEmailService);
                s.AddSingleton(fakeLogger);
            });
        });

        A.CallTo(() => fakeUserRepository.RetrieveByEmail(req.Email)).Returns(user);
        A.CallTo(() => fakeUserRepository.Update(user)).Returns(true);

        // Act
        await ep.HandleAsync(req, default);
        var rsp = ep.HttpContext.Response;

        // Assert
        Assert.NotNull(rsp);
        Assert.Equal(StatusCodes.Status200OK, rsp.StatusCode);
    }


    [Fact]
    public async Task ResetTokenFailure()
    {
        // Arrange
        var fakeUserRepository = A.Fake<IUsers>();
        var fakeEmailService = A.Fake<IEmailService>();
        var fakeLogger = A.Fake<ILogger<Endpoint>>();

        var req = new Request { Email = "test@test.de" };

        var ep = Factory.Create<Features.Users.Endpoints.Post.ResetToken.Endpoint>(ctx =>
        {
            ctx.AddTestServices(s =>
            {
                s.AddSingleton(fakeUserRepository);
                s.AddSingleton(fakeEmailService);
                s.AddSingleton(fakeLogger);
            });
        });

        A.CallTo(() => fakeUserRepository.RetrieveByEmail(req.Email)).Returns((User?)null);

        // Act
        await ep.HandleAsync(req, default);
        var rsp = ep.HttpContext.Response;

        // Assert
        Assert.NotNull(rsp);
        Assert.Equal(StatusCodes.Status400BadRequest, rsp.StatusCode);
    }
}
