using System.Net.Http.Json;
using SalesDistrict.Api.Contracts;

namespace SalesDistrict.Api.Tests.Integration;

[Collection(DatabaseCollection.Name)]
public class SalespersonsEndpointTests(SalesDistrictApiFactory factory)
{
    [Fact]
    public async Task GetSalespersons_ReturnsSeededSalespersonsOrderedByName()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var salespersons = await client.GetFromJsonAsync<List<SalespersonSummaryResponse>>("/api/v1/salespersons", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equivalent(GetExpectedSalespersons(), salespersons, strict: true);
    }

    List<SalespersonSummaryResponse> GetExpectedSalespersons()
    {
        return new List<SalespersonSummaryResponse>
        {
            new SalespersonSummaryResponse(1, "Anna Jensen"),
            new SalespersonSummaryResponse(3, "Anna Jensen"),
            new SalespersonSummaryResponse(2, "Bo Sørensen"),
            new SalespersonSummaryResponse(4, "Kasper Sørensen"),
            new SalespersonSummaryResponse(5, "Lars Jespersen"),
            new SalespersonSummaryResponse(6, "Morten Klausen"),
        };
    }
}