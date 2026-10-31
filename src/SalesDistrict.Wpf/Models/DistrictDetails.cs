namespace SalesDistrict.Wpf.Models;

public sealed record DistrictDetails
(
    int Id,
    string Name,
    Salesperson PrimarySalesperson,
    IReadOnlyList<Salesperson> SecondarySalespersons,
    IReadOnlyList<Store> Stores
);