namespace Voycar.Api.Web.Tests.Unit.Users.Post.Register;

using FakeItEasy;
using Features.Members.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Entities;
using Voycar.Api.Web.Features.Roles.Repository;
using Voycar.Api.Web.Features.Users.Endpoints.Post.Register;
using Voycar.Api.Web.Features.Users.Repository;
using Service;


public class Endpoint : TestBase<App>
{
    [Fact]
    public async Task Register_New_User_Successful_And_Return_Ok()
    {
        // Arrange
        var fakeMemberRepository = A.Fake<IMembers>();
        var fakeUserRepository = A.Fake<IUsers>();
        var fakeRoleRepository = A.Fake<IRoles>();
        var fakeEmailService = A.Fake<IEmailService>();
        var fakeLogger = A.Fake<ILogger<Endpoint>>();

        var fakeRole = new Role { Id = new Guid("4ECB35CC-906C-46D7-AB3B-EDB468E1DD51"), Name = "member" };
        var req = new Request
        {
            Email = "test@test.de",
            Password = "notsafe987",
            FirstName = "testFName",
            LastName = "testLName",
            Street = "test",
            HouseNumber = "test",
            PostalCode = "test",
            City = "test",
            Country = "test",
            BirthDate = new DateOnly(2000, 12, 12),
            BirthPlace = "test",
            PhoneNumber = "test"
        };

        var mem = new Member
        {
            Id = new Guid("E5180CD2-A399-4EE3-AAE3-D909FA25AFF3"),
            FirstName = "testFName",
            LastName = "testLName",
            Street = "test",
            HouseNumber = "test",
            PostalCode = "test",
            City = "test",
            Country = "test",
            BirthDate = new DateOnly(2000, 12, 12),
            BirthPlace = "test",
            PhoneNumber = "test"
        };

        var ep = Factory.Create<Features.Users.Endpoints.Post.Register.Endpoint>(ctx =>
        {
            ctx.AddTestServices(s =>
            {
                s.AddSingleton(fakeUserRepository);
                s.AddSingleton(fakeMemberRepository);
                s.AddSingleton(fakeRoleRepository);
                s.AddSingleton(fakeEmailService);
                s.AddSingleton(fakeLogger);
            });
        });

        var member = ep.Map.ToEntity(req);
        var user = new User { Email = req.Email, PasswordHash = "hashedPassword"};

        A.CallTo(() => fakeUserRepository.RetrieveByEmail(req.Email.ToLowerInvariant())).Returns((User?)null);
        A.CallTo(() => fakeRoleRepository.Retrieve(fakeRole.Name)).Returns(fakeRole);
        A.CallTo(() => fakeMemberRepository.Create(member)).Returns(member.Id);
        A.CallTo(() => fakeUserRepository.Create(user)).Returns(user.Id);
        A.CallTo(() => fakeEmailService.SendVerificationEmail(user)).DoesNothing();

        // Act
        await ep.HandleAsync(req, default);
        var rsp = ep.HttpContext.Response;

        // Assert
        Assert.NotNull(rsp);
        Assert.Equal(StatusCodes.Status200OK, rsp.StatusCode);
    }


    [Fact]
    public async Task Register_ExistingUser_Returns_BadRequest()
    {
        // Arrange
        var fakeMemberRepository = A.Fake<IMembers>();
        var fakeUserRepository = A.Fake<IUsers>();
        var fakeRoleRepository = A.Fake<IRoles>();
        var fakeEmailService = A.Fake<IEmailService>();
        var fakeLogger = A.Fake<ILogger<Endpoint>>();

        var req = new Request
        {
            Email = "test@test.de",
            Password = "notsafe987",
            FirstName = "testFName",
            LastName = "testLName",
            Street = "test",
            HouseNumber = "test",
            PostalCode = "test",
            City = "test",
            Country = "test",
            BirthDate = new DateOnly(2000, 12, 12),
            BirthPlace = "test",
            PhoneNumber = "test"
        };


        var ep = Factory.Create<Features.Users.Endpoints.Post.Register.Endpoint>(ctx =>
        {
            ctx.AddTestServices(s =>
            {
                s.AddSingleton(fakeUserRepository);
                s.AddSingleton(fakeMemberRepository);
                s.AddSingleton(fakeRoleRepository);
                s.AddSingleton(fakeEmailService);
                s.AddSingleton(fakeLogger);
            });
        });

        var user = new User { Email = req.Email, PasswordHash = "hashedPassword" };

        A.CallTo(() => fakeUserRepository.RetrieveByEmail(req.Email.ToLowerInvariant())).Returns(user);

        // Act
        await ep.HandleAsync(req, default);
        var rsp = ep.HttpContext.Response;

        // Assert
        Assert.NotNull(rsp);
        Assert.Equal(StatusCodes.Status400BadRequest, rsp.StatusCode);
    }
}
