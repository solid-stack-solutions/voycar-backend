namespace Voycar.Api.Web.Tests.Unit.Members.Endpoints.Put.Personal;

using Entities;
using FakeItEasy;
using Features.Members.Endpoints.Put.Personal;
using Features.Members.Repository;
using Features.Plans.Repository;
using Features.Users.Repository;
using Microsoft.Extensions.DependencyInjection;


public class Endpoint : TestBase<App>
{
    private readonly IUsers FakeUserRepository = A.Fake<IUsers>();
    private readonly IMembers FakeMemberRepository = A.Fake<IMembers>();
    private readonly IPlans FakePlanRepository = A.Fake<IPlans>();


    private readonly Request Request = new()
    {
        FirstName = "null",
        LastName = "null",
        Street = "null",
        HouseNumber = "null",
        PostalCode = "null",
        City = "null",
        Country = "null",
        BirthDate = new DateOnly(2002, 06, 25),
        BirthPlace = "null",
        PhoneNumber = "null",
        PlanId = new Guid("480BF07F-D770-4001-AF0F-4ABA2464D60A")
    };


    private Features.Members.Endpoints.Put.Personal.Endpoint SetupEndpoint()
    {
        return Factory.Create<Features.Members.Endpoints.Put.Personal.Endpoint>(ctx =>
        {
            ctx.AddTestServices(s =>
            {
                s.AddSingleton(this.FakeUserRepository);
                s.AddSingleton(this.FakeMemberRepository);
                s.AddSingleton(this.FakePlanRepository);
            });
        });
    }


    [Fact]
    public async Task Put_Request_Throws_ValidationsFailure_DueToNullUser()
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
    public async Task Put_Request_Throws_ValidationsFailure_DueToNullMember()
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
        Assert.Equal("ThrowError() called! - User is not a member", exception.Message);
    }


    [Fact]
    public async Task Put_Request_Throws_ValidationsFailure_DueToNullPlan()
    {
        // Arrange
        var ep = this.SetupEndpoint();
        var user = new User
        {
            Email = "test@test.de",
            PasswordHash = "notsafe987",
            MemberId = new Guid("E610101A-ED58-4A2F-8DEB-9FEAAEDD6B7C")
        };
        var member = new Member { Id = (Guid)user.MemberId };

        A.CallTo(() => this.FakeUserRepository.Retrieve(this.Request.UserId)).Returns(user);
        A.CallTo(() => this.FakeMemberRepository.Retrieve(member.Id)).Returns(member);
        A.CallTo(() => this.FakePlanRepository.Retrieve(this.Request.PlanId)).Returns(null);

        // Act
        async Task Act() => await ep.HandleAsync(this.Request, default);

        // Assert
        var exception = await Assert.ThrowsAnyAsync<ValidationFailureException>(Act);
        Assert.NotNull(exception);
        Assert.Equal("ThrowError() called! - Plan does not exist", exception.Message);
    }


    [Fact]
    public async Task Put_Request_Throws_ValidationsFailure_DueToNonExistingMember()
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
        A.CallTo(() => this.FakePlanRepository.Retrieve(this.Request.PlanId)).Returns(new Plan());
        A.CallTo(() => this.FakeMemberRepository.Retrieve((Guid)user.MemberId)).Returns(null);


        // Act
        async Task Act() => await ep.HandleAsync(this.Request, default);

        // Assert
        var exception = await Assert.ThrowsAnyAsync<ValidationFailureException>(Act);
        Assert.NotNull(exception);
        Assert.Equal("ThrowError() called! - Member does not exist", exception.Message);
    }


    [Fact]
    public async Task Put_Request_Throws_ValidationsFailure_DueToFailedUpdate()
    {
        // Arrange
        var ep = this.SetupEndpoint();
        var user = new User
        {
            Email = "test@test.de",
            PasswordHash = "notsafe987",
            MemberId = new Guid("E610101A-ED58-4A2F-8DEB-9FEAAEDD6B7C")
        };

        var member = new Member { Id = new Guid("E610101A-ED58-4A2F-8DEB-9FEAAEDD6B7C"), };

        A.CallTo(() => this.FakeUserRepository.Retrieve(this.Request.UserId)).Returns(user);
        A.CallTo(() => this.FakeMemberRepository.Retrieve((Guid)user.MemberId)).Returns(member);
        A.CallTo(() => this.FakePlanRepository.Retrieve(this.Request.PlanId)).Returns(new Plan());
        A.CallTo(() => this.FakeMemberRepository.Update(member)).Returns(false);

        // Act
        async Task Act() => await ep.HandleAsync(this.Request, default);

        // Assert
        var exception = await Assert.ThrowsAnyAsync<ValidationFailureException>(Act);
        Assert.NotNull(exception);
        Assert.Equal("ThrowError() called! - Failed to update member data", exception.Message);
    }
}
