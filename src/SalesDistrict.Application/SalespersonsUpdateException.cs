namespace SalesDistrict.Application;

public sealed class SalespersonsUpdateException : Exception
{
    public SalespersonsUpdateException()
    {
    }

    public SalespersonsUpdateException(string message)
        : base(message)
    {
    }

    public SalespersonsUpdateException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
