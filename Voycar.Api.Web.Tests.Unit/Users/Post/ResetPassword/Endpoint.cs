namespace Voycar.Api.Web.Tests.Unit.Users.Post.ResetPassword;

using Entities;
using FakeItEasy;
using Features.Users.Endpoints.Post.ResetPassword;
using Features.Users.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


public class Endpoint : TestBase<App>
{
    [Fact]
    public async Task ResetPasswordSuccessful()
    {
        // Arrange
        var fakeUserRepository = A.Fake<IUsers>();
        var fakeLogger = A.Fake<ILogger<Endpoint>>();

        var req = new Request{PasswordResetToken = "resetToken", Password = "newPassword"};
        var user = new User
        {
            Id = new Guid("47EB02FB-AA5B-4769-8B34-D23EC48DE506"),
            Email = "test@test.de",
            PasswordHash = "passwordHash",
            PasswordResetToken = "resetToken",
            ResetTokenExpires = new DateTime(DateTime.UtcNow.Ticks)
        };

        var ep = Factory.Create<Features.Users.Endpoints.Post.ResetPassword.Endpoint>(ctx =>
        {
            ctx.AddTestServices(s =>
            {
                s.AddSingleton(fakeUserRepository);
                s.AddSingleton(fakeLogger);
            });
        });

        A.CallTo(() => fakeUserRepository.RetrieveByVerificationToken(req.PasswordResetToken)).Returns(user);
        A.CallTo(() => fakeUserRepository.Update(user)).Returns(true);

        // Act
        await ep.HandleAsync(req, default);
        var rsp = ep.HttpContext.Response;

        // Assert
        Assert.NotNull(rsp);
        Assert.Equal(StatusCodes.Status200OK, rsp.StatusCode);
    }


    [Fact]
    public async Task ResetPasswordFailure_InvalidUser()
    {
        // Arrange
        var fakeUserRepository = A.Fake<IUsers>();
        var fakeLogger = A.Fake<ILogger<Post.Register.Endpoint>>();

        var req = new Request{PasswordResetToken = "resetToken", Password = "newPassword"};


        var ep = Factory.Create<Features.Users.Endpoints.Post.ResetPassword.Endpoint>(ctx =>
        {
            ctx.AddTestServices(s =>
            {
                s.AddSingleton(fakeUserRepository);
                s.AddSingleton(fakeLogger);
            });
        });

        A.CallTo(() => fakeUserRepository.RetrieveByPasswordResetToken(req.PasswordResetToken)).Returns((User?)null);

        // Act
        await ep.HandleAsync(req, default);
        var rsp = ep.HttpContext.Response;

        // Assert
        Assert.NotNull(rsp);
        Assert.Equal(StatusCodes.Status400BadRequest, rsp.StatusCode);
    }


    [Fact]
    public async Task ResetPasswordFailure_ResetResetToken()
    {
        // Arrange
        var fakeUserRepository = A.Fake<IUsers>();
        var fakeLogger = A.Fake<ILogger<Post.Register.Endpoint>>();

        var req = new Request{PasswordResetToken = "resetToken", Password = "newPassword"};
        var user = new User
        {
            Id = new Guid("47EB02FB-AA5B-4769-8B34-D23EC48DE506"),
            Email = "test@test.de",
            PasswordHash = "passwordHash",
            PasswordResetToken = "resetToken",
            ResetTokenExpires = new DateTime(2023,12,12)
        };

        var ep = Factory.Create<Features.Users.Endpoints.Post.ResetPassword.Endpoint>(ctx =>
        {
            ctx.AddTestServices(s =>
            {
                s.AddSingleton(fakeUserRepository);
                s.AddSingleton(fakeLogger);
            });
        });

        A.CallTo(() => fakeUserRepository.RetrieveByPasswordResetToken(req.PasswordResetToken)).Returns(user);

        // Act
        await ep.HandleAsync(req, default);
        var rsp = ep.HttpContext.Response;

        // Assert
        Assert.NotNull(rsp);
        Assert.Equal(StatusCodes.Status400BadRequest, rsp.StatusCode);
    }
}
