using Microsoft.AspNetCore.Mvc;
using SalesDistrict.Api.Contracts;
using SalesDistrict.Application.Services;

namespace SalesDistrict.Api.Controllers;

[ApiController]
[Route("api/v1/districts")]
public class DistrictsController : ControllerBase
{
    private readonly IDistrictService _districtService;

    public DistrictsController(IDistrictService districtService)
    {
        _districtService = districtService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DistrictSummaryResponse>>> GetDistricts(CancellationToken cancellationToken)
    {
        var districts = await _districtService.GetDistrictsAsync(cancellationToken);
        var response = districts
            .Select(d => new DistrictSummaryResponse(d.DistrictId, d.Name))
            .ToList();
        return Ok(response);
    }
}