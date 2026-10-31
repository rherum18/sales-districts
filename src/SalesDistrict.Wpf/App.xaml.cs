using System.Net.Http;
using System.Windows;
using Autofac;
using Microsoft.Extensions.Configuration;
using SalesDistrict.Wpf.Api;
using SalesDistrict.Wpf.ViewModels;

namespace SalesDistrict.Wpf;

public partial class App : Application
{
    private IContainer? _container;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
        var apiBaseAddress = configuration["Api:BaseAddress"]
            ?? throw new InvalidOperationException("Missing setting 'Api:BaseAddress'.");

        var builder = new ContainerBuilder();
        // One HttpClient for the whole app, so connections are reused instead of opened per call
        builder.RegisterInstance(new HttpClient { BaseAddress = new Uri(apiBaseAddress) });
        builder.RegisterType<SalesDistrictApiClient>().As<ISalesDistrictApiClient>().SingleInstance();
        builder.RegisterType<MainViewModel>().SingleInstance();
        builder.RegisterType<MainWindow>();
        _container = builder.Build();

        _container.Resolve<MainWindow>().Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _container?.Dispose();
        base.OnExit(e);
    }
}