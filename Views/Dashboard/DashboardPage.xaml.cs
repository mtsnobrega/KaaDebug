using KaaDebug.Core.Interfaces.Plants;
using KaaDebug.Core.Models.Dashboard;

namespace KaaDebug.Views.Dashboard;

public partial class DashboardPage : ContentPage
{
    private const int MaxRecentPlants = 3;

    private readonly IDashboardService _dashboardService;
    private DashboardData? _currentData;

    public DashboardPage(IDashboardService dashboardService)
    {
        InitializeComponent();
        _dashboardService = dashboardService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Só recarrega do zero se ainda não há dados (primeira navegação).
        // Em retornos subsequentes à tela (ex: voltar de Detalhes), evitamos
        // um loading completo - o pull-to-refresh cobre a atualização manual.
        if (_currentData is null)
        {
            await LoadDashboardAsync(showFullLoading: true);
        }
    }

    private async void OnRetryClicked(object? sender, EventArgs e)
    {
        await LoadDashboardAsync(showFullLoading: true);
    }

    private async void OnRefreshing(object? sender, EventArgs e)
    {
        await LoadDashboardAsync(showFullLoading: false);
        DashboardRefreshView.IsRefreshing = false;
    }

    private async Task LoadDashboardAsync(bool showFullLoading)
    {
        if (showFullLoading)
            ShowState(loading: true, error: false, content: false);

        try
        {
            var result = await _dashboardService.GetDashboardAsync();

            if (!result.Success || result.Data is null)
            {
                ErrorMessageLabel.Text = result.ErrorMessage ?? "Verifique sua conexão e tente novamente.";
                ShowState(loading: false, error: true, content: false);
                return;
            }

            _currentData = result.Data;
            PopulateContent(_currentData);
            ShowState(loading: false, error: false, content: true);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao carregar dashboard: {ex.Message}");
            ErrorMessageLabel.Text = "Ocorreu um erro inesperado. Tente novamente.";
            ShowState(loading: false, error: true, content: false);
        }
    }

    private void ShowState(bool loading, bool error, bool content)
    {
        LoadingStateLayout.IsVisible = loading;
        ErrorStateLayout.IsVisible = error;
        ContentLayout.IsVisible = content;
    }

    private void PopulateContent(DashboardData data)
    {
        GreetingLabel.Text = $"Olá, {data.UserFirstName} ??";
        TotalPlantsLabel.Text = data.TotalPlants.ToString();
        ActiveAlertsLabel.Text = data.ActiveAlertsCount.ToString();

        // Plantas: mostra só as 3 mais recentes/relevantes; restante via "Ver todas"
        var plantsToShow = data.RecentPlants.Take(MaxRecentPlants).ToList();
        PlantsCollectionView.ItemsSource = plantsToShow;
        PlantsCollectionView.IsVisible = plantsToShow.Count > 0;
        EmptyPlantsLayout.IsVisible = plantsToShow.Count == 0;

        // Notificações recentes
        NotificationsCollectionView.ItemsSource = data.RecentNotifications;
        NotificationsCollectionView.IsVisible = data.RecentNotifications.Count > 0;
        EmptyNotificationsLabel.IsVisible = data.RecentNotifications.Count == 0;
    }

    // ===================== NAVEGAÇÃO: PLANTAS =====================

    private async void OnPlantSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not PlantSummary plant)
            return;

        // Desseleciona para permitir tocar no mesmo item novamente depois
        PlantsCollectionView.SelectedItem = null;

        await Shell.Current.GoToAsync($"PlantsDetails?plantId={plant.Id}");
    }

    private async void OnViewAllPlantsTapped(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//PlantsList");
    }

    private async void OnAddPlantClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("RegisterPlant");
    }

    // ===================== NAVEGAÇÃO: NOTIFICAÇÕES =====================

    private async void OnNotificationSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not NotificationSummary notification)
            return;

        NotificationsCollectionView.SelectedItem = null;

        await Shell.Current.GoToAsync($"PlantsDetails?plantId={notification.PlantId}");
    }

    private async void OnViewAllNotificationsTapped(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//Notifications");
    }

    // ===================== BOTTOM NAVIGATION =====================

    private void OnNavDashboardTapped(object? sender, EventArgs e)
    {
        // Já estamos no Dashboard; sem ação.
    }

    private async void OnNavPlantsTapped(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//PlantsList");
    }

    private async void OnNavNotificationsTapped(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//Notifications");
    }

    private async void OnNavProfileTapped(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//Profile");
    }
}