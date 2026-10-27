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

    public async Task<IReadOnlyList<District>> GetAllAsync(CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT DistrictId, Name, PrimarySalespersonId
            FROM dbo.District
            ORDER BY Name
            """;

        using var connection = _connectionFactory.Create();
        var command = new CommandDefinition(sql, cancellationToken: cancellationToken);
        var districts = await connection.QueryAsync<District>(command);
        return districts.AsList();
    }
}