namespace Voycar.Api.Web.Tests.Integration.Setup;

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
                    BirthDate = new DateOnly(2000, 1, 1),
                    BirthPlace = "...",
                    PhoneNumber = "...",
                    DriversLicenseNumber = "...",
                    IdCardNumber = "..."
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

    public static async Task<HttpClient> CreateEmployeeClient(AppFixture<Program> app)
    {
        const string testMail = "employee.integration@test.de";
        const string password = "integration";
        Guid roleId = new Guid(); // TODO find id for employee role in DB

        return await CreateUserWithRoleClient(app, roleId, testMail, password);
    }

    public static async Task<HttpClient> CreateAdminClient(AppFixture<Program> app)
    {
        const string testMail = "admin.integration@test.de";
        const string password = "integration";
        Guid roleId = new Guid(); // TODO find id for admin role in DB

        return await CreateUserWithRoleClient(app, roleId, testMail, password);
    }


    private static async Task<HttpClient> CreateUserWithRoleClient(AppFixture<Program> app, Guid roleId, string email,
        string password)
    {
        var user = app.CreateClient();
        // TODO crate user directly on DB, with according role
        // TODO login user via POST request
        return user;
    }
}
