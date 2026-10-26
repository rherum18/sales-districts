using Microsoft.AspNetCore.Mvc;
using SalesDistrict.Application.Services;
using SalesDistrict.Domain;

namespace SalesDistrict.Api.Controllers;

[ApiController]
[Route("api/districts")]
public class DistrictsController : ControllerBase
{
    private readonly DistrictService _districtService;

    public DistrictsController(DistrictService districtService)
    {
        _districtService = districtService;
    }

    [HttpGet]
    public async Task<ActionResult<List<District>>> Districts()
    {
        var districts = await _districtService.GetDistrictsAsync();
        return Ok(districts);
    }

    [HttpPost]
    public async Task<ActionResult> CreateDistrict([FromBody] District district)
    {
        await _districtService.CreateDistrictAsync(district);
        return Ok();
    }

    [HttpPut("assign-primary-salesperson")]
    public async Task<ActionResult> AssignPrimarySalesperson(int districtId, int salespersonId)
    {
        await _districtService.AssignPrimarySalespersonToDistrictAsync(districtId, salespersonId);
        return Ok();
    }
}