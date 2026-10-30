namespace SalesDistrict.Api.Contracts;

public sealed record DistrictDetailsResponse(
    int Id, 
    string Name, 
    SalespersonSummaryResponse PrimarySalesperson, 
    List<SalespersonSummaryResponse> SecondarySalespersons,
    List<StoreSummaryResponse> Stores
);