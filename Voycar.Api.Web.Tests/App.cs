namespace Voycar.Api.Web.Tests;

using Microsoft.AspNetCore.Hosting;
using Moq;
using Web.Features.Roles.Repository;

public class App : AppFixture<Program>
{
    private Mock<IRoles> _rolesRepo;

    protected override Task SetupAsync()
    {
        // place one-time setup code here
        return Task.CompletedTask;
    }

    protected override void ConfigureApp(IWebHostBuilder a)
    {
        // do host builder config here
    }

    protected override void ConfigureServices(IServiceCollection s)
    {
        // do test service registration here
        this._rolesRepo = new Mock<IRoles>();
        s.AddTransient<IRoles>(_ => this._rolesRepo.Object);
    }

    protected override Task TearDownAsync()
    {
        // do cleanups here
        return Task.CompletedTask;
    }
}
