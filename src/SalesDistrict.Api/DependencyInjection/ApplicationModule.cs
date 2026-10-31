using Autofac;
using SalesDistrict.Application.Services;

namespace SalesDistrict.Api.DependencyInjection;
public sealed class ApplicationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(typeof(DistrictsService).Assembly)
            .Where(type => type.Name.EndsWith("Service"))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
    }
}