using Microsoft.Data.SqlClient;

namespace SalesDistrict.Data;

public sealed class SqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public SqlConnection Create() => new SqlConnection(_connectionString);
}