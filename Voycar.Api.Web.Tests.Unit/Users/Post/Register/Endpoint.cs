namespace Voycar.Api.Web.Tests.Unit.Users.Post.Register;

using FakeItEasy;
using Features.Members.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Entities;
using Features.Plans.Repository;
using Voycar.Api.Web.Features.Roles.Repository;
using Voycar.Api.Web.Features.Users.Endpoints.Post.Register;
using Voycar.Api.Web.Features.Users.Repository;
using Service;


public class Endpoint : TestBase<App>
{
    private readonly IMembers FakeMemberRepository = A.Fake<IMembers>();
    private readonly IUsers FakeUserRepository = A.Fake<IUsers>();
    private readonly IRoles FakeRoleRepository = A.Fake<IRoles>();
    private readonly IPlans FakePlanRepository = A.Fake<IPlans>();
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
                s.AddSingleton(this.FakePlanRepository);
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
        var fakePlan = new Plan
        {
            Id = new Guid("86D7F973-ECBC-4B1A-A959-EAF4C86E6237"),
            Name = "basic",
        };

        var ep = this.SetupEndpoint();

        var member = ep.Map.ToEntity(this.Request);
        var user = new User { Email = this.Request.Email, PasswordHash = "hashedPassword" };

        A.CallTo(() => this.FakeUserRepository.RetrieveByEmail(this.Request.Email)).Returns((User?)null);
        A.CallTo(() => this.FakeRoleRepository.Retrieve(fakeRole.Name)).Returns(fakeRole);
        A.CallTo(() => this.FakePlanRepository.Retrieve(fakeRole.Id)).Returns(fakePlan);
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
    public async Task Register_ExistingUser_Throws_ValidationsFailure()
    {
        // Arrange
        var ep = this.SetupEndpoint();
        var user = new User { Email = this.Request.Email, PasswordHash = "hashedPassword" };

        A.CallTo(() => this.FakeUserRepository.RetrieveByEmail(this.Request.Email)).Returns(user);

        // Act - local function
        async Task Act() => await ep.HandleAsync(this.Request, default);

        // Assert
        var exception = await Assert.ThrowsAnyAsync<ValidationFailureException>(Act);
        Assert.NotNull(exception);
        Assert.Equal("ThrowError() called! - User already exists", exception.Message);
    }
}
