using SalesDistrict.Application.Abstractions;
using SalesDistrict.Domain;

namespace SalesDistrict.Application.Services;

public sealed class SalespersonsService : ISalespersonsService
{
    private readonly ISalespersonRepository _repository;

    public SalespersonsService(ISalespersonRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<Salesperson>> GetSalespersonsAsync(CancellationToken cancellationToken = default)
        => _repository.GetAllAsync(cancellationToken);

}