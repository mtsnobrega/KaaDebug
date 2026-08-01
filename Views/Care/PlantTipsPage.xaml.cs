using KaaDebug.Core.Interfaces.Plants;

namespace KaaDebug.Views.Care;

[QueryProperty(nameof(PlantId), "plantId")]
public partial class PlantTipsPage : ContentPage
{
    private readonly IPlantTipsService _careService;
    private string? _plantId;

    public string PlantId
    {
        get => _plantId ?? string.Empty;
        set => _plantId = value;
    }

    public PlantTipsPage(IPlantTipsService careService)
    {
        InitializeComponent();
        _careService = careService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadCareInfoAsync();
    }

    private async void OnRetryClicked(object? sender, EventArgs e) =>
        await LoadCareInfoAsync();

    private async Task LoadCareInfoAsync()
    {
        ShowState(loading: true, error: false, content: false);

        try
        {
            var result = await _careService.GetCareInfoAsync(_plantId!);

            if (!result.Success || result.CareInfo is null)
            {
                ShowState(loading: false, error: true, content: false);
                return;
            }

            var info = result.CareInfo;

            SpeciesNameLabel.Text = info.SpeciesName;
            SpeciesSummaryLabel.Text = info.Summary;

            if (!string.IsNullOrWhiteSpace(info.Curiosity))
            {
                CuriosityLabel.Text = info.Curiosity;
                CuriosityCard.IsVisible = true;
            }

            TipsCollectionView.ItemsSource = info.Tips;
            ShowState(loading: false, error: false, content: true);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao carregar dicas: {ex.Message}");
            ShowState(loading: false, error: true, content: false);
        }
    }

    private void ShowState(bool loading, bool error, bool content)
    {
        LoadingStateLayout.IsVisible = loading;
        ErrorStateLayout.IsVisible = error;
        ContentLayout.IsVisible = content;
    }

    private async void OnBackClicked(object? sender, EventArgs e) =>
        await Shell.Current.GoToAsync("..");
}