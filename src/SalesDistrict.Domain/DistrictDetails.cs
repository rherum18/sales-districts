using SalesDistrict.Domain;

public class DistrictDetails
{
    public int DistrictId { get; init; }
    public required string Name { get; init; }
    public required Salesperson PrimarySalesperson { get; init; }
    public required IReadOnlyList<Salesperson> SecondarySalespersons { get; init; }
    public required IReadOnlyList<Store> Stores { get; init; }
}