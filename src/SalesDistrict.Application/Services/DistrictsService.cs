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

}