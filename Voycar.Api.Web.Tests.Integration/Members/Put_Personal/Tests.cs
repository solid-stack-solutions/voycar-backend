namespace Voycar.Api.Web.Tests.Integration.Members.Put_Personal;

using Context;
using Microsoft.EntityFrameworkCore;
using Setup;
using R = Features.Members.Endpoints.Put.Personal;


public class Tests : TestBase<App>
{
    private readonly App _app;

    private readonly VoycarDbContext Context;


    // Setup request client
    public Tests(App app)
    {
        this._app = app;
        this.Context = this._app.Context;
    }


    public R.Request CreateValidRequest()
    {
        return new R.Request
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
        };
    }


    [Fact]
    public async Task Put_Request_ReturnsOk_And_UpdateUserInDb()
    {
        // Arrange
        var email = "NewMemberClient@test.de";
        var request = this.CreateValidRequest();
        var memberClient = await ClientFactory.CreateMemberClient(this._app, this.Context, email, "password");

        // Act
        var httpResponse = await memberClient.PUTAsync<R.Endpoint, R.Request>(request);


        // Arrange assertion
        var userInDb = await this.Context.Users.FirstOrDefaultAsync(user => user.Email == email.ToLowerInvariant());
        var memberInDb = await this.Context.Members.FirstOrDefaultAsync(member => member.Id == userInDb.MemberId);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        userInDb.Should().NotBeNull();
        memberInDb.Should().NotBeNull();


        memberInDb.FirstName.Should().Be(request.FirstName);
        memberInDb.LastName.Should().Be(request.LastName);
        memberInDb.Street.Should().Be(request.Street);
        memberInDb.HouseNumber.Should().Be(request.HouseNumber);
        memberInDb.PostalCode.Should().Be(request.PostalCode);
        memberInDb.City.Should().Be(request.City);
        memberInDb.Country.Should().Be(request.Country);
        memberInDb.BirthDate.Should().Be(request.BirthDate);
        memberInDb.BirthPlace.Should().Be(request.BirthPlace);
        memberInDb.PhoneNumber.Should().Be(request.PhoneNumber);
    }


    [Fact]
    public async Task Put_Request_JustTurned18_ReturnsOk_And_UpdateUserInDb()
    {
        // Arrange
        var email = "NewMember@test.de";
        var memberClient = await ClientFactory.CreateMemberClient(this._app, this.Context, email, "password");
        var request = this.CreateValidRequest();
        request.BirthDate = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-18);

        // Act
        var httpResponse = await memberClient.PUTAsync<R.Endpoint, R.Request>(request);

        // Arrange assertion
        var userInDb = await this.Context.Users.FirstOrDefaultAsync(user => user.Email == email.ToLowerInvariant());
        var memberInDb = await this.Context.Members.FirstOrDefaultAsync(member => member.Id == userInDb.MemberId);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        userInDb.Should().NotBeNull();
        memberInDb.Should().NotBeNull();
    }


    // Validator-Tests
    [Fact]
    public async Task Put_Request_ReturnsBadRequest_DueToInvalidFirstName()
    {
        // Arrange
        var request = this.CreateValidRequest();
        request.FirstName = "";

        // Act
        var httpResponse = await this._app.Member.PUTAsync<R.Endpoint, R.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Put_Request_ReturnsBadRequest_DueToInvalidLastName()
    {
        // Arrange
        var request = this.CreateValidRequest();
        request.LastName = "";

        // Act
        var httpResponse = await this._app.Member.PUTAsync<R.Endpoint, R.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Put_Request_ReturnsBadRequest_DueToInvalidStreet()
    {
        // Arrange
        var request = this.CreateValidRequest();
        request.Street = "";

        // Act
        var httpResponse = await this._app.Member.PUTAsync<R.Endpoint, R.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Put_Request_ReturnsBadRequest_DueToInvalidHouseNumber()
    {
        // Arrange
        var request = this.CreateValidRequest();
        request.HouseNumber = "";

        // Act
        var httpResponse = await this._app.Member.PUTAsync<R.Endpoint, R.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Put_Request_ReturnsBadRequest_DueToInvalidPostalCode()
    {
        // Arrange
        var request = this.CreateValidRequest();
        request.PostalCode = "";

        // Act
        var httpResponse = await this._app.Member.PUTAsync<R.Endpoint, R.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Put_Request_ReturnsBadRequest_DueToInvalidCity()
    {
        // Arrange
        var request = this.CreateValidRequest();
        request.City = "";

        // Act
        var httpResponse = await this._app.Member.PUTAsync<R.Endpoint, R.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Put_Request_ReturnsBadRequest_DueToInvalidCountry()
    {
        // Arrange
        var request = this.CreateValidRequest();
        request.Country = "";

        // Act
        var httpResponse = await this._app.Member.PUTAsync<R.Endpoint, R.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Put_Request_ReturnsBadRequest_DueToInvalidBirthDate()
    {
        // Arrange
        var request = this.CreateValidRequest();

        request.BirthDate = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var httpResponse = await this._app.Member.PUTAsync<R.Endpoint, R.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Put_Request_ReturnsBadRequest_DueToInvalidBirthDate_OneDayTooYoung()
    {
        // Arrange
        var request = this.CreateValidRequest();

        request.BirthDate = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-18).AddDays(1);

        // Act
        var httpResponse = await this._app.Member.PUTAsync<R.Endpoint, R.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Put_Request_ReturnsBadRequest_DueToInvalidBirthPlace()
    {
        // Arrange
        var request = this.CreateValidRequest();
        request.BirthPlace = "";

        // Act
        var httpResponse = await this._app.Member.PUTAsync<R.Endpoint, R.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Put_Request_ReturnsBadRequest_DueToInvalidPhoneNumber()
    {
        // Arrange
        var request = this.CreateValidRequest();
        request.PhoneNumber = "";

        // Act
        var httpResponse = await this._app.Member.PUTAsync<R.Endpoint, R.Request>(request);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
