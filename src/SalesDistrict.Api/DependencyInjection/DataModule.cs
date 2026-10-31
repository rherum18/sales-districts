using Autofac;
using SalesDistrict.Data;

namespace SalesDistrict.Api.DependencyInjection;

public sealed class DataModule(string connectionString) : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterInstance(new SqlConnectionFactory(connectionString));

        builder.RegisterAssemblyTypes(typeof(SqlConnectionFactory).Assembly)
            .Where(type => type.Name.EndsWith("Repository"))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
    }
}