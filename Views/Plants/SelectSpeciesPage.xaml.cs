/*
 * Responsabilidade:
 * Code-Behind da tela modal para seleção de espécie no catálogo.
 * 
 * Papel na arquitetura:
 * Utiliza o padrão de Callback (Action<PlantSpecies>) para devolver o dado 
 * selecionado à tela chamadora (RegisterPlantPage), evitando o uso de rotas 
 * complexas ou passagem de mensagens globais.
 */

using KaaDebug.Core.Interfaces.Plants;
using KaaDebug.Core.Models.Plants;

namespace KaaDebug.Views.Plants;

public partial class SelectSpeciesPage : ContentPage
{
    private readonly IPlantsCatalogService _speciesService;
    public Action<PlantSpecies>? OnSpeciesPicked { get; set; }

    private List<PlantSpecies> _allSpecies = new();

    public SelectSpeciesPage(IPlantsCatalogService speciesService)
    {
        InitializeComponent();
        _speciesService = speciesService;
        NavigationPage.SetHasNavigationBar(this, false);
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