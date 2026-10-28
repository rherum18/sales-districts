using SalesDistrict.Domain;

namespace SalesDistrict.Application.Services;

public interface ISalespersonsService
{
    Task<IReadOnlyList<Salesperson>> GetSalespersonsAsync(CancellationToken cancellationToken = default);
}