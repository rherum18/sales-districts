using System.Collections.ObjectModel;
using System.Net.Http;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SalesDistrict.Wpf.Api;
using SalesDistrict.Wpf.Models;

namespace SalesDistrict.Wpf.ViewModels;

public sealed partial class MainViewModel : ObservableObject
{
    private readonly ISalesDistrictApiClient _apiClient;

    public MainViewModel(ISalesDistrictApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public ObservableCollection<District> Districts { get; } = [];

    public ObservableCollection<Salesperson> Salespersons { get; } = [];

    [ObservableProperty]
    public partial District? SelectedDistrict { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SetAsPrimaryCommand))]
    [NotifyCanExecuteChangedFor(nameof(AddAsSecondaryCommand))]
    public partial DistrictDetails? SelectedDistrictDetails { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SetAsPrimaryCommand))]
    [NotifyCanExecuteChangedFor(nameof(AddAsSecondaryCommand))]
    public partial Salesperson? SelectedSalesperson { get; set; }

    [ObservableProperty]
    public partial string? ErrorMessage { get; set; }

    [RelayCommand]
    private async Task LoadAsync()
    {
        ErrorMessage = null;
        try
        {
            var districts = await _apiClient.GetDistrictsAsync();
            var salespersons = await _apiClient.GetSalespersonsAsync();

            Districts.Clear();
            foreach (var district in districts)
            {
                Districts.Add(district);
            }

            Salespersons.Clear();
            foreach (var salesperson in salespersons)
            {
                Salespersons.Add(salesperson);
            }
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = $"Could not load districts and salespersons: {ex.Message}";
        }
    }

    partial void OnSelectedDistrictChanged(District? value) => LoadDistrictDetailsCommand.Execute(value);

    [RelayCommand(AllowConcurrentExecutions = true)]
    private async Task LoadDistrictDetailsAsync(District? district)
    {
        SelectedDistrictDetails = null;
        if (district is null)
        {
            return;
        }

        ErrorMessage = null;
        try
        {
            var details = await _apiClient.GetDistrictDetailsAsync(district.Id);

            // The user may have clicked another district while this one was loading
            if (district == SelectedDistrict)
            {
                SelectedDistrictDetails = details;
            }
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = $"Could not load district {district.Name}: {ex.Message}";
        }
    }

    private bool CanAssignSelectedSalesperson() => SelectedDistrictDetails is not null && SelectedSalesperson is not null;

    [RelayCommand(CanExecute = nameof(CanAssignSelectedSalesperson))]
    private Task SetAsPrimaryAsync()
    {
        var details = SelectedDistrictDetails!;
        var newPrimary = SelectedSalesperson!;

        // A promoted secondary stops being a secondary, and the previous primary leaves the district
        var secondaryIds = details.SecondarySalespersons
            .Where(secondary => secondary.Id != newPrimary.Id)
            .Select(secondary => secondary.Id)
            .ToList();

        return SaveAsync(details.Id, newPrimary.Id, secondaryIds);
    }

    [RelayCommand(CanExecute = nameof(CanAssignSelectedSalesperson))]
    private Task AddAsSecondaryAsync()
    {
        var details = SelectedDistrictDetails!;
        var secondaryIds = details.SecondarySalespersons
            .Select(secondary => secondary.Id)
            .Append(SelectedSalesperson!.Id)
            .ToList();

        return SaveAsync(details.Id, details.PrimarySalesperson.Id, secondaryIds);
    }

    [RelayCommand]
    private Task RemoveSecondaryAsync(Salesperson secondaryToRemove)
    {
        var details = SelectedDistrictDetails!;
        var secondaryIds = details.SecondarySalespersons
            .Where(secondary => secondary.Id != secondaryToRemove.Id)
            .Select(secondary => secondary.Id)
            .ToList();

        return SaveAsync(details.Id, details.PrimarySalesperson.Id, secondaryIds);
    }

    // The PUT replaces the district's whole assignment, so every change sends the complete new state.
    // The business rules are checked by the API, which explains a rejection in ErrorMessage.
    private async Task SaveAsync(int districtId, int primarySalespersonId, IReadOnlyList<int> secondarySalespersonIds)
    {
        ErrorMessage = null;
        try
        {
            var details = await _apiClient.UpdateDistrictSalespersonsAsync(districtId, primarySalespersonId, secondarySalespersonIds);

            // The user may have clicked another district while the change was being saved
            if (districtId == SelectedDistrict?.Id)
            {
                SelectedDistrictDetails = details;
            }
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}