using KaaDebug.Core.Interfaces.Plants;
using KaaDebug.Core.Models.Dashboard;
using KaaDebug.Core.Models.Plants;
using KaaDebug.Views.Controls;

namespace KaaDebug.Views.Plants;

/// <summary>
/// Implementa IQueryAttributable para receber o "plantId" enviado via
/// Shell.Current.GoToAsync("PlantDetails?plantId=..."), conforme o padrão
/// de navegação com parâmetros do .NET MAUI Shell.
/// </summary>
[QueryProperty(nameof(PlantId), "plantId")]
public partial class PlantsDetailsPage : ContentPage
{
    private readonly IPlantDetailsService _detailsService;

    private string? _plantId;
    private PlantDetails? _currentDetails;

    /// <summary>
    /// Recebido automaticamente pelo Shell a partir do parâmetro de rota
    /// "plantId" (ver atributo QueryProperty acima).
    /// </summary>
    public string PlantId
    {
        get => _plantId ?? string.Empty;
        //set => _plantId = value;
        set
        {
            _plantId = value;
            System.Diagnostics.Debug.WriteLine($"PlantId recebido: {_plantId}");
        }
    }

    public PlantsDetailsPage(IPlantDetailsService detailsService)
    {
        InitializeComponent();
        _detailsService = detailsService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (string.IsNullOrWhiteSpace(_plantId))
        {
            // Segurança: tela acessada sem id válido. Não há o que exibir.
            ErrorMessageLabel.Text = "Planta não encontrada.";
            ShowState(loading: false, error: true, content: false);
            return;
        }

        // Recarrega sempre ao aparecer (ex: voltando de Editar/Diagnóstico,
        // os dados podem ter mudado). Diferente do Dashboard, aqui a
        // informação precisa estar sempre fresca por ser a tela operacional.
        await LoadDetailsAsync(showFullLoading: _currentDetails is null);
    }

    private async void OnRetryClicked(object? sender, EventArgs e)
    {
        await LoadDetailsAsync(showFullLoading: true);
    }

    private async void OnRefreshing(object? sender, EventArgs e)
    {
        await LoadDetailsAsync(showFullLoading: false);
        DetailsRefreshView.IsRefreshing = false;
    }

    private async Task LoadDetailsAsync(bool showFullLoading)
    {
        if (showFullLoading)
            ShowState(loading: true, error: false, content: false);

        try
        {
            var result = await _detailsService.GetPlantDetailsAsync(_plantId!);

            if (!result.Success || result.Details is null)
            {
                ErrorMessageLabel.Text = result.ErrorMessage ?? "Verifique sua conexão e tente novamente.";
                ShowState(loading: false, error: true, content: false);
                return;
            }

            _currentDetails = result.Details;
            PopulateContent(_currentDetails);
            ShowState(loading: false, error: false, content: true);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao carregar detalhes da planta: {ex.Message}");
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

    private void PopulateContent(PlantDetails details)
    {
        PlantPhotoImage.Source = details.PhotoUrl;
        PlantNameLabel.Text = details.Name;
        PlantSpeciesLabel.Text = details.Species;

        PopulateHealthStatus(details.HealthStatus, details.StatusReason);
        PopulateDeviceStatus(details.Device);
        PopulateIndicators(details.Indicators);
        PopulateNotifications(details.RelatedNotifications);
    }

    private void PopulateHealthStatus(PlantHealthStatus status, string? reason)
    {
        var (color, text) = status switch
        {
            PlantHealthStatus.Healthy => (Color.FromArgb("#2E7D32"), "Saudável"),
            PlantHealthStatus.Attention => (Color.FromArgb("#F0A93B"), "Atenção"),
            PlantHealthStatus.Critical => (Color.FromArgb("#C62828"), "Crítico"),
            _ => (Color.FromArgb("#8A9A8C"), "—")
        };

        HealthStatusBadge.BackgroundColor = color;
        HealthStatusLabel.Text = text;

        StatusReasonLabel.Text = reason;
        StatusReasonLabel.IsVisible = !string.IsNullOrWhiteSpace(reason);
    }



















    private void PopulateDeviceStatus(DeviceInformation device)
    {
        // Oculta ambos os botões por padrão
        AssociateDeviceButton.IsVisible = false;
        RetryDeviceButton.IsVisible = false;

        switch (device.ConnectionStatus)
        {
            case DeviceConnectionStatus.Online:
                DeviceStatusTitleLabel.Text = "Dispositivo conectado";
                DeviceStatusDot.Color = Color.FromArgb("#2E7D32");
                DeviceStatusSubtitleLabel.Text = device.LastReadingAt is { } lastReading
                    ? $"Última leitura há {FormatElapsed(lastReading)}"
                    : "Aguardando primeira leitura";
                break;

            case DeviceConnectionStatus.Offline:
                DeviceStatusTitleLabel.Text = "Dispositivo offline";
                DeviceStatusDot.Color = Color.FromArgb("#C62828");
                DeviceStatusSubtitleLabel.Text = device.LastReadingAt is { } lastSeen
                    ? $"Última comunicação há {FormatElapsed(lastSeen)}"
                    : "Sem comunicação registrada";
                RetryDeviceButton.IsVisible = true; // ← mostra botão de verificar
                break;

            default: // NotAssociated
                DeviceStatusTitleLabel.Text = "Nenhum dispositivo associado";
                DeviceStatusDot.Color = Color.FromArgb("#8A9A8C");
                DeviceStatusSubtitleLabel.Text = "Associe um ESP32 para monitorar automaticamente";
                AssociateDeviceButton.IsVisible = true; // ← mostra botão de associar
                break;
        }
    }








    private async void OnAssociateDeviceClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"RegisterDevice?plantId={_plantId}");
    }

