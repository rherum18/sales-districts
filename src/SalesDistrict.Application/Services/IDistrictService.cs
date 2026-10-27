using System;
using System.Collections.Generic;
using System.Text;
using SalesDistrict.Domain;

namespace SalesDistrict.Application.Services
{
     public interface IDistrictService
    {
        Task<IReadOnlyList<District>> GetDistrictsAsync(CancellationToken cancellationToken = default);
    }
}
