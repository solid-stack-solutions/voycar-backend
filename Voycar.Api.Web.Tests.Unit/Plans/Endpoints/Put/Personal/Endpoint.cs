namespace Voycar.Api.Web.Tests.Unit.Plans.Endpoints.Put.Personal;

using Entities;
using FakeItEasy;
using Features.Members.Repository;
using Features.Plans.Endpoints.Put.Personal;
using Features.Plans.Repository;
using Features.Users.Repository;
using Microsoft.Extensions.DependencyInjection;


public class Endpoint : TestBase<App>
{
    private readonly IUsers FakeUserRepository = A.Fake<IUsers>();
    private readonly IMembers FakeMemberRepository = A.Fake<IMembers>();
    private readonly IPlans FakePlanRepository = A.Fake<IPlans>();

    private readonly Request Request = new() { PlanId = new Guid("35CA320B-59A9-4A5C-8E74-7CEC34059077") };

    private Features.Plans.Endpoints.Put.Personal.Endpoint SetupEndpoint()
    {
        return Factory.Create<Features.Plans.Endpoints.Put.Personal.Endpoint>(ctx =>
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
        var user = new User
        {
            Email = "test@test.de",
            PasswordHash = "notsafe987",
            MemberId = null
        };


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
            MemberId = new Guid("1A83EDDB-58A2-4B63-AA9F-25346546090D")
        };


        A.CallTo(() => this.FakeUserRepository.Retrieve(this.Request.UserId)).Returns(user);
        A.CallTo(() => this.FakeMemberRepository.Retrieve(user.MemberId.Value)).Returns(new Member());
        A.CallTo(() => this.FakePlanRepository.Retrieve(this.Request.PlanId)).Returns(null);

        // Act
        async Task Act() => await ep.HandleAsync(this.Request, default);

        // Assert
        var exception = await Assert.ThrowsAnyAsync<ValidationFailureException>(Act);
        Assert.NotNull(exception);
        Assert.Equal("ThrowError() called! - Plan does not exist", exception.Message);
    }

    [Fact]
    public async Task Put_Request_Throws_ValidationsFailure_DueToNullSamePlanIds()
    {
        // Arrange
        var ep = this.SetupEndpoint();
        var user = new User
        {
            Email = "test@test.de",
            PasswordHash = "notsafe987",
            MemberId = new Guid("1A83EDDB-58A2-4B63-AA9F-25346546090D")
        };

        A.CallTo(() => this.FakeUserRepository.Retrieve(this.Request.UserId)).Returns(user);
        A.CallTo(() => this.FakeMemberRepository.Retrieve(user.MemberId.Value)).Returns(new Member{PlanId = this.Request.PlanId});
        A.CallTo(() => this.FakePlanRepository.Retrieve(this.Request.PlanId)).Returns(new Plan{ Id = this.Request.PlanId});

        // Act
        async Task Act() => await ep.HandleAsync(this.Request, default);

        // Assert
        var exception = await Assert.ThrowsAnyAsync<ValidationFailureException>(Act);
        Assert.NotNull(exception);
        Assert.Equal("ThrowError() called! - Cannot update the plan with the same ID", exception.Message);
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

        var member = new Member
        {
            Id = new Guid("E610101A-ED58-4A2F-8DEB-9FEAAEDD6B7C"),
        };


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