    private async void OnRetryDeviceClicked(object? sender, EventArgs e)
    {
        // Recarrega a tela para verificar se o dispositivo voltou a responder
        await LoadDetailsAsync(showFullLoading: false);
    }

    private static string FormatElapsed(DateTime timestamp)
    {
        var elapsed = DateTime.Now - timestamp;

        if (elapsed.TotalMinutes < 60)
            return $"{(int)elapsed.TotalMinutes} min";

        if (elapsed.TotalHours < 24)
            return $"{(int)elapsed.TotalHours}h";

        return $"{(int)elapsed.TotalDays} dia(s)";
    }

    /// <summary>
    /// Popula o carrossel horizontal recriando os IndicatorCard a cada
    /// carga. Como são poucos itens fixos (4 sensores), recriar é mais
    /// simples do que reutilizar instâncias e não tem custo perceptível.
    /// </summary>
    private void PopulateIndicators(List<SensorIndicator> indicators)
    {
        IndicatorsStackLayout.Children.Clear();

        foreach (var indicator in indicators)
        {
            var card = new IndicatorsCardPage();
            card.SetIndicator(indicator);
            IndicatorsStackLayout.Children.Add(card);
        }
    }

    private void PopulateNotifications(List<NotificationSummary> notifications)
    {
        NotificationsCollectionView.ItemsSource = notifications;
        NotificationsCollectionView.IsVisible = notifications.Count > 0;
        EmptyNotificationsLabel.IsVisible = notifications.Count == 0;
    }

    // ===================== AÇÕES =====================

    private async void OnDiagnosisTapped(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"AiDiagnosis?plantId={_plantId}");
    }

    private async void OnTipsTapped(object? sender, EventArgs e)
    {
         await Shell.Current.GoToAsync($"PlantTips?plantId={_plantId}");
    }

    private async void OnEditTapped(object? sender, EventArgs e) => await NavigateToEditAsync();
    private async void OnEditClicked(object? sender, EventArgs e) => await NavigateToEditAsync();

    private async Task NavigateToEditAsync()
    {
        await Shell.Current.GoToAsync($"EditPlant?plantId={_plantId}");
    }

    private async void OnHistoryTapped(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"PlantHistory?plantId={_plantId}");
    }

    private async void OnNotificationSelected(object? sender, SelectionChangedEventArgs e)
    {
        // Notificação já é desta planta; apenas desseleciona visualmente.
        // (Sem navegação adicional, pois já estamos na tela da planta correspondente.)
        NotificationsCollectionView.SelectedItem = null;
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}