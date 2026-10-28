namespace SalesDistrict.Domain;

public class Store
{
    public int StoreId { get; set; }
    public required string Name { get; init; }
    public int DistrictId { get; set; }
}