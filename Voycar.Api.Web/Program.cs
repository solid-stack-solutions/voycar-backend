using Microsoft.EntityFrameworkCore;
using Voycar.Api.Web.Context;
using Voycar.Api.Web.Features.Members.Repository;
using Voycar.Api.Web.Features.Members.Services.EmailService;

try
{
    var builder = WebApplication.CreateBuilder(args);


    builder.Services.AddDbContext<VoycarDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("VoycarDb")));

    builder.Host.UseSerilog((context, configuration) =>
    {
        configuration.ReadFrom.Configuration(context.Configuration);
    });


    builder.Services.AddTransient<IEmailService, EmailService>();
    builder.Services.AddTransient<IMemberRepository, MemberRepository>();



    builder.Services.AddFastEndpoints();
    builder.Services.SwaggerDocument(options =>
    {
        options.DocumentSettings = s =>
        {
            s.Title = "Voycar Web API Documentation";
            s.Version = "v1";
        };
    });

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    app.UseFastEndpoints();

    // Caution: Swagger available in production environment
    app.UseSwaggerGen();


    app.Run();
}
catch (Exception exception)
{
    Log.Fatal(exception, "Application terminated unexpectedly!");
}
finally
{
    Log.Information("Application terminated!");
    Log.CloseAndFlush();
}

public partial class Program {}
