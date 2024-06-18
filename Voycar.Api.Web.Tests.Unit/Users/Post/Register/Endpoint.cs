namespace Voycar.Api.Web.Tests.Unit.Users.Post.Register;

using FakeItEasy;
using Features.Members.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Entities;
using Voycar.Api.Web.Features.Roles.Repository;
using Voycar.Api.Web.Features.Users.Endpoints.Post.Register;
using Voycar.Api.Web.Features.Users.Repository;
using Service;

public class Endpoint : TestBase<App>
{
    private readonly IMembers fakeMemberRepository = A.Fake<IMembers>();
    private readonly IUsers fakeUserRepository = A.Fake<IUsers>();
    private readonly IRoles fakeRoleRepository = A.Fake<IRoles>();
    private readonly IEmailService fakeEmailService = A.Fake<IEmailService>();
    private readonly ILogger fakeLogger = A.Fake<ILogger<Endpoint>>();

    private readonly Request request = new()
    {
        Email = "test@test.de",
        Password = "notsafe987",
        BirthDate = new DateOnly(2000, 12, 12),
    };

    private Features.Users.Endpoints.Post.Register.Endpoint SetupEndpoint()
    {
        return Factory.Create<Features.Users.Endpoints.Post.Register.Endpoint>(ctx =>
        {
            ctx.AddTestServices(s =>
            {
                s.AddSingleton(this.fakeUserRepository);
                s.AddSingleton(this.fakeMemberRepository);
                s.AddSingleton(this.fakeRoleRepository);
                s.AddSingleton(this.fakeEmailService);
                s.AddSingleton(this.fakeLogger);
            });
        });
    }


    [Fact]
    public async Task Register_New_User_Successful_And_Return_Ok()
    {
        // Arrange
        var fakeRole = new Role { Id = new Guid("4ECB35CC-906C-46D7-AB3B-EDB468E1DD51"), Name = "member" };

        var ep = this.SetupEndpoint();

        var member = ep.Map.ToEntity(this.request);
        var user = new User { Email = this.request.Email, PasswordHash = "hashedPassword" };

        A.CallTo(() => this.fakeUserRepository.RetrieveByEmail(this.request.Email)).Returns((User?)null);
        A.CallTo(() => this.fakeRoleRepository.Retrieve(fakeRole.Name)).Returns(fakeRole);
        A.CallTo(() => this.fakeMemberRepository.Create(member)).Returns(member.Id);
        A.CallTo(() => this.fakeUserRepository.Create(user)).Returns(user.Id);
        A.CallTo(() => this.fakeEmailService.SendVerificationEmail(user)).DoesNothing();

        // Act
        await ep.HandleAsync(this.request, default);
        var rsp = ep.HttpContext.Response;

        // Assert
        Assert.NotNull(rsp);
        Assert.Equal(StatusCodes.Status200OK, rsp.StatusCode);
    }


    [Fact]
    public async Task Register_ExistingUser_Returns_BadRequest()
    {
        // Arrange
        var ep = this.SetupEndpoint();
        var user = new User { Email = this.request.Email, PasswordHash = "hashedPassword" };

        A.CallTo(() => this.fakeUserRepository.RetrieveByEmail(this.request.Email)).Returns(user);

        // Act
        await ep.HandleAsync(this.request, default);
        var rsp = ep.HttpContext.Response;

        // Assert
        Assert.NotNull(rsp);
        Assert.Equal(StatusCodes.Status400BadRequest, rsp.StatusCode);
    }
}
