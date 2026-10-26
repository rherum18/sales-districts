using Microsoft.AspNetCore.Mvc;
using SalesDistrict.Domain;

namespace SalesDistrict.Api.Controllers
{
    [ApiController]
    [Route("api/salespersons")]
    public class SalespersonController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> Create(Salesperson salesperson, int districtId)
        {
            
            return Ok();
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int salespersonId)
        {
            // Implement the delete logic here
            return Ok();
        }
    }
}
