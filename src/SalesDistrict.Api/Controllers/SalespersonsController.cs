using Microsoft.AspNetCore.Mvc;
using SalesDistrict.Api.Contracts;
using SalesDistrict.Application.Services;

namespace SalesDistrict.Api.Controllers;

[ApiController]
[Route("api/v1/salespersons")]
public class SalespersonsController : ControllerBase
{
    private readonly ISalespersonsService _salespersonsService;

    public SalespersonsController(ISalespersonsService salespersonsService)
    {
        _salespersonsService = salespersonsService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SalespersonSummaryResponse>>> GetSalespersons(CancellationToken cancellationToken)
    {
        var salespersons = await _salespersonsService.GetSalespersonsAsync(cancellationToken);
        var response = salespersons
            .Select(s => new SalespersonSummaryResponse(s.SalespersonId, s.Name))
            .ToList();
        return Ok(response);
    }
}

