using Dapper;
using SalesDistrict.Application.Abstractions;
using SalesDistrict.Domain;

namespace SalesDistrict.Data.Repositories;

public sealed class DistrictRepository : IDistrictRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public DistrictRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    private sealed record DistrictRow(int DistrictId, string Name, int PrimarySalespersonId, string PrimarySalespersonName);

    public async Task<IReadOnlyList<District>> GetAllAsync(CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT DistrictId, Name, PrimarySalespersonId
            FROM dbo.District
            ORDER BY Name
            """;

        await using var connection = _connectionFactory.Create();
        var command = new CommandDefinition(sql, cancellationToken: cancellationToken);
        var districts = await connection.QueryAsync<District>(command);
        return districts.AsList();
    }

    public async Task<DistrictDetails?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT d.DistrictId, d.Name, p.SalespersonId AS PrimarySalespersonId, p.Name AS PrimarySalespersonName
            FROM dbo.District d
            JOIN dbo.Salesperson p ON p.SalespersonId = d.PrimarySalespersonId
            WHERE d.DistrictId = @DistrictId;

            SELECT s.SalespersonId, s.Name
            FROM dbo.DistrictSecondarySalesperson dss
            JOIN dbo.Salesperson s ON s.SalespersonId = dss.SalespersonId
            WHERE dss.DistrictId = @DistrictId
            ORDER BY s.Name, s.SalespersonId;

            SELECT StoreId, Name, DistrictId
            FROM dbo.Store
            WHERE DistrictId = @DistrictId
            ORDER BY Name;
            """;
        await using var connection = _connectionFactory.Create();
        var command = new CommandDefinition(sql, new { DistrictId = id }, cancellationToken: cancellationToken);

        await using var grid = await connection.QueryMultipleAsync(command);

        var row = await grid.ReadSingleOrDefaultAsync<DistrictRow>();
        var secondaries = (await grid.ReadAsync<Salesperson>()).AsList();
        var stores = (await grid.ReadAsync<Store>()).AsList();

        if (row is null)
            return null;

        return new DistrictDetails
        {
            DistrictId = row.DistrictId,
            Name = row.Name,
            PrimarySalesperson = new Salesperson { SalespersonId = row.PrimarySalespersonId, Name = row.PrimarySalespersonName },
            SecondarySalespersons = secondaries,
            Stores = stores,
        };
    }
}