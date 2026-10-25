namespace SalesDistrict.Domain;

public class District
{
    public int DistrictId { get; set; }
    public required string Name { get; init; }
    public int PrimarySalespersonId { get; set; }
}