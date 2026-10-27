using System;
using System.Collections.Generic;
using System.Text;
using SalesDistrict.Domain;

namespace SalesDistrict.Application.Services
{
    public interface ISalespersonService
    {
        Task<IReadOnlyList<Salesperson>> GetSalespersonsAsync(CancellationToken cancellationToken = default);
    }
}
