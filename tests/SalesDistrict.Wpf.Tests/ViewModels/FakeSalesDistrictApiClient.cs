using System.Net.Http;
using SalesDistrict.Wpf.Api;
using SalesDistrict.Wpf.Models;

namespace SalesDistrict.Wpf.Tests.ViewModels;

internal sealed class FakeSalesDistrictApiClient : ISalesDistrictApiClient
{
    public static readonly District Nordjylland = new(1, "Nordjylland");
    public static readonly District Midtjylland = new(2, "Midtjylland");

    public static readonly Salesperson Anna = new(1, "Anna Jensen");
    public static readonly Salesperson Bo = new(2, "Bo Sørensen");
    public static readonly Salesperson Carl = new(3, "Carl Nielsen");
    public static readonly Salesperson Mathias = new(4, "Mathias Hansen");

    private static readonly Salesperson[] AllSalespersons = [Anna, Bo, Carl, Mathias];

    public bool ApiIsDown { get; init; }

    // When set, every update is rejected with this message, as the API does with a 400 ProblemDetails
    public string? RejectUpdatesWith { get; init; }

    public Func<int, Task<DistrictDetails>> GetDetails { get; init; } = districtId => Task.FromResult(DetailsFor(districtId));

    public Task<IReadOnlyList<District>> GetDistrictsAsync(CancellationToken cancellationToken = default)
        => ApiIsDown
            ? throw new HttpRequestException("No connection could be made")
            : Task.FromResult<IReadOnlyList<District>>([Nordjylland, Midtjylland]);

    public Task<DistrictDetails> GetDistrictDetailsAsync(int districtId, CancellationToken cancellationToken = default)
        => GetDetails(districtId);

    public Task<IReadOnlyList<Salesperson>> GetSalespersonsAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Salesperson>>(AllSalespersons);

    public Task<DistrictDetails> UpdateDistrictSalespersonsAsync(
        int districtId,
        int primarySalespersonId,
        IReadOnlyList<int> secondarySalespersonIds,
        CancellationToken cancellationToken = default)
    {
        if (RejectUpdatesWith is not null)
        {
            throw new HttpRequestException(RejectUpdatesWith);
        }

        // Answers like the API: the district as it looks after the update
        return Task.FromResult(new DistrictDetails(
            districtId,
            $"District {districtId}",
            AllSalespersons.Single(salesperson => salesperson.Id == primarySalespersonId),
            secondarySalespersonIds.Select(id => AllSalespersons.Single(salesperson => salesperson.Id == id)).ToList(),
            []));
    }

    // Every district starts with Anna as primary and Bo and Mathias as secondaries
    public static DistrictDetails DetailsFor(int districtId) => new(
        districtId,
        $"District {districtId}",
        Anna,
        [Bo, Mathias],
        [new Store(1, "Aalborg")]);
}