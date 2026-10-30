using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using SalesDistrict.Api.Contracts;

namespace SalesDistrict.Api.Tests.Integration;

[Collection(DatabaseCollection.Name)]
public class UpdateDistrictSalespersonsEndpointTests(SalesDistrictApiFactory factory) : IAsyncLifetime
{
    // xUnit creates a new instance per test, so every test starts from the seed data
    public async ValueTask InitializeAsync() => await factory.ResetDataAsync();

    // ...and leaves the seed data behind for the read-only tests in the same collection
    public async ValueTask DisposeAsync() => await factory.ResetDataAsync();

    [Fact]
    public async Task UpdateDistrictSalespersons_ValidAssignment_SavesAndReturnsDistrict()
    {
        // Arrange
        var client = factory.CreateClient();
        var request = new UpdateDistrictSalespersonsRequest(4, [2, 6]);

        // Act
        var response = await client.PutAsJsonAsync("/api/v1/districts/1/salespersons", request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var returned = await response.Content.ReadFromJsonAsync<DistrictDetailsResponse>(TestContext.Current.CancellationToken);
        var saved = await client.GetFromJsonAsync<DistrictDetailsResponse>("/api/v1/districts/1", TestContext.Current.CancellationToken);
        var expected = new DistrictDetailsResponse(
            1,
            "Nordjylland",
            new SalespersonSummaryResponse(4, "Kasper Sørensen"),
            [new(2, "Bo Sørensen"), new(6, "Morten Klausen")],
            [new(1, "Aalborg"), new(3, "Frederikshavn"), new(2, "Hjørring")]);
        Assert.Equivalent(expected, returned, strict: true);
        Assert.Equivalent(expected, saved, strict: true);
    }

    [Fact]
    public async Task UpdateDistrictSalespersons_PromoteSecondaryToPrimary_Succeeds()
    {
        // Arrange: in the seed, Bo Sørensen (2) is secondary in Sjælland (4).
        // This only works because the repository removes the secondaries before it changes the primary.
        var client = factory.CreateClient();
        var request = new UpdateDistrictSalespersonsRequest(2, [3]);

        // Act
        var response = await client.PutAsJsonAsync("/api/v1/districts/4/salespersons", request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var saved = await client.GetFromJsonAsync<DistrictDetailsResponse>("/api/v1/districts/4", TestContext.Current.CancellationToken);
        Assert.NotNull(saved);
        Assert.Equal(2, saved.PrimarySalesperson.Id);
        Assert.Equal([3], saved.SecondarySalespersons.Select(s => s.Id));
    }

    [Fact]
    public async Task UpdateDistrictSalespersons_EmptySecondaries_RemovesAllSecondaries()
    {
        // Arrange
        var client = factory.CreateClient();
        var request = new UpdateDistrictSalespersonsRequest(3, []);

        // Act
        var response = await client.PutAsJsonAsync("/api/v1/districts/4/salespersons", request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var saved = await client.GetFromJsonAsync<DistrictDetailsResponse>("/api/v1/districts/4", TestContext.Current.CancellationToken);
        Assert.NotNull(saved);
        Assert.Empty(saved.SecondarySalespersons);
    }

    [Fact]
    public async Task UpdateDistrictSalespersons_PrimaryAlsoSecondary_ReturnsBadRequestWithProblemDetails()
    {
        // Arrange
        var client = factory.CreateClient();
        var request = new UpdateDistrictSalespersonsRequest(1, [1]);

        // Act
        var response = await client.PutAsJsonAsync("/api/v1/districts/1/salespersons", request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problem);
        Assert.Contains("cannot also be a secondary", problem.Detail);
    }

    [Fact]
    public async Task UpdateDistrictSalespersons_UnknownPrimary_ReturnsBadRequestAndKeepsData()
    {
        // Arrange
        var client = factory.CreateClient();
        var request = new UpdateDistrictSalespersonsRequest(99, []);

        // Act
        var response = await client.PutAsJsonAsync("/api/v1/districts/4/salespersons", request, TestContext.Current.CancellationToken);

        // Assert: the error is reported, and the DELETE of the secondaries was rolled back
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        Assert.Equal("Primary salesperson 99 does not exist.", problem?.Detail);
        var unchanged = await client.GetFromJsonAsync<DistrictDetailsResponse>("/api/v1/districts/4", TestContext.Current.CancellationToken);
        Assert.NotNull(unchanged);
        Assert.Equal(3, unchanged.PrimarySalesperson.Id);
        Assert.Equal(3, unchanged.SecondarySalespersons.Count);
    }

    [Fact]
    public async Task UpdateDistrictSalespersons_UnknownSecondary_ReturnsBadRequest()
    {
        // Arrange
        var client = factory.CreateClient();
        var request = new UpdateDistrictSalespersonsRequest(5, [2, 99]);

        // Act
        var response = await client.PutAsJsonAsync("/api/v1/districts/2/salespersons", request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        Assert.Equal("Secondary salesperson 99 does not exist.", problem?.Detail);
    }

    [Fact]
    public async Task UpdateDistrictSalespersons_UnknownDistrict_ReturnsNotFound()
    {
        // Arrange
        var client = factory.CreateClient();
        var request = new UpdateDistrictSalespersonsRequest(1, [2]);

        // Act
        var response = await client.PutAsJsonAsync("/api/v1/districts/999/salespersons", request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateDistrictSalespersons_MissingSecondaryList_ReturnsBadRequest()
    {
        // Arrange
        var client = factory.CreateClient();
        var json = JsonContent.Create(new { primarySalespersonId = 1 });

        // Act
        var response = await client.PutAsync("/api/v1/districts/1/salespersons", json, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
