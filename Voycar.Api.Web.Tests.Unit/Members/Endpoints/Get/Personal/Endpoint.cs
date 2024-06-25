namespace Voycar.Api.Web.Tests.Unit.Members.Endpoints.Get.Personal;

using Entities;
using FakeItEasy;
using Features.Members.Endpoints.Get.Personal;
using Features.Members.Repository;
using Features.Users.Repository;
using Microsoft.Extensions.DependencyInjection;


public class Endpoint : TestBase<App>
{
    private readonly IUsers FakeUserRepository = A.Fake<IUsers>();
    private readonly IMembers FakeMemberRepository = A.Fake<IMembers>();

    private readonly Request Request = new();


    private Features.Members.Endpoints.Get.Personal.Endpoint SetupEndpoint()
    {
        return Factory.Create<Features.Members.Endpoints.Get.Personal.Endpoint>(ctx =>
        {
            ctx.AddTestServices(s =>
            {
                s.AddSingleton(this.FakeUserRepository);
                s.AddSingleton(this.FakeMemberRepository);
            });
        });
    }


    [Fact]
    public async Task Get_Request_Throws_ValidationsFailure_DueToNullUser()
    {
        // Arrange
        var ep = this.SetupEndpoint();

        A.CallTo(() => this.FakeUserRepository.Retrieve(this.Request.UserId)).Returns(null);

        // Act
        async Task Act() => await ep.HandleAsync(this.Request, default);


        // Assert
        var exception = await Assert.ThrowsAnyAsync<ValidationFailureException>(Act);
        Assert.NotNull(exception);
        Assert.Equal("ThrowError() called! - User does not exist", exception.Message);
    }


    [Fact]
    public async Task Get_Request_Throws_ValidationsFailure_DueToNullMember()
    {
        // Arrange
        var ep = this.SetupEndpoint();
        var user = new User { Email = "test@test.de", PasswordHash = "notsafe987", MemberId = null };


        A.CallTo(() => this.FakeUserRepository.Retrieve(this.Request.UserId)).Returns(user);

        // Act
        async Task Act() => await ep.HandleAsync(this.Request, default);

        // Assert
        var exception = await Assert.ThrowsAnyAsync<ValidationFailureException>(Act);
        Assert.NotNull(exception);
        Assert.Equal("ThrowError() called! - Member does not exist", exception.Message);
    }


    [Fact]
    public async Task Get_Request_Throws_ValidationsFailure_DueToNonExistingMember()
    {
        // Arrange
        var ep = this.SetupEndpoint();
        var user = new User
        {
            Email = "test@test.de",
            PasswordHash = "notsafe987",
            MemberId = new Guid("E610101A-ED58-4A2F-8DEB-9FEAAEDD6B7C")
        };


        A.CallTo(() => this.FakeUserRepository.Retrieve(this.Request.UserId)).Returns(user);
        A.CallTo(() => this.FakeMemberRepository.Retrieve((Guid)user.MemberId)).Returns(null);

        // Act
        async Task Act() => await ep.HandleAsync(this.Request, default);

        // Assert
        var exception = await Assert.ThrowsAnyAsync<ValidationFailureException>(Act);
        Assert.NotNull(exception);
        Assert.Equal("ThrowError() called! - Member does not exist", exception.Message);
    }
}
