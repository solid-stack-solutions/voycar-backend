namespace Voycar.Api.Web.Tests.Integration.Setup;

using Registration = Features.Members.Post.Registration;
using Verify = Features.Members.Get.Verify;
using Login = Features.Members.Post.Login;

public static class ClientFactory
{
    public static async Task<HttpClient> CreateMemberClient(AppFixture<Program> app)
    {
        var member = app.CreateClient();

        const string testMail = "integration@test.de";
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
}
