namespace Voycar.Api.Web.Tests.Integration.Setup;

using Common;
using Context;
using Entities;
using Microsoft.EntityFrameworkCore;
using Registration = Features.Members.Post.Registration;
using Login = Features.Members.Post.Login;

public static class ClientFactory
{

    public static async Task<HttpClient> CreateMemberClient(AppFixture<Program> app, VoycarDbContext context)
    {
        const string testMail = "member.integration@test.de";
        const string password = "integration";

        var member = app.CreateClient();

        // Register new member, calling endpoint since manually registering member would be too complicated
        var (registerHttpResponse, registerResponseBody) =
            await member.POSTAsync<Registration.Endpoint, Registration.Request, Registration.Response>(
                new Registration.Request
                {
                    Email = testMail,
                    Password = password,
                    FirstName = "...",
                    LastName = "...",
                    Street = "...",
                    HouseNumber = "...",
                    PostalCode = "...",
                    City = "...",
                    Country = "...",
                    BirthDate = new DateOnly(2000, 1, 1),
                    BirthPlace = "...",
                    PhoneNumber = "...",
                }
            );
        registerHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        VerifyUserInDb(context, testMail);
        LogInHttpClient(member, testMail, password);

        return member;
    }

    public static async Task<HttpClient> CreateEmployeeClient(AppFixture<Program> app, VoycarDbContext context)
    {
        const string EmployeeRoleName = "employee";
        const string testMail = $"{EmployeeRoleName}.integration@test.de";
        const string password = "integration";

        // Get employee role ID
        var roleId = (await context.Roles.FirstOrDefaultAsync(
            role => role.Name == EmployeeRoleName))?.Id;
        if (roleId is null)
        {
            throw new RoleNotInDbException($"role \"{EmployeeRoleName}\" is not in db");
        }

        return await CreateUserWithRoleClient(app, context, (Guid)roleId, testMail, password);
    }

    public static async Task<HttpClient> CreateAdminClient(AppFixture<Program> app, VoycarDbContext context)
    {
        const string AdminRoleName = "admin";
        const string testMail = $"{AdminRoleName}.integration@test.de";
        const string password = "integration";

        // Get admin role ID
        var roleId = (await context.Roles.FirstOrDefaultAsync(
            role => role.Name == AdminRoleName))?.Id;
        if (roleId is null)
        {
            throw new RoleNotInDbException($"role \"{AdminRoleName}\" is not in db");
        }

        return await CreateUserWithRoleClient(app, context, (Guid)roleId, testMail, password);
    }


    private static async Task<HttpClient> CreateUserWithRoleClient(AppFixture<Program> app, VoycarDbContext context,
        Guid roleId, string email, string password)
    {
        // Create user entity
        var user = new User()
        {
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(password),
            RoleId = roleId
        };
        context.Users.Add(user);
        var changedAmount = await context.SaveChangesAsync();
        if (changedAmount < 1)
        {
            throw new DbUpdateException("unable to create user in db");
        }

        var userClient = app.CreateClient();
        LogInHttpClient(userClient, email, password);

        return userClient;
    }

    private static async void VerifyUserInDb(VoycarDbContext context, string email)
    {
        // ToDo register endpoint should return ID, which can then be used to find user
        var userEntity = await context.Users.FirstOrDefaultAsync(user => user.Email == email);
        // Assert that user entity exists
        userEntity.Should().NotBeNull();

        userEntity!.VerifiedAt = DateTime.UtcNow;
        context.Users.Update(userEntity);
        var changedAmount = await context.SaveChangesAsync();
        if (changedAmount < 1)
        {
            throw new DbUpdateException("unable to set verification status of user in db");
        }
    }

    private static async void LogInHttpClient(HttpClient client, string email, string password)
    {
        // Login user client, calling login endpoint since manually handling auth cookies would be too complicated
        var response = await client.POSTAsync<Login.Endpoint, Login.Request>(new Login.Request()
        {
            Email = email, Password = password
        });
        // Assert login response
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
