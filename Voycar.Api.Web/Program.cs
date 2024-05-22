try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, configuration) =>
    {
        configuration.ReadFrom.Configuration(context.Configuration);
    });

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

    if (app.Environment.IsDevelopment())
    {
        app.UseSwaggerGen();
    }

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
