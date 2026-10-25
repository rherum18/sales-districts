using Dapper;
using Microsoft.Data.SqlClient;
using SalesDistrict.Domain;

namespace SalesDistrict.Application.Services;

public class DistrictService
{
    private readonly string _connectionString;

    public DistrictService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<District>> GetDistrictsAsync()
    {
        const string sql = "SELECT DistrictId, Name, PrimarySalespersonId FROM dbo.District";

        using var connection = new SqlConnection(_connectionString);

        var districts = await connection.QueryAsync<District>(sql);

        return districts.AsList();
    }
}