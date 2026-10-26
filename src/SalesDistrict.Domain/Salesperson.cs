using System;
using System.Collections.Generic;
using System.Text;

namespace SalesDistrict.Domain;

public class Salesperson
{
    public int SalespersonId { get; set; }
    public required string Name { get; init; }
}
