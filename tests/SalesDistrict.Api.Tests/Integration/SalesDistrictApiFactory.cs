using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;

namespace SalesDistrict.Api.Tests.Integration;

// Starts the API against a dedicated test database, so integration tests never touch the real SalesDistrict database.
// The database is built from the same scripts as the real one (database/01 and 02), which keeps schema and seed in one place.
public sealed class SalesDistrictApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string TestDatabaseName = "SalesDistrict_IntegrationTests";

    private static readonly string SqlServer =
        Environment.GetEnvironmentVariable("SALESDISTRICT_TEST_SQLSERVER") ?? @"(localdb)\MSSQLLocalDB";

    public string ConnectionString { get; } = new SqlConnectionStringBuilder
    {
        DataSource = SqlServer,
        InitialCatalog = TestDatabaseName,
        IntegratedSecurity = true,
        TrustServerCertificate = true,
    }.ConnectionString;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:SalesDistrict", ConnectionString);
    }

    public async ValueTask InitializeAsync()
    {
        await RunScriptAsync("01_create_schema.sql");
        await ResetDataAsync();
    }

    // Restores the seed data, so every test starts from the same known state.
    public Task ResetDataAsync() => RunScriptAsync("02_seed_data.sql");

    private async Task RunScriptAsync(string fileName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "database", fileName);
        var script = await File.ReadAllTextAsync(path);

        // Point the scripts at the test database instead of the real one
        script = Regex.Replace(script, @"\bSalesDistrict\b", TestDatabaseName);

        var masterConnectionString = new SqlConnectionStringBuilder(ConnectionString) { InitialCatalog = "master" }.ConnectionString;
        await using var connection = new SqlConnection(masterConnectionString);
        await connection.OpenAsync();

        // GO is a client-side batch separator (SSMS/sqlcmd), not T-SQL, so each batch is sent on its own
        var batches = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        foreach (var batch in batches.Where(b => !string.IsNullOrWhiteSpace(b)))
        {
            await using var command = new SqlCommand(batch, connection);
            await command.ExecuteNonQueryAsync();
        }
    }
}

[CollectionDefinition(Name)]
public sealed class DatabaseCollection : ICollectionFixture<SalesDistrictApiFactory>
{
    // Tests in this collection share one test database and run one at a time
    public const string Name = "Database";
}
