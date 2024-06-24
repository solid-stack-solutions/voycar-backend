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
    private readonly IUsers FakeUserRepository = A.Fake<IUsers>();
    private readonly IEmailService FakeEmailService = A.Fake<IEmailService>();
    private readonly ILogger FakeLogger = A.Fake<ILogger<Endpoint>>();
    private readonly Request Request = new() { Email = "test@test.de" };


    private Features.Users.Endpoints.Post.ResetToken.Endpoint SetupEndpoint()
    {
        return Factory.Create<Features.Users.Endpoints.Post.ResetToken.Endpoint>(ctx =>
        {
            ctx.AddTestServices(s =>
            {
                s.AddSingleton(this.FakeUserRepository);
                s.AddSingleton(this.FakeEmailService);
                s.AddSingleton(this.FakeLogger);
            });
        });
    }


    [Fact]
    public async Task Create_ResetToken_Successful_And_Return_Ok()
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

        A.CallTo(() => this.FakeUserRepository.RetrieveByEmail(this.Request.Email)).Returns(user);
        A.CallTo(() => this.FakeUserRepository.Update(user)).Returns(true);

        // Act
        await ep.HandleAsync(this.Request, default);
        var rsp = ep.HttpContext.Response;

        // Assert
        Assert.NotNull(rsp);
        Assert.Equal(StatusCodes.Status200OK, rsp.StatusCode);
    }


    [Fact]
    public async Task Create_ResetToken_Fails_And_Throws_ValidationsFailure()
    {
        // Arrange
        var ep = this.SetupEndpoint();

        A.CallTo(() => this.FakeUserRepository.RetrieveByEmail(this.Request.Email)).Returns((User?)null);

        // Act - local function
        async Task Act() => await ep.HandleAsync(this.Request, default);

        // Assert
        var exception = await Assert.ThrowsAnyAsync<ValidationFailureException>(Act);
        Assert.NotNull(exception);
        Assert.Equal("ThrowError() called! - User not found", exception.Message);
    }
}
