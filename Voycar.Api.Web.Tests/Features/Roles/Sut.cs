namespace Voycar.Api.Web.Tests.Features.Roles;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Web.Context;
using Web.Features.Roles.Repository;

public class Sut : AppFixture<Program>
{
    //public readonly VoycarDbContext _context;
    //public readonly IRoles _roles;

    //public Sut(VoycarDbContext context, IRoles roles)
    //{
    //    this._context = context;
    //    this._roles = roles;
    //}


    // public Sut()
    //{
     //   var options = new DbContextOptionsBuilder<VoycarDbContext>();
      //  options.UseNpgsql("User ID=admin;Password=admin;Server=localhost;Port=5433;Database=VoycarDb;");

       // var op = options.Options;

       // this._context = new VoycarDbContext(op);
       // this._roles = new Roles(this._context);
    //}

    protected override void ConfigureApp(IWebHostBuilder a)
    {
        a.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<VoycarDbContext>));

            var connString = "User ID=admin;Password=admin;Server=localhost;Port=5433;Database=VoycarDb;";

            services.AddNpgsql<VoycarDbContext>(connString);

            var dbContext = CreateDbContext(services);
            //dbContext.Database.EnsureCreated();

        });
    }

    protected override void ConfigureServices(IServiceCollection s)
    {
         //this._roles = new Mock<IRoles>();
         //s.AddTransient<IRoles, Roles>();
    }

    private static VoycarDbContext CreateDbContext(IServiceCollection service)
    {
        var serviceProv = service.BuildServiceProvider();
        var scope = serviceProv.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<VoycarDbContext>();
        return dbContext;
    }
}
