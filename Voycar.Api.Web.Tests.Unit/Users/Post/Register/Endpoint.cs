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
    private readonly IMembers FakeMemberRepository = A.Fake<IMembers>();
    private readonly IUsers FakeUserRepository = A.Fake<IUsers>();
    private readonly IRoles FakeRoleRepository = A.Fake<IRoles>();
    private readonly IEmailService FakeEmailService = A.Fake<IEmailService>();
    private readonly ILogger FakeLogger = A.Fake<ILogger<Endpoint>>();

    private readonly Request Request = new()
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
                s.AddSingleton(this.FakeUserRepository);
                s.AddSingleton(this.FakeMemberRepository);
                s.AddSingleton(this.FakeRoleRepository);
                s.AddSingleton(this.FakeEmailService);
                s.AddSingleton(this.FakeLogger);
            });
        });
    }


    [Fact]
    public async Task Register_New_User_Successful_And_Return_Ok()
    {
        // Arrange
        var fakeRole = new Role { Id = new Guid("4ECB35CC-906C-46D7-AB3B-EDB468E1DD51"), Name = "member" };

        var ep = this.SetupEndpoint();

        var member = ep.Map.ToEntity(this.Request);
        var user = new User { Email = this.Request.Email, PasswordHash = "hashedPassword" };

        A.CallTo(() => this.FakeUserRepository.RetrieveByEmail(this.Request.Email)).Returns((User?)null);
        A.CallTo(() => this.FakeRoleRepository.Retrieve(fakeRole.Name)).Returns(fakeRole);
        A.CallTo(() => this.FakeMemberRepository.Create(member)).Returns(member.Id);
        A.CallTo(() => this.FakeUserRepository.Create(user)).Returns(user.Id);
        A.CallTo(() => this.FakeEmailService.SendVerificationEmail(user)).DoesNothing();

        // Act
        await ep.HandleAsync(this.Request, default);
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
        var user = new User { Email = this.Request.Email, PasswordHash = "hashedPassword" };

        A.CallTo(() => this.FakeUserRepository.RetrieveByEmail(this.Request.Email)).Returns(user);

        // Act
        await ep.HandleAsync(this.Request, default);
        var rsp = ep.HttpContext.Response;

        // Assert
        Assert.NotNull(rsp);
        Assert.Equal(StatusCodes.Status400BadRequest, rsp.StatusCode);
    }
}
