using SalesDistrict.Application;
using SalesDistrict.Application.Abstractions;
using SalesDistrict.Application.Services;
using SalesDistrict.Domain;

namespace SalesDistrict.Api.Tests.ServiceTests;

public class DistrictsServiceTests
{
    private sealed class FakeDistrictRepository(bool districtExists = true) : IDistrictRepository
    {
        public int UpdateCalls { get; private set; }

        public Task<IReadOnlyList<District>> GetAllAsync(CancellationToken cancellationToken)
            => Task.FromResult<IReadOnlyList<District>>([]);

        public Task<DistrictDetails?> GetByIdAsync(int id, CancellationToken cancellationToken)
            => Task.FromResult<DistrictDetails?>(new DistrictDetails
            {
                DistrictId = id,
                Name = "TestDistrict",
                PrimarySalesperson = new Salesperson { SalespersonId = 1, Name = "TestPerson" },
                SecondarySalespersons = [],
                Stores = [],
            });

        public Task<bool> UpdateDistrictSalespersonsAsync(int districtId, int primarySalespersonId, IReadOnlyList<int> secondarySalespersonIds, CancellationToken cancellationToken)
        {
            UpdateCalls++;
            return Task.FromResult(districtExists);
        }
    }

    [Fact]
    public async Task UpdateDistrictSalespersons_PrimaryAlsoSecondary_ThrowsAndDoesNotWrite()
    {
        // Arrange
        var repository = new FakeDistrictRepository();
        var service = new DistrictsService(repository);

        // Act
        var ex = await Assert.ThrowsAsync<SalespersonsUpdateException>(
            () => service.UpdateDistrictSalespersonsAsync(1, 3, [2, 3], TestContext.Current.CancellationToken));

        // Assert
        Assert.Contains("Primary salesperson 3", ex.Message);
        Assert.Equal(0, repository.UpdateCalls);
    }

    [Fact]
    public async Task UpdateDistrictSalespersons_DuplicateSecondaries_ThrowsAndDoesNotWrite()
    {
        // Arrange
        var repository = new FakeDistrictRepository();
        var service = new DistrictsService(repository);

        // Act
        var ex = await Assert.ThrowsAsync<SalespersonsUpdateException>(
            () => service.UpdateDistrictSalespersonsAsync(1, 1, [2, 4, 2], TestContext.Current.CancellationToken));

        // Assert
        Assert.Contains("IDs: 2", ex.Message);
        Assert.Equal(0, repository.UpdateCalls);
    }

    [Fact]
    public async Task UpdateDistrictSalespersons_UnknownDistrict_ReturnsNull()
    {
        // Arrange
        var service = new DistrictsService(new FakeDistrictRepository(districtExists: false));

        // Act
        var result = await service.UpdateDistrictSalespersonsAsync(999, 1, [2], TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateDistrictSalespersons_ValidAssignment_ReturnsReloadedDistrict()
    {
        // Arrange
        var repository = new FakeDistrictRepository();
        var service = new DistrictsService(repository);

        // Act
        var result = await service.UpdateDistrictSalespersonsAsync(7, 1, [2], TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(7, result.DistrictId);
        Assert.Equal(1, repository.UpdateCalls);
    }
}
