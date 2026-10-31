namespace SalesDistrict.Wpf.Api.Contracts;

public sealed record DistrictSummaryResponse(int Id, string Name);

public sealed record SalespersonSummaryResponse(int Id, string Name);

public sealed record StoreSummaryResponse(int Id, string Name);

public sealed record DistrictDetailsResponse
(
    int Id,
    string Name,
    SalespersonSummaryResponse PrimarySalesperson,
    IReadOnlyList<SalespersonSummaryResponse> SecondarySalespersons,
    IReadOnlyList<StoreSummaryResponse> Stores
);

public sealed record UpdateDistrictSalespersonsRequest(int PrimarySalespersonId, IReadOnlyList<int> SecondarySalespersonIds);

public sealed record ProblemDetailsResponse(string? Title, string? Detail);