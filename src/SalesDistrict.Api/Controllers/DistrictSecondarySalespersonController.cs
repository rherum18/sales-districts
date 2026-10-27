using Microsoft.AspNetCore.Mvc;
using SalesDistrict.Application.Services;

namespace SalesDistrict.Api.Controllers;

[ApiController]
[Route("api/v1/{districtId}/secondary-salespersons")]
public class DistrictSecondarySalespersonController : ControllerBase
{
    private readonly DistrictSecondarySalespersonService _districtSecondarySalespersonService;

    public DistrictSecondarySalespersonController(DistrictSecondarySalespersonService districtsSecondarySalespersonService)
    {
        _districtSecondarySalespersonService = districtsSecondarySalespersonService;
    }

    [HttpPut]
    public async Task<ActionResult> AssignSecondarySalespersons(int districtId, List<int> salespersonIds)
    {
        await _districtSecondarySalespersonService.AssignSecondarySalesPersonsToDistrictAsync(districtId, salespersonIds);
        return Ok();
    }
}