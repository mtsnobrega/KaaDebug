/*
 * Responsabilidade:
 * Code-Behind da página de registro/associação de dispositivos IoT.
 * 
 * Papel na arquitetura:
 * Orquestrador de Processo (Apresentação). Implementa um fluxo de dois passos:
 * primeiro consome a Verificação (IDeviceVerificationService); e, 
 * dependendo das regras de negócio (ex: exigir que o dispositivo esteja Online), 
 * prossegue para a Associação (IDeviceAssociationService). 
 * Também gerencia as ricas transições de estado visual (Cores, Ícones e Cards).
 */
using KaaDebug.Core.Interfaces.Devices;

namespace KaaDebug.Views.Devices;

[QueryProperty(nameof(PlantId), "plantId")]
public partial class RegisterDevicePage : ContentPage
{
    private readonly IDeviceAssociationService _associationService;
    private readonly IDeviceVerificationService _verificationService;

    private string? _plantId;
    private bool _associationDone;

    public string PlantId
    {
        get => _plantId ?? string.Empty;
        set => _plantId = value;
    }
    public RegisterDevicePage(
        IDeviceAssociationService associationService,
        IDeviceVerificationService verificationService)
    {
        InitializeComponent();
        _associationService = associationService;
        _verificationService = verificationService;
    }

    // ===================== CAMPO DE CÓDIGO =====================
    private void OnDeviceCodeChanged(object? sender, TextChangedEventArgs e)
    {
        var raw = e.NewTextValue ?? string.Empty;
        var filtered = new string(raw.ToUpperInvariant().Where(c => char.IsLetterOrDigit(c)).ToArray());

        if (filtered != e.NewTextValue)
        {
            DeviceCodeSuffixEntry.Text = filtered;
            return;
        }

        if (CodeErrorLabel.IsVisible) CodeErrorLabel.IsVisible = false;
        if (GeneralErrorBorder.IsVisible) GeneralErrorBorder.IsVisible = false;

        AssociateButton.IsEnabled = filtered.Length == 4 && !_associationDone;
    }

    private string GetFullCode() =>
        $"ESP32-{DeviceCodeSuffixEntry.Text?.Trim().ToUpperInvariant()}";

    // ===================== ASSOCIAR =====================
    private async void OnAssociateClicked(object? sender, EventArgs e)
    {
        var suffix = DeviceCodeSuffixEntry.Text?.Trim() ?? string.Empty;

        if (suffix.Length != 4)
        {
            CodeErrorLabel.Text = "O código deve ter 4 caracteres.";
            CodeErrorLabel.IsVisible = true;
            return;
        }

        await PerformAssociationAsync(GetFullCode());
    }

    private async Task PerformAssociationAsync(string fullCode)
    {

        try
        {
            // PASSO 1: Verifica se o dispositivo existe e está disponível
            var verifyResult = await _verificationService.VerifyDeviceAsync(fullCode);

            if (!verifyResult.Success)
            {
                ShowError(
                    verifyResult.ErrorMessage ??
                    "Não foi possível verificar o dispositivo.");

                return;
            }

            // Dispositivo não encontrado
            if (verifyResult.Status == DeviceVerificationStatus.NotFound)
            {
                ShowStatusCard(
                    checking: false,
                    status: DeviceVerificationStatus.NotFound);

                return;
            }

            // Dispositivo encontrado, mas offline
            if (verifyResult.Status == DeviceVerificationStatus.Offline)
            {
                ShowStatusCard(
                    checking: false,
                    status: DeviceVerificationStatus.Offline);

                ShowError(
                    "O dispositivo está offline. Ligue o dispositivo e tente novamente.");

                return;
            }

            // PASSO 2: Só associa se estiver ONLINE
            var associationResult =
                await _associationService.AssociateAsync(_plantId!, fullCode);

            if (!associationResult.Success)
            {
                ShowError(
                    associationResult.ErrorMessage ??
                    "Não foi possível associar o dispositivo.");

                return;
            }
            SetLoadingState(false);

            // PASSO 3: Associação concluída
            ShowStatusCard(
                checking: false,
                status: DeviceVerificationStatus.Online);

            _associationDone = true;

            AssociateButton.IsVisible = false;
            DoneButton.IsVisible = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Erro ao associar dispositivo: {ex}");

            ShowError(
                "Ocorreu um erro inesperado. Tente novamente.");
        }
        finally
        {
            SetLoadingState(false);
        }

        SetLoadingState(true);
    }

