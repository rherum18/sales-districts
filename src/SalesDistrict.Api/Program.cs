using Autofac;
using Autofac.Extensions.DependencyInjection;
using SalesDistrict.Api.DependencyInjection;
using SalesDistrict.Api.ErrorHandling;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{

    var builder = WebApplication.CreateBuilder(args);
    var connectionString = builder.Configuration.GetConnectionString("SalesDistrict")
        ?? throw new InvalidOperationException("Missing connection string 'SalesDistrict'.");


    // Add services to the container.
    builder.Services.AddOpenApi();

    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
    builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
    {
        containerBuilder.RegisterModule(new DataModule(connectionString));
        containerBuilder.RegisterModule<ApplicationModule>();
    });

    builder.Services.AddProblemDetails();
    builder.Services.AddExceptionHandler<SalespersonsUpdateExceptionHandler>();

    builder.Services.AddSerilog((services, loggerConfiguration) => loggerConfiguration
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services));

    builder.Services.AddControllers().AddControllersAsServices();

    builder.Services.AddAuthorization();

    var app = builder.Build();

    app.UseSerilogRequestLogging();
    app.UseExceptionHandler();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}