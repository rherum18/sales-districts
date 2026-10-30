using SalesDistrict.Domain;

namespace SalesDistrict.Application.Abstractions;

public interface IDistrictRepository
{
    Task<IReadOnlyList<District>> GetAllAsync(CancellationToken cancellationToken);

    Task<DistrictDetails?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<bool> UpdateDistrictSalespersonsAsync(int districtId, int primarySalespersonId, IReadOnlyList<int> secondarySalespersonIds, CancellationToken cancellationToken);
}