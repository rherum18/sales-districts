using Dapper;
using Microsoft.Data.SqlClient;
using SalesDistrict.Domain;

namespace SalesDistrict.Application.Services;

public class SalespersonService
{
    private readonly string _connectionString;

    public SalespersonService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IReadOnlyList<Salesperson>> GetSalespersonsAsync()
    {
        const string sql = "SELECT SalespersonId, Name, PrimarySalespersonId FROM dbo.Salesperson";

        using var connection = new SqlConnection(_connectionString);

        var salespersons = await connection.QueryAsync<Salesperson>(sql);

        return salespersons.AsList();
    }
}