using Microsoft.AspNetCore.Mvc;
using SalesDistrict.Api.Contracts;
using SalesDistrict.Application.Services;
using SalesDistrict.Domain;

namespace SalesDistrict.Api.Controllers;

[ApiController]
[Route("api/v1/districts")]
public class DistrictsController : ControllerBase
{
    private readonly IDistrictsService _districtsService;

    public DistrictsController(IDistrictsService districtsService)
    {
        _districtsService = districtsService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DistrictSummaryResponse>>> GetDistricts(CancellationToken cancellationToken)
    {
        var districts = await _districtsService.GetDistrictsAsync(cancellationToken);
        var response = districts
            .Select(d => new DistrictSummaryResponse(d.DistrictId, d.Name))
            .ToList();
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DistrictDetailsResponse>> GetDistrict(int id, CancellationToken cancellationToken)
    {
        var district = await _districtsService.GetDistrictAsync(id, cancellationToken);

        if (district == null)
        {
            return NotFound();
        }

        var response = MapToResponse(district);

        return Ok(response);
    }

    [HttpPut("{id:int}/salespersons")]
    public async Task<ActionResult<DistrictDetailsResponse>> UpdateDistrictSalespersons(int id, UpdateDistrictSalespersonsRequest request, CancellationToken cancellationToken)
    {
        var district = await _districtsService.UpdateDistrictSalespersonsAsync(id, request.PrimarySalespersonId, request.SecondarySalespersonIds, cancellationToken);

        if (district == null)
        {
            return NotFound();
        }

        var response = MapToResponse(district);
        
        return Ok(response);
    }

    private static DistrictDetailsResponse MapToResponse(DistrictDetails district)
    {
        return new DistrictDetailsResponse(
            district.DistrictId,
            district.Name,
            new SalespersonSummaryResponse(district.PrimarySalesperson.SalespersonId, district.PrimarySalesperson.Name),
            district.SecondarySalespersons.Select(s => new SalespersonSummaryResponse(s.SalespersonId, s.Name)).ToList(),
            district.Stores.Select(st => new StoreSummaryResponse(st.StoreId, st.Name)).ToList()
        );
    }
}