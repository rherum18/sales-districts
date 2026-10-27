using SalesDistrict.Domain;

namespace SalesDistrict.Application.Abstractions;

public interface ISalespersonRepository
{
    Task<IReadOnlyList<Salesperson>> GetAllAsync(CancellationToken cancellationToken);
}