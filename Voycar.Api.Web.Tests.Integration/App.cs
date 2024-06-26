﻿namespace Voycar.Api.Web.Tests.Integration;

using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Context;
using Setup;
using Testcontainers.PostgreSql;


/*
Usually, the SUT (API and database) will only be created once for all test classes and cached afterward.
 Disabling the WAF cache will create a new instance for every test class, resulting in worse test performance.
 This behavior is intended so that test classes can perform operations on the database without impacting
 any other tests that assert the content of the database.
 See: https://fast-endpoints.com/docs/integration-unit-testing#app-fixture
 See: https://gist.github.com/dj-nitehawk/04a78cea10f2239eb81c958c52ec84e0
*/
[DisableWafCache]
public class App : AppFixture<Program>
{
    /* Change this constant to true, to test against production database
    WARNING: This will DELETE your existing data */
    private const bool TestAgainstProductionDb = false;
    private const string TestMemberMail = "member.integration@test.de";
    private const string TestEmployeeMail = "employee.integration@test.de";
    private const string TestAdminMail = "admin.integration@test.de";
    private const string Password = "integration";
    private PostgreSqlContainer Container { get; set; }
    private string ConnectionString { get; set; }
    public VoycarDbContext Context { get; private set; }
    public HttpClient Member { get; private set; }
    public HttpClient Employee { get; private set; }
    public HttpClient Admin { get; private set; }


    protected override async Task SetupAsync()
    {
        // == Place one-time setup code here ==

        if (TestAgainstProductionDb)
        {
            // WARNING: This will test against production database and DELETE existing data
            this.ConnectionString =
                "User ID=admin;Password=admin;Server=localhost;Port=5432;Database=VoycarDb;";
            return;
        }

        // Create a Db test container to run tests against
        this.Container = new PostgreSqlBuilder()
            .WithImage("postgres:16.3")
            .WithDatabase("pgsql-testcontainer")
            .WithUsername("admin")
            .WithPassword("admin")
            .WithExposedPort(5241)
            .Build();
        await this.Container.StartAsync();
        this.ConnectionString = this.Container.GetConnectionString();

        // Set up Db for tests
        this.Context = this.Services.GetRequiredService<VoycarDbContext>();
        await this.Context.Database.EnsureDeletedAsync(); // Delete data on Db
        await this.Context.Database.EnsureCreatedAsync(); // Populate with Db schema

        // Create custom HttpClients to use in tests
        this.Member = await ClientFactory.CreateMemberClient(this, this.Context, TestMemberMail, Password);
        this.Employee = await ClientFactory.CreateEmployeeClient(this, this.Context, TestEmployeeMail, Password);
        this.Admin = await ClientFactory.CreateAdminClient(this, this.Context, TestAdminMail, Password);
    }


    protected override void ConfigureApp(IWebHostBuilder builder)
    {
        // == Do host builder config here ==
    }


    protected override void ConfigureServices(IServiceCollection services)
    {
        // == Do test service registration here ==
        var descriptor = services.SingleOrDefault(service =>
            service.ServiceType == typeof(DbContextOptions<VoycarDbContext>)
        );
        if (descriptor is not null)
        {
            services.Remove(descriptor);
        }

        services.AddDbContext<VoycarDbContext>(options =>
        {
            options.UseNpgsql(this.ConnectionString);
        });
    }


    protected override Task TearDownAsync()
    {
        // == Do cleanups here ==

        this.Member.Dispose();
        this.Employee.Dispose();
        this.Admin.Dispose();
        return this.Container.StopAsync();
    }
}
