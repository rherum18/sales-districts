namespace SalesDistrict.Api.Contracts;

public sealed record DistrictDetailsResponse(
    int Id, 
    string Name, 
    SalespersonSummaryResponse primarySalesperson, 
    List<SalespersonSummaryResponse> secondarySalespersons,
    List<StoreSummaryResponse> stores
);