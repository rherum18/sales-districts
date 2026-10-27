using SalesDistrict.Domain;

namespace SalesDistrict.Application.Services;

public class DistrictSecondarySalespersonService
{
    private readonly string _connectionString;

    public DistrictSecondarySalespersonService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task AssignSecondarySalesPersonsToDistrictAsync(int districtId, List<int> salespersonIds)
    {
    }
}