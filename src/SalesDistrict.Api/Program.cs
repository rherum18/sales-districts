using SalesDistrict.Application.Abstractions;
using SalesDistrict.Application.Services;
using SalesDistrict.Data;
using SalesDistrict.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("SalesDistrict")
    ?? throw new InvalidOperationException("Missing connection string 'SalesDistrict'.");

// Add services to the container.
builder.Services.AddSingleton(new SqlConnectionFactory(connectionString));
builder.Services.AddScoped<IDistrictRepository, DistrictRepository>();
builder.Services.AddScoped<IDistrictService, DistrictService>();
builder.Services.AddScoped<ISalespersonRepository, SalespersonRepository>();
builder.Services.AddScoped<ISalespersonService, SalespersonService>();

builder.Services.AddControllers();

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
