namespace SalesDistrict.Api.Contracts;

public sealed record UpdateDistrictSalespersonsRequest(int PrimarySalespersonId, IReadOnlyList<int> SecondarySalespersonIds);