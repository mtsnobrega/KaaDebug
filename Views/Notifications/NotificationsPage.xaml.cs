using KaaDebug.Core.Interfaces.Notifications;
using KaaDebug.Core.Models.Dashboard;

namespace KaaDebug.Views.Notifications;

public partial class NotificationsPage : ContentPage
{
    private readonly INotificationsService _notificationsService;
    private List<NotificationSummary> _allNotifications = new();

    public NotificationsPage(INotificationsService notificationsService)
    {
        InitializeComponent();
        _notificationsService = notificationsService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadNotificationsAsync(showFullLoading: true);
    }

    private async void OnRetryClicked(object? sender, EventArgs e) =>
        await LoadNotificationsAsync(showFullLoading: true);

    private async void OnRefreshing(object? sender, EventArgs e)
    {
        await LoadNotificationsAsync(showFullLoading: false);
        NotificationsRefreshView.IsRefreshing = false;
    }

    private async Task LoadNotificationsAsync(bool showFullLoading)
    {
        if (showFullLoading)
            ShowState(loading: true, error: false, empty: false, list: false);

        try
        {
            var result = await _notificationsService.GetAllNotificationsAsync();

            if (!result.Success || result.Notifications is null)
            {
                ShowState(loading: false, error: true, empty: false, list: false);
                return;
            }

            _allNotifications = result.Notifications;

            if (_allNotifications.Count == 0)
            {
                UpdateHeaderControls(unreadCount: 0);
                ShowState(loading: false, error: false, empty: true, list: false);
                return;
            }

            RenderNotifications(_allNotifications);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao carregar notificações: {ex.Message}");
            ShowState(loading: false, error: true, empty: false, list: false);
        }
    }

    /// <summary>
    /// Agrupa as notificações por período (Hoje / Esta semana / Anteriores)
    /// para facilitar a leitura — padrão adotado por apps como Gmail e iMessage.
    /// </summary>
    private void RenderNotifications(List<NotificationSummary> notifications)
    {
        // Ordena: não lidas primeiro, depois por data decrescente
        var sorted = notifications
            .OrderBy(n => n.IsRead)
            .ThenByDescending(n => n.CreatedAt)
            .ToList();

        var groups = sorted
            .GroupBy(n => GetGroupKey(n.CreatedAt))
            .Select(g => new NotificationGroup(g.Key, g.ToList()))
            .ToList();

        NotificationsCollectionView.ItemsSource = groups;
        NotificationsCollectionView.IsGrouped = true;

        var unreadCount = notifications.Count(n => !n.IsRead);
        UpdateHeaderControls(unreadCount);
        ShowState(loading: false, error: false, empty: false, list: true);
    }

    private static string GetGroupKey(DateTime dt)
    {
        var today = DateTime.Today;
        var diff = (today - dt.Date).TotalDays;

        if (diff < 1) return "Hoje";
        if (diff < 7) return "Esta semana";
        return "Anteriores";
    }

    private void UpdateHeaderControls(int unreadCount)
    {
        if (unreadCount > 0)
        {
            UnreadBadgeLabel.Text = $"{unreadCount} não lida{(unreadCount > 1 ? "s" : "")}";
            UnreadBadgeLabel.IsVisible = true;
        }
        else
        {
            UnreadBadgeLabel.IsVisible = false;
        }

        ClearAllLabel.IsVisible = _allNotifications.Count > 0;
    }

    private void ShowState(bool loading, bool error, bool empty, bool list)
    {
        LoadingStateLayout.IsVisible = loading;
        ErrorStateLayout.IsVisible = error;
        EmptyStateLayout.IsVisible = empty;
        NotificationsCollectionView.IsVisible = list;
    }

    // ===================== AÇÕES =====================

    private async void OnNotificationSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not NotificationSummary notification)
            return;

        NotificationsCollectionView.SelectedItem = null;

        // Marca como lida ao tocar (fire-and-forget: não bloqueia a navegação)
        if (!notification.IsRead)
        {
            _ = _notificationsService.MarkAsReadAsync(notification.Id);
            notification.IsRead = true;
        }

        await Shell.Current.GoToAsync($"PlantDetails?plantId={notification.PlantId}");
    }

    private async void OnClearAllTapped(object? sender, EventArgs e)
    {
        bool confirmed = await DisplayAlert(
            "Limpar notificações",
            "Deseja remover todas as notificações? Esta ação não pode ser desfeita.",
            "Sim, limpar",
            "Cancelar");

        if (!confirmed) return;

        try
        {
            var result = await _notificationsService.ClearAllNotificationsAsync();

            if (!result.Success)
            {
                await DisplayAlert("Erro", "Não foi possível limpar as notificações.", "OK");
                return;
            }

            _allNotifications.Clear();
            UpdateHeaderControls(unreadCount: 0);
            ShowState(loading: false, error: false, empty: true, list: false);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao limpar notificações: {ex.Message}");
        }
    }

    private async void OnBackClicked(object? sender, EventArgs e) =>
        await Shell.Current.GoToAsync("//Dashboard");
}

/// <summary>
/// Grupo de notificações para exibição agrupada no CollectionView.
/// Herda de List para ser diretamente bindável ao CollectionView.IsGrouped.
/// </summary>
public class NotificationGroup : List<NotificationSummary>
{
    public string Key { get; }

    public NotificationGroup(string key, List<NotificationSummary> items) : base(items)
    {
        Key = key;
    }
}