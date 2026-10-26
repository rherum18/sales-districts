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

    public async Task<IReadOnlyList<District>> GetDistrictsAsync()
    {
        const string sql = "SELECT DistrictId, Name, PrimarySalespersonId FROM dbo.District";

        using var connection = new SqlConnection(_connectionString);

        var districts = await connection.QueryAsync<District>(sql);

        return districts.AsList();
    }

    public async Task CreateDistrictAsync(District district)
    {
        const string sql = "INSERT INTO dbo.District (Name, PrimarySalespersonId) VALUES (@Name, @PrimarySalespersonId)";
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(sql, district);
    }

    public async Task UpdateDistrictAsync(District district)
    {
        const string sql = @"
            UPDATE dbo.District
            SET PrimarySalespersonId = @PrimarySalespersonId
            WHERE DistrictId = @DistrictId";
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(sql, district);
    }

    public async Task AssignPrimarySalespersonToDistrictAsync(int districtId, int salespersonId)
    {
        const string sql = @"
            UPDATE dbo.District
            SET PrimarySalespersonId = @SalespersonId
            WHERE DistrictId = @DistrictId";
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(sql, new { DistrictId = districtId, SalespersonId = salespersonId });
    }
}