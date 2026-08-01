using KaaDebug.Core.Interfaces.Plants;
using KaaDebug.Core.Models.Plants;

namespace KaaDebug.Views.Plants;

public partial class SelectSpeciesPage : ContentPage
{
    private readonly IPlantsCatalogService _speciesService;

    /// <summary>
    /// Callback executado quando uma espécie é selecionada. Definido pela
    /// página que abriu este modal (RegisterPlantPage), para receber o
    /// resultado sem precisar de um sistema de mensageria mais complexo.
    /// </summary>
    public Action<PlantSpecies>? OnSpeciesPicked { get; set; }

    private List<PlantSpecies> _allSpecies = new();

    public SelectSpeciesPage(IPlantsCatalogService speciesService)
    {
        InitializeComponent();
        _speciesService = speciesService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadSpeciesAsync();
    }

    private async void OnRetryClicked(object? sender, EventArgs e)
    {
        await LoadSpeciesAsync();
    }

    private async Task LoadSpeciesAsync()
    {
        ShowState(loading: true, error: false, noResults: false, list: false);

        try
        {
            var result = await _speciesService.GetAllSpeciesAsync();

            if (!result.Success || result.Species is null)
            {
                ShowState(loading: false, error: true, noResults: false, list: false);
                return;
            }

            _allSpecies = result.Species;
            RenderList(_allSpecies);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao carregar espécies: {ex.Message}");
            ShowState(loading: false, error: true, noResults: false, list: false);
        }
    }

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_allSpecies.Count == 0)
            return;

        var searchText = e.NewTextValue?.Trim() ?? string.Empty;

        var filtered = string.IsNullOrWhiteSpace(searchText)
            ? _allSpecies
            : _allSpecies.Where(s => s.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();

        RenderList(filtered);
    }

    private void RenderList(List<PlantSpecies> species)
    {
        if (species.Count == 0)
        {
            ShowState(loading: false, error: false, noResults: true, list: false);
            return;
        }

        SpeciesCollectionView.ItemsSource = species;
        ShowState(loading: false, error: false, noResults: false, list: true);
    }

    private void ShowState(bool loading, bool error, bool noResults, bool list)
    {
        LoadingStateLayout.IsVisible = loading;
        ErrorStateLayout.IsVisible = error;
        NoResultsLabel.IsVisible = noResults;
        SpeciesCollectionView.IsVisible = list;
    }

    private async void OnSpeciesSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not PlantSpecies species)
            return;

        OnSpeciesPicked?.Invoke(species);
        await Shell.Current.GoToAsync("..");
    }

    private async void OnCancelTapped(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}