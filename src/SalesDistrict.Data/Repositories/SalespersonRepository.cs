using Dapper;
using SalesDistrict.Application.Abstractions;
using SalesDistrict.Domain;

namespace SalesDistrict.Data.Repositories;

public sealed class SalespersonRepository : ISalespersonRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public SalespersonRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<Salesperson>> GetAllAsync(CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT SalespersonId, Name
            FROM dbo.Salesperson
            ORDER BY Name, SalespersonId
            """;

        await using var connection = _connectionFactory.Create();
        var command = new CommandDefinition(sql, cancellationToken: cancellationToken);
        var salespersons = await connection.QueryAsync<Salesperson>(command);
        return salespersons.AsList();
    }
}