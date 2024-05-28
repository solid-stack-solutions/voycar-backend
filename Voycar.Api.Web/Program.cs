using Microsoft.EntityFrameworkCore;
using Voycar.Api.Web.Context;
using Voycar.Api.Web.Features.Permissions.Repository;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, configuration) =>
    {
        configuration.ReadFrom.Configuration(context.Configuration);
    });

    builder.Services.AddDbContext<VoycarDbContext>((options) =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("VoycarDb"));
    });
    // repositories
    builder.Services.AddTransient<IPermissions, Permissions>();

    builder.Services.AddFastEndpoints();
    builder.Services.SwaggerDocument(options =>
    {
        options.DocumentSettings = settings =>
        {
            settings.Title = "Voycar Web API Documentation";
            settings.Version = "v1";
        };
    });

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    app.UseFastEndpoints();

    // Caution: Swagger available in production environment
    app.UseSwaggerGen();

    app.Run();
}
catch (Exception exception) when (exception is not HostAbortedException)
{
    Log.Fatal(exception, "Application terminated unexpectedly!");
}
finally
{
    Log.Information("Application terminated!");
    Log.CloseAndFlush();
}

public partial class Program {}
