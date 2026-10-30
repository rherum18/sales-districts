using SalesDistrict.Api.Contracts;
using System.Net.Http.Json;

namespace SalesDistrict.Api.Tests.Integration;

[Collection(DatabaseCollection.Name)]
public class DistrictsEndpointTests(SalesDistrictApiFactory factory)
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

    [Fact]
    public async Task GetDistrictDetails_ReturnsSecondariesAndStores()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var districtDetails = await client.GetFromJsonAsync<DistrictDetailsResponse>("/api/v1/districts/4", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(districtDetails);
        Assert.NotNull(districtDetails.PrimarySalesperson);
        Assert.NotNull(districtDetails.SecondarySalespersons);
        Assert.NotNull(districtDetails.Stores);
        Assert.Equivalent(GetExpectedDistrictDetails(), districtDetails, strict: true);
    }

    [Fact]
    public async Task GetDistrictWithNoStores_ReturnsEmptyList()
    {
        // Arrange
        var client = factory.CreateClient();
        
        // Act
        var districtDetails = await client.GetFromJsonAsync<DistrictDetailsResponse>("/api/v1/districts/3", TestContext.Current.CancellationToken);
        
        // Assert
        Assert.NotNull(districtDetails);
        Assert.Equal(3, districtDetails.Id);
        Assert.Equal("Sønderjylland", districtDetails.Name);
        Assert.NotNull(districtDetails.PrimarySalesperson);
        Assert.Equal(6, districtDetails.PrimarySalesperson.Id);
        Assert.Equal("Morten Klausen", districtDetails.PrimarySalesperson.Name);
        Assert.NotNull(districtDetails.SecondarySalespersons);
        Assert.Empty(districtDetails.SecondarySalespersons);
        Assert.NotNull(districtDetails.Stores);
        Assert.Empty(districtDetails.Stores);
    }

    [Fact]
    public async Task GetDistrictsWithWrongId_ReturnsNotFound()
    {
        //Arrange
        var client = factory.CreateClient();

        //Act
        var response = await client.GetAsync("/api/v1/districts/999", TestContext.Current.CancellationToken);

        //Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
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
    DistrictDetailsResponse GetExpectedDistrictDetails()
    {
        return new DistrictDetailsResponse
        (
            4,
            "Sjælland",
            new SalespersonSummaryResponse(3, "Anna Jensen"),
            new List<SalespersonSummaryResponse>
            {
                new SalespersonSummaryResponse(1, "Anna Jensen"),
                new SalespersonSummaryResponse(2, "Bo Sørensen"),
                new SalespersonSummaryResponse(5, "Lars Jespersen")
            },
            new List<StoreSummaryResponse>
            {
                new StoreSummaryResponse(10, "Kalundborg"),
                new StoreSummaryResponse(7, "København"),
                new StoreSummaryResponse(8, "Næstved"),
                new StoreSummaryResponse(6, "Roskilde"),
                new StoreSummaryResponse(9, "Slagelse")
            }
        );
    }
}

