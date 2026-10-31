namespace SalesDistrict.Api.Contracts;

public sealed record DistrictDetailsResponse(
    int Id, 
    string Name, 
    SalespersonSummaryResponse PrimarySalesperson, 
    IReadOnlyList<SalespersonSummaryResponse> SecondarySalespersons,
    IReadOnlyList<StoreSummaryResponse> Stores
);