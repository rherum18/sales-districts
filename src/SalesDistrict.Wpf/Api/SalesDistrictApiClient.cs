using System.Net.Http;
using System.Net.Http.Json;
using SalesDistrict.Wpf.Api.Contracts;
using SalesDistrict.Wpf.Models;

namespace SalesDistrict.Wpf.Api;

public sealed class SalesDistrictApiClient : ISalesDistrictApiClient
{
    private readonly HttpClient _httpClient;

    public SalesDistrictApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<District>> GetDistrictsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<IReadOnlyList<DistrictSummaryResponse>>("api/v1/districts", cancellationToken) ?? [];
        return response.Select(district => new District(district.Id, district.Name)).ToList();
    }

    public async Task<DistrictDetails> GetDistrictDetailsAsync(int districtId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<DistrictDetailsResponse>($"api/v1/districts/{districtId}", cancellationToken)
            ?? throw new HttpRequestException($"The API returned no details for district {districtId}.");

        return MapDistrictDetails(response);
    }

    public async Task<IReadOnlyList<Salesperson>> GetSalespersonsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<IReadOnlyList<SalespersonSummaryResponse>>("api/v1/salespersons", cancellationToken) ?? [];
        return response.Select(MapSalesperson).ToList();
    }

    public async Task<DistrictDetails> UpdateDistrictSalespersonsAsync(
        int districtId,
        int primarySalespersonId,
        IReadOnlyList<int> secondarySalespersonIds,
        CancellationToken cancellationToken = default)
    {
        var request = new UpdateDistrictSalespersonsRequest(primarySalespersonId, secondarySalespersonIds);
        using var response = await _httpClient.PutAsJsonAsync($"api/v1/districts/{districtId}/salespersons", request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw await CreateExceptionAsync(response, cancellationToken);
        }

        var details = await response.Content.ReadFromJsonAsync<DistrictDetailsResponse>(cancellationToken)
            ?? throw new HttpRequestException($"The API returned no details for district {districtId}.");

        return MapDistrictDetails(details);
    }

    // Uses the API's own explanation when it sends one, e.g. "Primary salesperson 3 cannot also be a secondary salesperson."
    private static async Task<HttpRequestException> CreateExceptionAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var problem = response.Content.Headers.ContentType?.MediaType == "application/problem+json"
            ? await response.Content.ReadFromJsonAsync<ProblemDetailsResponse>(cancellationToken)
            : null;

        var message = problem?.Detail ?? problem?.Title ?? $"The API responded {(int)response.StatusCode} {response.ReasonPhrase}.";
        return new HttpRequestException(message, null, response.StatusCode);
    }

    private static DistrictDetails MapDistrictDetails(DistrictDetailsResponse response)
        => new(
            response.Id,
            response.Name,
            MapSalesperson(response.PrimarySalesperson),
            response.SecondarySalespersons.Select(MapSalesperson).ToList(),
            response.Stores.Select(store => new Store(store.Id, store.Name)).ToList());

    private static Salesperson MapSalesperson(SalespersonSummaryResponse salesperson)
        => new(salesperson.Id, salesperson.Name);
}