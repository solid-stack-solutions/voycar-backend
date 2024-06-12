namespace Voycar.Api.Web.Tests.Integration.Setup;

using Common;
using Context;
using Entities;
using Microsoft.EntityFrameworkCore;
using Registration = Features.Users.Endpoints.Post.Register;
using Login = Features.Users.Endpoints.Post.Login;

public static class ClientFactory
{
    /// <summary>
    /// Creates a new http client which is logged in as a member. The member will be saved in the database
    /// and verified. The http client will be authorized with the member role.
    /// </summary>
    /// <param name="app">The AppFixture to create a new http client with</param>
    /// <param name="context">The context to save the member in the database</param>
    /// <returns>The created and logged in http member client</returns>
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

        await VerifyUserInDb(context, testMail);
        await LogInHttpClient(member, testMail, password);

        return member;
    }

    /// <summary>
    /// Creates a new http client which is logged in as an employee. The employee will be saved in the database
    /// and verified. The http client will be authorized with the employee role.
    /// </summary>
    /// <param name="app">The AppFixture to create a new http client with</param>
    /// <param name="context">The context to save the employee in the database</param>
    /// <returns>The created and logged in http employee client</returns>
    /// <exception cref="RoleNotInDbException">If the employee role is not in the database</exception>
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

    /// <summary>
    /// Creates a new http client which is logged in as an admin. The admin will be saved in the database and verified.
    /// The http client will be authorized with the admin role.
    /// </summary>
    /// <param name="app">The AppFixture to create a new http client with</param>
    /// <param name="context">The context to save the admin in the database</param>
    /// <returns>The created and logged in http admin client</returns>
    /// <exception cref="RoleNotInDbException">If the admin role is not in the database</exception>
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


    /// <summary>
    /// Creates a new http client which is logged in as a newly created user. The associated user will be saved
    /// in the databse and verified. The user is assigned the given role ID and the client will be authorized with the
    /// according role.
    /// </summary>
    /// <param name="app">The AppFixture to create a new http client with</param>
    /// <param name="context">The context to save the user in the database</param>
    /// <param name="roleId">The database ID of the role to assign to the user</param>
    /// <param name="email">The email for the user</param>
    /// <param name="password">The password for the user</param>
    /// <returns>The created and logged in http client</returns>
    /// <exception cref="DbUpdateException">If the user or attributes of the user can't be saved in the
    /// database</exception>
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
        await VerifyUserInDb(context, email);
        await LogInHttpClient(userClient, email, password);

        return userClient;
    }

    private static async Task VerifyUserInDb(VoycarDbContext context, string email)
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

    private static async Task LogInHttpClient(HttpClient client, string email, string password)
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
