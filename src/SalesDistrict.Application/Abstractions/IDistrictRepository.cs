using SalesDistrict.Domain;

namespace SalesDistrict.Application.Abstractions;

public interface IDistrictRepository
{
    Task<IReadOnlyList<District>> GetAllAsync(CancellationToken cancellationToken);
}