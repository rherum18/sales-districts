using Microsoft.AspNetCore.Mvc;
using SalesDistrict.Api.Contracts;
using SalesDistrict.Api.Controllers;
using SalesDistrict.Application.Services;
using SalesDistrict.Domain;

namespace SalesDistrict.Api.Tests.ControllerTests;

public class DistrictsControllerTests
{
    private sealed class FakeDistrictsService(
    IReadOnlyList<District>? districts = null,
    DistrictDetails? details = null) : IDistrictsService
    {
        public Task<IReadOnlyList<District>> GetDistrictsAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(districts ?? []);

        public Task<DistrictDetails?> GetDistrictAsync(int id, CancellationToken cancellationToken = default)
            => Task.FromResult(details?.DistrictId == id ? details : null);
    }

    [Fact]
    public async Task GetDistricts_ReturnsOkResult_WithDistricts()
    {
        // Arrange
        var districts = new List<District>
        {
            new() { DistrictId = 7, Name = "TestName1", PrimarySalespersonId = 2 },
            new() { DistrictId = 8, Name = "TestName2", PrimarySalespersonId = 4 },
        };
        var controller = new DistrictsController(new FakeDistrictsService(districts));

        // Act
        var result = await controller.GetDistricts(TestContext.Current.CancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedDistricts = Assert.IsAssignableFrom<IReadOnlyList<DistrictSummaryResponse>>(okResult.Value);
        List<DistrictSummaryResponse> expected = [new(7, "TestName1"), new(8, "TestName2")];
        Assert.Equal(expected, returnedDistricts); ;
    }
}
