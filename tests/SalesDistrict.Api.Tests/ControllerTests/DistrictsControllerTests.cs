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

        public Task<DistrictDetails?> UpdateDistrictSalespersonsAsync(int districtId, int primarySalespersonId, IReadOnlyList<int> secondarySalespersonIds, CancellationToken cancellationToken = default)
            => Task.FromResult(details?.DistrictId == districtId ? details : null);
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
        Assert.Equal(expected, returnedDistricts);
    }

    [Fact]
    public async Task GetDistrictsWithWrongId_ReturnsNotFound()
    {
        // Arrange
        var controller = new DistrictsController(new FakeDistrictsService());

        // Act
        var result = await controller.GetDistrict(999, TestContext.Current.CancellationToken);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task UpdateDistrictSalespersons_UnknownDistrict_ReturnsNotFound()
    {
        // Arrange
        var controller = new DistrictsController(new FakeDistrictsService());

        // Act
        var result = await controller.UpdateDistrictSalespersons(999, new UpdateDistrictSalespersonsRequest(1, [2]), TestContext.Current.CancellationToken);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task UpdateDistrictSalespersons_KnownDistrict_MapsDetailsToResponse()
    {
        // Arrange
        var details = new DistrictDetails
        {
            DistrictId = 7,
            Name = "TestDistrict",
            PrimarySalesperson = new Salesperson { SalespersonId = 1, Name = "Primary" },
            SecondarySalespersons = [new Salesperson { SalespersonId = 2, Name = "Secondary" }],
            Stores = [new Store { StoreId = 3, Name = "Store", DistrictId = 7 }],
        };
        var controller = new DistrictsController(new FakeDistrictsService(details: details));

        // Act
        var result = await controller.UpdateDistrictSalespersons(7, new UpdateDistrictSalespersonsRequest(1, [2]), TestContext.Current.CancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var expected = new DistrictDetailsResponse(7, "TestDistrict", new(1, "Primary"), [new(2, "Secondary")], [new(3, "Store")]);
        Assert.Equivalent(expected, okResult.Value, strict: true);
    }
}
