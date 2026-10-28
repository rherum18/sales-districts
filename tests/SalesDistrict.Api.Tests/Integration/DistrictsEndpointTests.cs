using Microsoft.AspNetCore.Mvc.Testing;
using SalesDistrict.Api.Contracts;
using System.Net.Http.Json;

namespace SalesDistrict.Api.Tests.Integration;

public class DistrictsEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetDistricts_ReturnsSeededDistrictsOrderedByName()
    {
        // Arrange
        var client = factory.CreateClient();
        
        // Act
        var districts = await client.GetFromJsonAsync<List<DistrictSummaryResponse>>("/api/v1/districts", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(districts);
        Assert.NotEmpty(districts);
        Assert.Equal(4, districts.Count);
        Assert.Equal(GetExpectedDistricts(), districts);
    }

    List<DistrictSummaryResponse> GetExpectedDistricts()
    {
        return new List<DistrictSummaryResponse>
        {
            new DistrictSummaryResponse(2, "Midtjylland"),
            new DistrictSummaryResponse(1, "Nordjylland"),
            new DistrictSummaryResponse(4, "Sjælland"),
            new DistrictSummaryResponse(3, "Sønderjylland")
        };
    }
}

