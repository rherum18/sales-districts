using SalesDistrict.Application.Abstractions;
using SalesDistrict.Domain;

namespace SalesDistrict.Application.Services;

public sealed class DistrictsService : IDistrictsService
{
    private readonly IDistrictRepository _repository;

    public DistrictsService(IDistrictRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<District>> GetDistrictsAsync(CancellationToken cancellationToken = default)
        => _repository.GetAllAsync(cancellationToken);

    public Task<DistrictDetails?> GetDistrictAsync(int id, CancellationToken cancellationToken = default)
        => _repository.GetByIdAsync(id, cancellationToken);

    async public Task<DistrictDetails?> UpdateDistrictSalespersonsAsync(int districtId, int primarySalespersonId, IReadOnlyList<int> secondarySalespersonIds, CancellationToken cancellationToken = default)
    {
        ValidateUpdate(primarySalespersonId, secondarySalespersonIds);

        var districtWasUpdated = await _repository.UpdateDistrictSalespersonsAsync(districtId, primarySalespersonId, secondarySalespersonIds, cancellationToken);

        if (!districtWasUpdated)
        {
            return null;
        }

        return await _repository.GetByIdAsync(districtId, cancellationToken);
    }

    private static void ValidateUpdate(int primarySalespersonId, IReadOnlyList<int> secondarySalespersonIds)
    {
        if (secondarySalespersonIds.Contains(primarySalespersonId))
        {
            throw new SalespersonsUpdateException($"Invalid salesperson assignment: Primary salesperson {primarySalespersonId} cannot also be a secondary salesperson.");
        }
        if (secondarySalespersonIds.Distinct().Count() != secondarySalespersonIds.Count)
        {
            var duplicateIds = secondarySalespersonIds.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key);
            throw new SalespersonsUpdateException($"Invalid salesperson assignment: Duplicate salespersons found with IDs: {string.Join(", ", duplicateIds)}");
        }
    }
}