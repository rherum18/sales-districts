using SalesDistrict.Wpf.Models;

namespace SalesDistrict.Wpf.Api;

public interface ISalesDistrictApiClient
{
    Task<IReadOnlyList<District>> GetDistrictsAsync(CancellationToken cancellationToken = default);

    Task<DistrictDetails> GetDistrictDetailsAsync(int districtId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Salesperson>> GetSalespersonsAsync(CancellationToken cancellationToken = default);

    Task<DistrictDetails> UpdateDistrictSalespersonsAsync
    (
        int districtId,
        int primarySalespersonId,
        IReadOnlyList<int> secondarySalespersonIds,
        CancellationToken cancellationToken = default
    );
}