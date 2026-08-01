using KaaDebug.Core.Interfaces.Plants;
using KaaDebug.Core.Models.Plants;
using KaaDebug.Views.Controls;

namespace KaaDebug.Views.Plants;

[QueryProperty(nameof(PlantId), "plantId")]
public partial class PlantHistoryPage : ContentPage
{
    private readonly IPlantHistoryService _historyService;

    private string? _plantId;
    private HistoryPeriod _currentPeriod = HistoryPeriod.Last24Hours;
    private SensorType? _currentSensorFilter = null; // null = todos
    private PlantHistoryData? _currentData;

    public string PlantId
    {
        get => _plantId ?? string.Empty;
        set => _plantId = value;
    }

    public PlantHistoryPage(IPlantHistoryService historyService)
    {
        InitializeComponent();
        _historyService = historyService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadHistoryAsync(showFullLoading: true);
    }

    private async void OnRetryClicked(object? sender, EventArgs e) =>
        await LoadHistoryAsync(showFullLoading: true);

    private async void OnRefreshing(object? sender, EventArgs e)
    {
        await LoadHistoryAsync(showFullLoading: false);
        HistoryRefreshView.IsRefreshing = false;
    }

    private async Task LoadHistoryAsync(bool showFullLoading)
    {
        if (showFullLoading)
            ShowState(loading: true, error: false, charts: false);

        try
        {
            var result = await _historyService.GetHistoryAsync(_plantId!, _currentPeriod);

            if (!result.Success || result.Data is null)
            {
                ShowState(loading: false, error: true, charts: false);
                return;
            }

            _currentData = result.Data;
            PlantNameSubtitleLabel.Text = result.Data.PlantName;

            RenderCharts(_currentData);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao carregar histórico: {ex.Message}");
            ShowState(loading: false, error: true, charts: false);
        }
    }

    // ===================== RENDERIZAÇÃO =====================

    private void RenderCharts(PlantHistoryData data)
    {
        ChartsLayout.Children.Clear();

        var sensorsToShow = _currentSensorFilter.HasValue
            ? data.Sensors.Where(s => s.Type == _currentSensorFilter.Value).ToList()
            : data.Sensors;

        foreach (var sensor in sensorsToShow)
        {
            var card = new SensorHistoryCard();
            card.SetData(sensor, data.Period);
            ChartsLayout.Children.Add(card);
        }

        ShowState(loading: false, error: false, charts: true);
    }

    private void ShowState(bool loading, bool error, bool charts)
    {
        LoadingStateLayout.IsVisible = loading;
        ErrorStateLayout.IsVisible = error;
        ChartsLayout.IsVisible = charts;
    }

    // ===================== FILTRO DE PERÍODO =====================

    private void OnPeriod24hTapped(object? sender, EventArgs e) =>
        SetPeriod(HistoryPeriod.Last24Hours);

    private void OnPeriod7dTapped(object? sender, EventArgs e) =>
        SetPeriod(HistoryPeriod.Last7Days);

    private void OnPeriod30dTapped(object? sender, EventArgs e) =>
        SetPeriod(HistoryPeriod.Last30Days);

    private async void SetPeriod(HistoryPeriod period)
    {
        if (_currentPeriod == period) return;

        _currentPeriod = period;
        UpdatePeriodChipsVisualState();
        await LoadHistoryAsync(showFullLoading: true);
    }

    private void UpdatePeriodChipsVisualState()
    {
        var active = Color.FromArgb("#2E7D32");
        var inactiveBg = Colors.White;
        var inactiveStroke = Color.FromArgb("#D7E5D9");
        var inactiveText = Color.FromArgb("#5C715E");

        // Reseta os 3
        foreach (var (border, label) in new[]
        {
            (Period24hBorder, Period24hLabel),
            (Period7dBorder, Period7dLabel),
            (Period30dBorder, Period30dLabel)
        })
        {
            border.BackgroundColor = inactiveBg;
            border.Stroke = inactiveStroke;
            label.TextColor = inactiveText;
        }

        var (activeBorder, activeLabel) = _currentPeriod switch
        {
            HistoryPeriod.Last7Days => (Period7dBorder, Period7dLabel),
            HistoryPeriod.Last30Days => (Period30dBorder, Period30dLabel),
            _ => (Period24hBorder, Period24hLabel)
        };

        activeBorder.BackgroundColor = active;
        activeBorder.Stroke = active;
        activeLabel.TextColor = Colors.White;
    }

    // ===================== FILTRO DE SENSOR =====================

    private void OnFilterAllSensorsTapped(object? sender, EventArgs e) =>
        SetSensorFilter(null);

    private void OnFilterSoilTapped(object? sender, EventArgs e) =>
        SetSensorFilter(SensorType.SoilMoisture);

    private void OnFilterAirTapped(object? sender, EventArgs e) =>
        SetSensorFilter(SensorType.AirHumidity);

    private void OnFilterTempTapped(object? sender, EventArgs e) =>
        SetSensorFilter(SensorType.Temperature);

    private void OnFilterLuxTapped(object? sender, EventArgs e) =>
        SetSensorFilter(SensorType.Luminosity);

    private void SetSensorFilter(SensorType? sensorType)
    {
        if (_currentSensorFilter == sensorType) return;

        _currentSensorFilter = sensorType;
        UpdateSensorChipsVisualState();

        // Filtra nos dados já carregados (sem nova chamada à API)
        if (_currentData is not null)
            RenderCharts(_currentData);
    }

    private void UpdateSensorChipsVisualState()
    {
        var active = Color.FromArgb("#2E7D32");
        var inactiveBg = Colors.White;
        var inactiveStroke = Color.FromArgb("#D7E5D9");
        var inactiveText = Color.FromArgb("#5C715E");

        foreach (var (border, label) in new[]
        {
            (FilterAllSensorsBorder, FilterAllSensorsLabel),
            (FilterSoilBorder, FilterSoilLabel),
            (FilterAirBorder, FilterAirLabel),
            (FilterTempBorder, FilterTempLabel),
            (FilterLuxBorder, FilterLuxLabel)
        })
        {
            border.BackgroundColor = inactiveBg;
            border.Stroke = inactiveStroke;
            label.TextColor = inactiveText;
        }

        var (activeBorder, activeLabel) = _currentSensorFilter switch
        {
            SensorType.SoilMoisture => (FilterSoilBorder, FilterSoilLabel),
            SensorType.AirHumidity => (FilterAirBorder, FilterAirLabel),
            SensorType.Temperature => (FilterTempBorder, FilterTempLabel),
            SensorType.Luminosity => (FilterLuxBorder, FilterLuxLabel),
            _ => (FilterAllSensorsBorder, FilterAllSensorsLabel)
        };

        activeBorder.BackgroundColor = active;
        activeBorder.Stroke = active;
        activeLabel.TextColor = Colors.White;
    }

    // ===================== NAVEGAÇÃO =====================

    private async void OnBackClicked(object? sender, EventArgs e) =>
        await Shell.Current.GoToAsync("..");
}