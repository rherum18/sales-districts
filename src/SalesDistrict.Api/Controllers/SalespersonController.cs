using Microsoft.AspNetCore.Mvc;
using SalesDistrict.Api.Contracts;
using SalesDistrict.Application.Services;

namespace SalesDistrict.Api.Controllers;

[ApiController]
[Route("api/v1/salespersons")]
public class SalespersonController : ControllerBase
{
    private readonly ISalespersonService _salespersonService;

    public SalespersonController(ISalespersonService salespersonService)
    {
        _salespersonService = salespersonService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SalespersonSummaryResponse>>> GetSalespersons(CancellationToken cancellationToken)
    {
        var salespersons = await _salespersonService.GetSalespersonsAsync(cancellationToken);
        var response = salespersons
            .Select(s => new SalespersonSummaryResponse(s.SalespersonId, s.Name))
            .ToList();
        return Ok(response);
    }
}

