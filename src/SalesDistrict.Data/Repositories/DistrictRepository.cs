using Dapper;
using Microsoft.Data.SqlClient;
using SalesDistrict.Application;
using SalesDistrict.Application.Abstractions;
using SalesDistrict.Domain;

namespace SalesDistrict.Data.Repositories;

public sealed class DistrictRepository : IDistrictRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    private const int ForeignKeyViolation = 547;

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

    public async Task<bool> UpdateDistrictSalespersonsAsync(int districtId, int primarySalespersonId, IReadOnlyList<int> secondarySalespersonIds, CancellationToken cancellationToken)
    {
        const string deleteSql = """
            DELETE FROM dbo.DistrictSecondarySalesperson
            WHERE DistrictId = @DistrictId;
            """;

        const string updatePrimarySql = """
            UPDATE dbo.District
            SET PrimarySalespersonId = @PrimarySalespersonId
            WHERE DistrictId = @DistrictId;
            """;

        const string insertSql = """
            INSERT INTO dbo.DistrictSecondarySalesperson (DistrictId, SalespersonId, PrimarySalespersonId)
            VALUES (@DistrictId, @SalespersonId, @PrimarySalespersonId);
            """;

        await using var connection = _connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        // Delete existing secondary salespersons
        var deleteCommand = new CommandDefinition(deleteSql, new { DistrictId = districtId }, transaction: transaction, cancellationToken: cancellationToken);
        await connection.ExecuteAsync(deleteCommand);

        // Update primary salesperson
        var updatePrimaryCommand = new CommandDefinition(updatePrimarySql, new { DistrictId = districtId, PrimarySalespersonId = primarySalespersonId }, transaction: transaction, cancellationToken: cancellationToken);
        int rows;

        try
        {
            rows = await connection.ExecuteAsync(updatePrimaryCommand);
        }
        catch (SqlException ex) when (ex.Number == ForeignKeyViolation)
        {
            throw new SalespersonsUpdateException($"Primary salesperson {primarySalespersonId} does not exist.", ex);
        }

        if (rows == 0)
        {
            return false;
        }

        // Insert new secondary salespersons
        foreach (var salespersonId in secondarySalespersonIds)
        {
            var insertCommand = new CommandDefinition(insertSql, new { DistrictId = districtId, SalespersonId = salespersonId, PrimarySalespersonId = primarySalespersonId }, transaction: transaction, cancellationToken: cancellationToken);
            try
            {
                await connection.ExecuteAsync(insertCommand);
            }
            catch (SqlException ex) when (ex.Number == ForeignKeyViolation)
            {
                throw new SalespersonsUpdateException($"Secondary salesperson {salespersonId} does not exist.", ex);
            }
        }

        await transaction.CommitAsync(cancellationToken);

        return true;
    }
}