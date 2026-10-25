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
    public async Task<ActionResult<IEnumerable<District>>> Get()
    {
        var districts = await _districtService.GetDistrictsAsync();
        return Ok(districts);
    }
}