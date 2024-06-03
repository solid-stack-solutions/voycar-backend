namespace Voycar.Api.Web.Tests;

using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Context;

public class App : AppFixture<Program>
{
    private const string ConnString = "User ID=admin;Password=admin;Server=localhost;Port=5432;Database=VoycarDb;";
    private VoycarDbContext _context;

    protected override Task SetupAsync()
    {
        // place one-time setup code here
        this._context = this.CreateContext();

        this._context.Database.EnsureDeleted();
        this._context.Database.EnsureCreated();

        return Task.CompletedTask;
    }

    protected override void ConfigureApp(IWebHostBuilder builder)
    {
        // do host builder config here
    }

    protected override void ConfigureServices(IServiceCollection service)
    {
        // do test service registration here
        service.RemoveAll(typeof(DbContextOptions<VoycarDbContext>));
        service.AddNpgsql<VoycarDbContext>(ConnString);
    }

    protected override Task TearDownAsync()
    {
        // do cleanups here
        return Task.CompletedTask;
    }

    public VoycarDbContext CreateContext()
    {
        return new VoycarDbContext(new DbContextOptionsBuilder<VoycarDbContext>().UseNpgsql(ConnString).Options);
    }
}
