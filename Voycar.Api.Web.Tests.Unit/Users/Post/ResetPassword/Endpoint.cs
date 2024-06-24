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
    private readonly IUsers FakeUserRepository = A.Fake<IUsers>();
    private readonly ILogger FakeLogger = A.Fake<ILogger<Endpoint>>();
    private readonly Request Request = new() { PasswordResetToken = "resetToken", Password = "newPassword" };


    private Features.Users.Endpoints.Post.ResetPassword.Endpoint SetupEndpoint()
    {
        return Factory.Create<Features.Users.Endpoints.Post.ResetPassword.Endpoint>(ctx =>
        {
            ctx.AddTestServices(s =>
            {
                s.AddSingleton(this.FakeUserRepository);
                s.AddSingleton(this.FakeLogger);
            });
        });
    }


    [Fact]
    public async Task Reset_Password_Successful_And_Return_Ok()
    {
        // Arrange
        var user = new User
        {
            Id = new Guid("47EB02FB-AA5B-4769-8B34-D23EC48DE506"),
            Email = "test@test.de",
            PasswordHash = "passwordHash",
            PasswordResetToken = "resetToken",
            ResetTokenExpires = new DateTime(DateTime.UtcNow.Ticks).AddDays(1)
        };

        var ep = this.SetupEndpoint();

        A.CallTo(() => this.FakeUserRepository.RetrieveByPasswordResetToken(this.Request.PasswordResetToken))
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
    public async Task Reset_Password_For_Invalid_User_Fails_And_Return_BadRequest()
    {
        // Arrange
        var ep = this.SetupEndpoint();

        A.CallTo(() => this.FakeUserRepository.RetrieveByPasswordResetToken(this.Request.PasswordResetToken))
            .Returns((User?)null);

        // Act
        var exception = await Assert.ThrowsAsync<ValidationFailureException>(() => ep.HandleAsync(this.Request, default));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal("ThrowError() called! - Token does not belong to any user", exception.Message);
    }


    [Fact]
    public async Task Reset_Password_For_Invalid_Reset_Token_Fails_And_Throws_ValidationsFailure()
    {
        // Arrange
        var user = new User
        {
            Id = new Guid("47EB02FB-AA5B-4769-8B34-D23EC48DE506"),
            Email = "test@test.de",
            PasswordHash = "passwordHash",
            PasswordResetToken = "resetToken",
            ResetTokenExpires = new DateTime(2023, 12, 12)
        };

        var ep = this.SetupEndpoint();

        A.CallTo(() => this.FakeUserRepository.RetrieveByPasswordResetToken(this.Request.PasswordResetToken))
            .Returns(user);

        // Act - local function
        async Task Act() => await ep.HandleAsync(this.Request, default);

        // Assert
        var exception = await Assert.ThrowsAnyAsync<ValidationFailureException>(Act);
        Assert.NotNull(exception);
        Assert.Equal("ThrowError() called! - Token does not belong to any user", exception.Message);
    }
}
