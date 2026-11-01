using SalesDistrict.Wpf.Models;
using SalesDistrict.Wpf.ViewModels;
using static SalesDistrict.Wpf.Tests.ViewModels.FakeSalesDistrictApiClient;

namespace SalesDistrict.Wpf.Tests.ViewModels;

public class MainViewModelTests
{
    [Fact]
    public async Task Load_FillsDistrictAndSalespersonLists()
    {
        // Arrange
        var viewModel = new MainViewModel(new FakeSalesDistrictApiClient());

        // Act
        await viewModel.LoadCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal([Nordjylland, Midtjylland], viewModel.Districts);
        Assert.Equal([Anna, Bo, Carl, Mathias], viewModel.Salespersons);
        Assert.Null(viewModel.ErrorMessage);
    }

    [Fact]
    public async Task Load_ApiIsDown_ShowsErrorMessage()
    {
        // Arrange
        var viewModel = new MainViewModel(new FakeSalesDistrictApiClient { ApiIsDown = true });

        // Act
        await viewModel.LoadCommand.ExecuteAsync(null);

        // Assert
        Assert.Empty(viewModel.Districts);
        Assert.StartsWith("Could not load districts", viewModel.ErrorMessage);
    }

    [Fact]
    public async Task SelectingDistrict_LoadsItsDetails()
    {
        // Arrange
        var viewModel = new MainViewModel(new FakeSalesDistrictApiClient());

        // Act
        viewModel.SelectedDistrict = Midtjylland;
        await viewModel.LoadDistrictDetailsCommand.ExecutionTask!;

        // Assert
        Assert.NotNull(viewModel.SelectedDistrictDetails);
        Assert.Equal(Midtjylland.Id, viewModel.SelectedDistrictDetails.Id);
    }

    [Fact]
    public async Task SelectingAnotherDistrictWhileLoading_ShowsTheLastSelectedDistrict()
    {
        // Arrange
        var slowNordjylland = new TaskCompletionSource<DistrictDetails>();
        var viewModel = new MainViewModel(new FakeSalesDistrictApiClient
        {
            GetDetails = districtId => districtId == Nordjylland.Id ? slowNordjylland.Task : Task.FromResult(DetailsFor(districtId)),
        });

        // Act
        viewModel.SelectedDistrict = Nordjylland;
        var nordjyllandLoad = viewModel.LoadDistrictDetailsCommand.ExecutionTask!;
        viewModel.SelectedDistrict = Midtjylland;
        slowNordjylland.SetResult(DetailsFor(Nordjylland.Id));
        await nordjyllandLoad;

        // Assert
        Assert.Equal(Midtjylland.Id, viewModel.SelectedDistrictDetails?.Id);
    }

    [Fact]
    public async Task AssignCommands_AreDisabledUntilSalespersonIsSelected()
    {
        // Arrange
        var viewModel = await CreateWithNordjyllandSelectedAsync(new FakeSalesDistrictApiClient());

        // Act & Assert
        Assert.False(viewModel.SetAsPrimaryCommand.CanExecute(null));
        viewModel.SelectedSalesperson = Carl;
        Assert.True(viewModel.SetAsPrimaryCommand.CanExecute(null));
    }

    [Fact]
    public async Task SetAsPrimary_PromotesSecondary_AndRemovesItFromSecondaries()
    {
        // Arrange
        var viewModel = await CreateWithNordjyllandSelectedAsync(new FakeSalesDistrictApiClient());
        viewModel.SelectedSalesperson = Bo;

        // Act
        await viewModel.SetAsPrimaryCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal(Bo, viewModel.SelectedDistrictDetails?.PrimarySalesperson);
        Assert.Equal([Mathias], viewModel.SelectedDistrictDetails!.SecondarySalespersons);
    }

    [Fact]
    public async Task AddAsSecondary_KeepsExistingSecondaries()
    {
        // Arrange
        var viewModel = await CreateWithNordjyllandSelectedAsync(new FakeSalesDistrictApiClient());
        viewModel.SelectedSalesperson = Carl;

        // Act
        await viewModel.AddAsSecondaryCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal(Anna, viewModel.SelectedDistrictDetails?.PrimarySalesperson);
        Assert.Equal([Bo, Mathias, Carl], viewModel.SelectedDistrictDetails!.SecondarySalespersons);
    }

    [Fact]
    public async Task RemoveSecondary_RemovesOnlyThatSalesperson()
    {
        // Arrange
        var viewModel = await CreateWithNordjyllandSelectedAsync(new FakeSalesDistrictApiClient());

        // Act
        await viewModel.RemoveSecondaryCommand.ExecuteAsync(Bo);

        // Assert
        Assert.Equal(Anna, viewModel.SelectedDistrictDetails?.PrimarySalesperson);
        Assert.Equal([Mathias], viewModel.SelectedDistrictDetails!.SecondarySalespersons);
    }

    [Fact]
    public async Task UpdateRejectedByApi_ShowsApiMessage_AndKeepsCurrentDetails()
    {
        // Arrange
        const string apiMessage = "Invalid salesperson assignment: Primary salesperson 1 cannot also be a secondary salesperson.";
        var viewModel = await CreateWithNordjyllandSelectedAsync(new FakeSalesDistrictApiClient { RejectUpdatesWith = apiMessage });
        viewModel.SelectedSalesperson = Anna;

        // Act
        await viewModel.AddAsSecondaryCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal(apiMessage, viewModel.ErrorMessage);
        Assert.Equal([Bo, Mathias], viewModel.SelectedDistrictDetails!.SecondarySalespersons);
    }

    private static async Task<MainViewModel> CreateWithNordjyllandSelectedAsync(FakeSalesDistrictApiClient apiClient)
    {
        var viewModel = new MainViewModel(apiClient);
        viewModel.SelectedDistrict = Nordjylland;
        await viewModel.LoadDistrictDetailsCommand.ExecutionTask!;
        return viewModel;
    }
}