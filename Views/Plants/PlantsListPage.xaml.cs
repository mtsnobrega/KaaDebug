using KaaDebug.Core.Interfaces.Plants;
using KaaDebug.Core.Models.Dashboard;

namespace KaaDebug.Views.Plants;

public partial class PlantsListPage : ContentPage
{
    private enum StatusFilter
    {
        All,
        Healthy,
        Attention,
        Critical
    }

    private readonly IPlantsListService _plantsService;

    private List<PlantSummary> _allPlants = new();
    private StatusFilter _currentFilter = StatusFilter.All;
    private string _currentSearchText = string.Empty;

    public PlantsListPage(IPlantsListService plantsService)
    {
        InitializeComponent();
        _plantsService = plantsService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadPlantsAsync(showFullLoading: true);
    }

    private async void OnRetryClicked(object? sender, EventArgs e)
    {
        await LoadPlantsAsync(showFullLoading: true);
    }

    private async void OnRefreshing(object? sender, EventArgs e)
    {
        await LoadPlantsAsync(showFullLoading: false);
        PlantsRefreshView.IsRefreshing = false;
    }

    private async Task LoadPlantsAsync(bool showFullLoading)
    {
        if (showFullLoading)
            ShowState(loading: true, error: false, empty: false, noResults: false, list: false);

        try
        {
            var result = await _plantsService.GetAllPlantsAsync();

            if (!result.Success || result.Plants is null)
            {
                ErrorMessageLabel.Text = result.ErrorMessage ?? "Verifique sua conexão e tente novamente.";
                ShowState(loading: false, error: true, empty: false, noResults: false, list: false);
                return;
            }

            _allPlants = result.Plants;

            if (_allPlants.Count == 0)
            {
                ShowState(loading: false, error: false, empty: true, noResults: false, list: false);
                return;
            }

            ApplyFiltersAndRender();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao carregar plantas: {ex.Message}");
            ErrorMessageLabel.Text = "Ocorreu um erro inesperado. Tente novamente.";
            ShowState(loading: false, error: true, empty: false, noResults: false, list: false);
        }
    }

    // ===================== BUSCA E FILTROS =====================

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        _currentSearchText = e.NewTextValue?.Trim() ?? string.Empty;

        // Só re-renderiza se já carregamos as plantas (evita erro durante o loading inicial)
        if (_allPlants.Count > 0)
            ApplyFiltersAndRender();
    }

    private void OnFilterAllTapped(object? sender, EventArgs e) => SetFilter(StatusFilter.All);
    private void OnFilterHealthyTapped(object? sender, EventArgs e) => SetFilter(StatusFilter.Healthy);
    private void OnFilterAttentionTapped(object? sender, EventArgs e) => SetFilter(StatusFilter.Attention);
    private void OnFilterCriticalTapped(object? sender, EventArgs e) => SetFilter(StatusFilter.Critical);

    private void SetFilter(StatusFilter filter)
    {
        _currentFilter = filter;
        UpdateFilterChipsVisualState();
        ApplyFiltersAndRender();
    }

    /// <summary>
    /// Aplica busca por nome + filtro de status sobre a lista completa
    /// (em memória, pois o volume de plantas de um usuário doméstico é
    /// pequeno - não justifica busca paginada no servidor).
    /// </summary>
    private void ApplyFiltersAndRender()
    {
        var filtered = _allPlants.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(_currentSearchText))
        {
            filtered = filtered.Where(p =>
                p.Name.Contains(_currentSearchText, StringComparison.OrdinalIgnoreCase));
        }

        filtered = _currentFilter switch
        {
            StatusFilter.Healthy => filtered.Where(p => p.HealthStatus == PlantHealthStatus.Healthy),
            StatusFilter.Attention => filtered.Where(p => p.HealthStatus == PlantHealthStatus.Attention),
            StatusFilter.Critical => filtered.Where(p => p.HealthStatus == PlantHealthStatus.Critical),
            _ => filtered
        };

        var result = filtered.ToList();

        if (result.Count == 0)
        {
            ShowState(loading: false, error: false, empty: false, noResults: true, list: false);
            return;
        }

        PlantsCollectionView.ItemsSource = result;
        ShowState(loading: false, error: false, empty: false, noResults: false, list: true);
    }

    private void UpdateFilterChipsVisualState()
    {
        var activeBg = Color.FromArgb("#2E7D32");
        var activeText = Colors.White;
        var inactiveBg = Colors.White;
        var inactiveStroke = Color.FromArgb("#D7E5D9");
        var inactiveText = Color.FromArgb("#5C715E");

        // Reseta todos
        foreach (var (border, label) in new[]
                 {
                     (FilterAllBorder, FilterAllLabel),
                     (FilterHealthyBorder, FilterHealthyLabel),
                     (FilterAttentionBorder, FilterAttentionLabel),
                     (FilterCriticalBorder, FilterCriticalLabel)
                 })
        {
            border.BackgroundColor = inactiveBg;
            border.Stroke = inactiveStroke;
            label.TextColor = inactiveText;
        }

        // Aplica destaque no filtro ativo
        var (activeBorder, activeLabel) = _currentFilter switch
        {
            StatusFilter.Healthy => (FilterHealthyBorder, FilterHealthyLabel),
            StatusFilter.Attention => (FilterAttentionBorder, FilterAttentionLabel),
            StatusFilter.Critical => (FilterCriticalBorder, FilterCriticalLabel),
            _ => (FilterAllBorder, FilterAllLabel)
        };

        activeBorder.BackgroundColor = activeBg;
        activeBorder.Stroke = activeBg;
        activeLabel.TextColor = activeText;
    }

    // ===================== ESTADOS DA TELA =====================

    private void ShowState(bool loading, bool error, bool empty, bool noResults, bool list)
    {
        LoadingStateLayout.IsVisible = loading;
        ErrorStateLayout.IsVisible = error;
        EmptyStateLayout.IsVisible = empty;
        NoResultsStateLayout.IsVisible = noResults;
        PlantsCollectionView.IsVisible = list;
    }

    // ===================== NAVEGAÇÃO =====================

    private async void OnPlantSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not PlantSummary plant)
            return;

        PlantsCollectionView.SelectedItem = null;
        await Shell.Current.GoToAsync($"PlantsDetails?plantId={plant.Id}");
    }

    private async void OnAddPlantClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//RegisterPlant");
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//Dashboard");
    }
}