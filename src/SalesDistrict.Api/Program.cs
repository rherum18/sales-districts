using SalesDistrict.Application.Abstractions;
using SalesDistrict.Application.Services;
using SalesDistrict.Data;
using SalesDistrict.Data.Repositories;
using SalesDistrict.Api.ErrorHandling;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("SalesDistrict")
    ?? throw new InvalidOperationException("Missing connection string 'SalesDistrict'.");


// Add services to the container.
builder.Services.AddOpenApi();

builder.Services.AddSingleton(new SqlConnectionFactory(connectionString));
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<SalespersonsUpdateExceptionHandler>();
builder.Services.AddScoped<IDistrictRepository, DistrictRepository>();
builder.Services.AddScoped<IDistrictsService, DistrictsService>();
builder.Services.AddScoped<ISalespersonRepository, SalespersonRepository>();
builder.Services.AddScoped<ISalespersonsService, SalespersonsService>();

builder.Services.AddControllers();

builder.Services.AddAuthorization();

var app = builder.Build();

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
