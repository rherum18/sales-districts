using Autofac;
using SalesDistrict.Data;

namespace SalesDistrict.Api.DependencyInjection;

public sealed class DataModule : Module
{
    private readonly string _connectionString;

    public DataModule(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterInstance(new SqlConnectionFactory(_connectionString));

        builder.RegisterAssemblyTypes(typeof(SqlConnectionFactory).Assembly)
            .Where(type => type.Name.EndsWith("Repository"))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
    }
}