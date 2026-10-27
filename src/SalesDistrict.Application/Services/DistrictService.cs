using SalesDistrict.Application.Abstractions;
using SalesDistrict.Domain;

namespace SalesDistrict.Application.Services;

public sealed class DistrictService : IDistrictService
{
    private readonly IDistrictRepository _repository;

    public DistrictService(IDistrictRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<District>> GetDistrictsAsync(CancellationToken cancellationToken = default)
        => _repository.GetAllAsync(cancellationToken);

}