    // ===================== ESTADOS DO CARD DE STATUS =====================
    private void ShowStatusCard(bool checking, DeviceVerificationStatus? status = null)
    {
        StatusCard.IsVisible = true;

        if (checking)
        {
            ApplyStatusCardTheme("#F4FBF4", "#D7E5D9");
            StatusCheckingIndicator.IsRunning = true;
            StatusCheckingIndicator.IsVisible = true;
            StatusIconLabel.IsVisible = false;
            StatusTitleLabel.Text = "Verificando conexão...";
            StatusSubtitleLabel.Text = "Aguardando resposta do dispositivo";
            return;
        }

        StatusCheckingIndicator.IsRunning = false;
        StatusCheckingIndicator.IsVisible = false;
        StatusIconLabel.IsVisible = true;

        switch (status)
        {
            case DeviceVerificationStatus.Online:
                ApplyStatusCardTheme("#EAF5EA", "#A5D6A7");
                StatusIconLabel.Text = "✅";
                StatusTitleLabel.Text = "Dispositivo online";
                StatusSubtitleLabel.Text = "O ESP32 está comunicando normalmente. Monitoramento ativo.";
                break;

            case DeviceVerificationStatus.Offline:
                ApplyStatusCardTheme("#FFF8E1", "#FFE082");
                StatusIconLabel.Text = "⚠️";
                StatusTitleLabel.Text = "Dispositivo offline";
                StatusSubtitleLabel.Text = "Associação salva, mas o dispositivo não está respondendo. Verifique se está ligado e conectado ao Wi-Fi.";
                break;

            // NOVO CASE: Impede que o 'Unassociated' caia no 'default' de erro!
            case DeviceVerificationStatus.Unassociated:
                ApplyStatusCardTheme("#E3F2FD", "#90CAF9");
                StatusIconLabel.Text = "🔗";
                StatusTitleLabel.Text = "Dispositivo livre";
                StatusSubtitleLabel.Text = "O ESP32 está conectado à rede e pronto para ser associado.";
                break;

            case DeviceVerificationStatus.Associated:
                ApplyStatusCardTheme("#E3F2FD", "#90CAF9");
                StatusIconLabel.Text = "🔗";
                StatusTitleLabel.Text = "Dispositivo livre";
                StatusSubtitleLabel.Text = "O ESP32 não pode ser utilizado em mais de uma planta";
                break;

            default: // Qualquer outra coisa, ou NotFound
                ApplyStatusCardTheme("#FDECEA", "#F5C2C0");
                StatusIconLabel.Text = "❌";
                StatusTitleLabel.Text = "Dispositivo não encontrado";
                StatusSubtitleLabel.Text = "Verifique o código na etiqueta e tente novamente.";
                break;
        }

    }

    private void ApplyStatusCardTheme(string bgColor, string strokeColor)
    {
        StatusCard.BackgroundColor = Color.FromArgb(bgColor);
        StatusCard.Stroke = new SolidColorBrush(Color.FromArgb(strokeColor));
    }

    // ===================== UTILITÁRIOS =====================
    private void SetLoadingState(bool isLoading)
    {
        AssociateButton.IsEnabled = !isLoading && !_associationDone;
        AssociateButton.Text = isLoading ? string.Empty : "Associar dispositivo";
        AssociateLoadingIndicator.IsVisible = isLoading;
        AssociateLoadingIndicator.IsRunning = isLoading;
        DeviceCodeSuffixEntry.IsEnabled = !isLoading;
    }

    private void ShowError(string message)
    {
        GeneralErrorLabel.Text = message;
        GeneralErrorBorder.IsVisible = true;
    }

    // ===================== NAVEGAÇÃO =====================
    private async void OnDoneClicked(object? sender, EventArgs e) =>
        await Shell.Current.GoToAsync("..");

    private async void OnBackClicked(object? sender, EventArgs e) =>
        await Shell.Current.GoToAsync("..");
}