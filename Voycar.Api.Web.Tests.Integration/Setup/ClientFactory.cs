namespace Voycar.Api.Web.Tests.Integration.Setup;

using Common;
using Context;
using Entities;
using Microsoft.EntityFrameworkCore;
using Registration = Features.Members.Post.Registration;
using Verify = Features.Members.Get.Verify;
using Login = Features.Members.Post.Login;

public static class ClientFactory
{

    public static async Task<HttpClient> CreateMemberClient(AppFixture<Program> app)
    {
        var member = app.CreateClient();

        const string testMail = "member.integration@test.de";
        const string password = "integration";

        var (regHttpRsp, regBodyRes) =
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
        var verHttpRsp = await member.GETAsync<Verify.Endpoint, Verify.Request>(new Verify.Request
        {
            VerificationToken = regBodyRes.VerificationToken
        });
        await member.POSTAsync<Login.Endpoint, Login.Request>(new Login.Request
        {
            Email = testMail, Password = password
        });

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
        Guid roleId, string email,
        string password)
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

        // Login user client
        var userClient = app.CreateClient();
        var response = await userClient.POSTAsync<Login.Endpoint, Login.Request>(new Login.Request()
        {
            Email = email, Password = password
        });
        // Assert login response
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        return userClient;
    }
}
