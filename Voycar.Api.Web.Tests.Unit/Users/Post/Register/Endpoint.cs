namespace Voycar.Api.Web.Tests.Unit.Users.Post.Register;

using Bogus.DataSets;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Voycar.Api.Web.Entities;
using Voycar.Api.Web.Features.Members.Repository;
using Voycar.Api.Web.Features.Roles.Repository;
using Voycar.Api.Web.Features.Users.Endpoints.Post.Register;
using Voycar.Api.Web.Features.Users.Repository;
using Voycar.Api.Web.Service;


public class Endpoint : TestBase<App>
{
    [Fact]
    public async Task RegisterUserSuccessful()
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
            BirthDate = new DateOnly(2000,12,12),
            BirthPlace = "test",
            PhoneNumber = "test"
        };

        var ep = Factory.Create<Features.Users.Endpoints.Post.Register.Endpoint>(fakeUserRepository,
            fakeMemberRepository, fakeRoleRepository, fakeEmailService, fakeLogger);

        var member = ep.Map.ToEntity(req);
        var user = new User
        {
            Email = req.Email,
            PasswordHash = "hashedPassword",
            VerificationToken = "verificationToken",
            RoleId = fakeRoleRepository.Retrieve(fakeRole.Name).Result!.Id
        };


        A.CallTo(() => fakeUserRepository.Retrieve("email", req.Email)).Returns((User?)null);
        A.CallTo(() => fakeRoleRepository.Retrieve(fakeRole.Name)).Returns(fakeRole);
        A.CallTo(() => fakeMemberRepository.Create(member)).Returns(member.Id);
        A.CallTo(() => fakeUserRepository.Create(user)).Returns(user.Id);











        // Act
        // Assert
    }
}
