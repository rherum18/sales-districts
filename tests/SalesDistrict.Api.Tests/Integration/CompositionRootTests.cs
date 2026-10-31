using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace SalesDistrict.Api.Tests.Integration;

[Collection(DatabaseCollection.Name)]
public class CompositionRootTests(SalesDistrictApiFactory factory)
{
    public static TheoryData<Type> ControllerTypes => new(
        typeof(Program).Assembly.GetTypes()
            .Where(type => type.IsSubclassOf(typeof(ControllerBase)) && !type.IsAbstract));

    [Theory]
    [MemberData(nameof(ControllerTypes))]
    public void Container_ResolvesController(Type controllerType)
    {
        // Arrange
        using var scope = factory.Services.CreateScope();

        // Act
        var controller = scope.ServiceProvider.GetRequiredService(controllerType);

        // Assert
        Assert.IsType(controllerType, controller);
    }
}