namespace Voycar.Api.Web.Tests.Integration.Setup;

using System.Data;
using Context;
using Entities;
using Microsoft.EntityFrameworkCore;
using Registration = Features.Users.Endpoints.Post.Register;
using Login = Features.Users.Endpoints.Post.Login;


public static class ClientFactory
{
    /// <summary>
    /// Creates a new HTTP client which is logged in as a member. The member will be saved in the database
    /// and verified. The HTTP client will be authorized with the member role.
    /// </summary>
    /// <param name="app">The <c>AppFixture</c> to create a new HTTP client with</param>
    /// <param name="context">The context to save the member in the database</param>
    /// <param name="email">The Email to create and login the member</param>
    /// <param name="password">The password to create and login the member</param>
    /// <returns>The created and logged in HTTP member client</returns>
    /// <exception cref="HttpRequestException">If the endpoint for creating a member fails</exception>
    public static async Task<HttpClient> CreateMemberClient(AppFixture<Program> app, VoycarDbContext context,
        string email, string password)
    {
        const string planName = "basic";

        var member = app.CreateClient();
        var planId = (await context.Plans.FirstOrDefaultAsync(
            plan => plan.Name == planName))?.Id;
        if (planId is null)
        {
            throw new RowNotInTableException($"plan \"{planName}\" is not in db");
        }

        // Register new member, calling endpoint since manually registering member would be too complicated
        var (registerHttpResponse, registerResponseBody) =
            await member.POSTAsync<Registration.Endpoint, Registration.Request, Registration.Response>(
                new Registration.Request
                {
                    Email = email.ToLowerInvariant(),
                    Password = password,
                    FirstName = "...",
                    LastName = "...",
                    Street = "...",
                    HouseNumber = "...",
                    PostalCode = "...",
                    City = "...",
                    Country = "...",
                    BirthDate = new DateOnly(2000,
                        1,
                        1),
                    BirthPlace = "...",
                    PhoneNumber = "...",
                    PlanId = (Guid)planId
                }
            );
        if (registerHttpResponse.StatusCode != HttpStatusCode.OK)
        {
            throw new HttpRequestException("unable to create new member from endpoint");
        }

        await VerifyUserInDb(context, email.ToLowerInvariant());
        await LogInHttpClient(member, email.ToLowerInvariant(), password);

        return member;
    }


    /// <summary>
    /// Creates a new HTTP client which is logged in as an employee. The employee will be saved in the database
    /// and verified. The HTTP client will be authorized with the employee role.
    /// </summary>
    /// <param name="app">The <c>AppFixture</c> to create a new HTTP client with</param>
    /// <param name="context">The context to save the employee in the database</param>
    /// <param name="email">The Email to create and login the employee</param>
    /// <param name="password">The password to create and login the employee</param>
    /// <returns>The created and logged in http employee client</returns>
    /// <exception cref="RowNotInTableException">If the employee role is not in the database</exception>
    public static async Task<HttpClient> CreateEmployeeClient(AppFixture<Program> app, VoycarDbContext context,
        string email, string password)
    {
        const string EmployeeRoleName = "employee";

        // Get employee role ID
        var roleId = (await context.Roles.FirstOrDefaultAsync(
            role => role.Name == EmployeeRoleName))?.Id;
        if (roleId is null)
        {
            throw new RowNotInTableException($"role \"{EmployeeRoleName}\" is not in db");
        }

        return await CreateUserWithRoleClient(app, context, (Guid)roleId, email.ToLowerInvariant(), password);
    }


    /// <summary>
    /// Creates a new http client which is logged in as an admin. The admin will be saved in the database and verified.
    /// The http client will be authorized with the admin role.
    /// </summary>
    /// <param name="app">The AppFixture to create a new http client with</param>
    /// <param name="context">The context to save the admin in the database</param>
    /// <param name="email">The Email to create and login the admin</param>
    /// <param name="password">The Password to create and login the admin</param>
    /// <returns>The created and logged in http admin client</returns>
    /// <exception cref="RowNotInTableException">If the admin role is not in the database</exception>
    public static async Task<HttpClient> CreateAdminClient(AppFixture<Program> app, VoycarDbContext context,
        string email, string password)
    {
        const string AdminRoleName = "admin";

        // Get admin role ID
        var roleId = (await context.Roles.FirstOrDefaultAsync(
            role => role.Name == AdminRoleName))?.Id;
        if (roleId is null)
        {
            throw new RowNotInTableException($"role \"{AdminRoleName}\" is not in db");
        }

        return await CreateUserWithRoleClient(app, context, (Guid)roleId, email.ToLowerInvariant(), password);
    }


    /// <summary>
    /// Creates a new HTTP client which is logged in as a newly created user. The associated user will be saved
    /// in the databse and verified. The user is assigned the given role ID and the client will be authorized with the
    /// according role.
    /// </summary>
    /// <param name="app">The <c>AppFixture</c> to create a new HTTP client with</param>
    /// <param name="context">The context to save the user in the database</param>
    /// <param name="roleId">The database ID of the role to assign to the user</param>
    /// <param name="email">The email for the user</param>
    /// <param name="password">The password for the user</param>
    /// <returns>The created and logged in HTTP client</returns>
    /// <exception cref="DbUpdateException">If the user or attributes of the user can't be saved in the
    /// database</exception>
    private static async Task<HttpClient> CreateUserWithRoleClient(AppFixture<Program> app, VoycarDbContext context,
        Guid roleId, string email, string password)
    {
        // Create user entity
        var user = new User()
        {
            Email = email, PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(password), RoleId = roleId
        };
        context.Users.Add(user);
        var changedAmount = await context.SaveChangesAsync();
        if (changedAmount < 1)
        {
            throw new DbUpdateException("unable to create user in db");
        }

        var userClient = app.CreateClient();
        await VerifyUserInDb(context, email.ToLowerInvariant());
        await LogInHttpClient(userClient, email.ToLowerInvariant(), password);

        return userClient;
    }


    private static async Task VerifyUserInDb(VoycarDbContext context, string email)
    {
        // ToDo register endpoint should return ID, which can then be used to find user
        var userEntity = await context.Users.FirstOrDefaultAsync(user => user.Email == email);
        if (userEntity is null)
        {
            throw new RowNotInTableException("user is not in db, unable to verify user");
        }

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

        if (response.StatusCode != HttpStatusCode.OK)
        {
            throw new HttpRequestException("unable to login user with login endpoint");
        }
    }
}
