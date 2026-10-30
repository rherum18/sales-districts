using SalesDistrict.Domain;

namespace SalesDistrict.Application.Services;

public interface IDistrictsService
{
    Task<IReadOnlyList<District>> GetDistrictsAsync(CancellationToken cancellationToken = default);
    Task<DistrictDetails?> GetDistrictAsync(int id, CancellationToken cancellationToken = default);
    Task<DistrictDetails?> UpdateDistrictSalespersonsAsync(int districtId, int primarySalespersonId, IReadOnlyList<int> secondarySalespersonIds, CancellationToken cancellationToken = default);
}