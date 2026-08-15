using KaaDebug.Core.Interfaces.Devices;

namespace KaaDebug.Views.Devices;

[QueryProperty(nameof(PlantId), "plantId")]
public partial class RegisterDevicePage : ContentPage
{
    // ALTERADO: _editService removido, _associationService adicionado
    private readonly IDeviceAssociationService _associationService;
    private readonly IDeviceVerificationService _verificationService;

    private string? _plantId;
    private bool _associationDone;

    public string PlantId
    {
        get => _plantId ?? string.Empty;
        set => _plantId = value;
    }

    // ALTERADO: construtor recebe IDeviceAssociationService
    public RegisterDevicePage(
        IDeviceAssociationService associationService,
        IDeviceVerificationService verificationService)
    {
        InitializeComponent();
        _associationService = associationService;   // ALTERADO
        _verificationService = verificationService; // MANTIDO
    }

    // ===================== CAMPO DE CÓDIGO =====================
    // MANTIDO inteiro
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
    // MANTIDO
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
        SetLoadingState(true);

        try
        {
            // PASSO 1: Tenta "tomar posse" (Associar) o dispositivo primeiro.
            var associationResult = await _associationService.AssociateAsync(_plantId!, fullCode);

            if (!associationResult.Success)
            {
                // Se a associação falhar porque o dispositivo JÁ TEM DONO, o backend 
                // deve retornar uma mensagem de erro específica.
                // Exemplo: verifique se a string contém algo que indique que já está em uso.
                bool alreadyInUse = associationResult.ErrorMessage?.Contains("já associado", StringComparison.OrdinalIgnoreCase) == true
                                 || associationResult.ErrorMessage?.Contains("already exists", StringComparison.OrdinalIgnoreCase) == true;

                if (alreadyInUse)
                {
                    ShowError("Este dispositivo já está associado a outra planta ou usuário.");
                }
                else
                {
                    ShowError(associationResult.ErrorMessage ?? "Não foi possível associar o dispositivo.");
                }

                return;
            }

            // PASSO 2: A associação funcionou! 
            // Se funcionou, significa que o status dele antes era "Não Associado".
            // Agora que ele é nosso, a API vai permitir ler se ele está "Online" ou "Offline".

            SetLoadingState(false);
            ShowStatusCard(checking: true);

            var verifyResult = await _verificationService.VerifyDeviceAsync(fullCode);

            // Se mesmo após associar com sucesso ele retornar NotFound, 
            // significa que ele nunca foi ligado na internet (não existe no broker IoT).
            if (!verifyResult.Success || verifyResult.Status == DeviceVerificationStatus.NotFound)
            {
                ShowStatusCard(checking: false, status: DeviceVerificationStatus.NotFound);
                return;
            }

            // Se chegou aqui, o dispositivo é seu e está Online (funcionando) ou Offline (não funcionando).
            ShowStatusCard(checking: false, status: verifyResult.Status);

            _associationDone = true;
            AssociateButton.IsVisible = false;
            DoneButton.IsVisible = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao associar dispositivo: {ex.Message}");
            ShowError("Ocorreu um erro inesperado. Tente novamente.");
        }
        finally
        {
            // Garante que o loading do botão pare caso o processo morra no meio
            if (AssociateLoadingIndicator.IsRunning)
            {
                SetLoadingState(false);
            }
        }








        /*
        SetLoadingState(true);

        try
        {
            // ALTERADO: usa _associationService.AssociateAsync em vez de _editService.UpdatePlantAsync
            // Chama PUT /plants/{id}/device — não exige Name
            var associationResult = await _associationService.AssociateAsync(_plantId!, fullCode);

            if (!associationResult.Success)
            {
                ShowError(associationResult.ErrorMessage ?? "Não foi possível associar o dispositivo.");
                return;
            }

            // MANTIDO: Passo 2 — verifica status de comunicação do ESP32
            SetLoadingState(false);
            ShowStatusCard(checking: true);

            var verifyResult = await _verificationService.VerifyDeviceAsync(fullCode);

            if (!verifyResult.Success)
            {
                ShowStatusCard(checking: false, status: DeviceVerificationStatus.NotFound);
                return;
            }

            ShowStatusCard(checking: false, status: verifyResult.Status);
            _associationDone = true;
            AssociateButton.IsVisible = false;
            DoneButton.IsVisible = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao associar dispositivo: {ex.Message}");
            ShowError("Ocorreu um erro inesperado. Tente novamente.");
        }
        finally
        {
            SetLoadingState(false);
        }
        */
    }

    // ===================== ESTADOS DO CARD DE STATUS =====================
    // MANTIDO inteiro
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

            default: // Qualquer outra coisa, ou NotFound
                ApplyStatusCardTheme("#FDECEA", "#F5C2C0");
                StatusIconLabel.Text = "❌";
                StatusTitleLabel.Text = "Dispositivo não encontrado";
                StatusSubtitleLabel.Text = "Verifique o código na etiqueta e tente novamente.";
                break;
        }









        /*
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

            default:
                ApplyStatusCardTheme("#FDECEA", "#F5C2C0");
                StatusIconLabel.Text = "❌";
                StatusTitleLabel.Text = "Dispositivo não encontrado";
                StatusSubtitleLabel.Text = "Verifique o código na etiqueta e tente novamente.";
                break;
        }
        */
    }

    private void ApplyStatusCardTheme(string bgColor, string strokeColor)
    {
        StatusCard.BackgroundColor = Color.FromArgb(bgColor);
        StatusCard.Stroke = new SolidColorBrush(Color.FromArgb(strokeColor));
    }

    // ===================== UTILITÁRIOS =====================
    // MANTIDO inteiro
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
    // MANTIDO inteiro
    private async void OnDoneClicked(object? sender, EventArgs e) =>
        await Shell.Current.GoToAsync("..");

    private async void OnBackClicked(object? sender, EventArgs e) =>
        await Shell.Current.GoToAsync("..");
}

/*
 * using KaaDebug.Core.Interfaces.Devices;
using KaaDebug.Core.Interfaces.Plants;

namespace KaaDebug.Views.Devices;

[QueryProperty(nameof(PlantId), "plantId")]
public partial class RegisterDevicePage : ContentPage
{
    private readonly IPlantsEditService _editService;
    private readonly IDeviceVerificationService _verificationService;

    private string? _plantId;
    private bool _associationDone;

    public string PlantId
    {
        get => _plantId ?? string.Empty;
        set => _plantId = value;
    }

    public RegisterDevicePage(
        //IPlantsEditService editService,
        //IDeviceVerificationService verificationService)
        IDeviceAssociationService associationService,
        IDeviceVerificationService verificationService)
    {
        InitializeComponent();
        _editService = editService;
        _verificationService = verificationService;
    }

    // ===================== CAMPO DE CÓDIGO =====================

    private void OnDeviceCodeChanged(object? sender, TextChangedEventArgs e)
    {
        // Aceita apenas letras maiúsculas e dígitos
        var raw = e.NewTextValue ?? string.Empty;
        var filtered = new string(raw.ToUpperInvariant().Where(c => char.IsLetterOrDigit(c)).ToArray());

        if (filtered != e.NewTextValue)
        {
            DeviceCodeSuffixEntry.Text = filtered;
            return;
        }

        if (CodeErrorLabel.IsVisible) CodeErrorLabel.IsVisible = false;
        if (GeneralErrorBorder.IsVisible) GeneralErrorBorder.IsVisible = false;

        // Botão habilitado apenas quando os 4 caracteres estiverem preenchidos
        AssociateButton.IsEnabled = filtered.Length == 4 && !_associationDone;
        System.Diagnostics.Debug.WriteLine(filtered);
        var fullCode = GetFullCode();
        System.Diagnostics.Debug.WriteLine($"Código Final Enviado: {fullCode}");
    }

    private string GetFullCode() => $"ESP32-{DeviceCodeSuffixEntry.Text?.Trim().ToUpperInvariant()}";
    
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
        SetLoadingState(true);

        try
        {
            // Passo 1: Salva a associação no backend
            var editRequest = new EditPlantRequest
            {
                PlantId = _plantId!,
                Name = string.Empty,  // nome não muda nesta tela; o backend deve ignorar campos vazios
                DeviceCode = fullCode
            };

            


            var editResult = await _editService.UpdatePlantAsync(editRequest);

            if (!editResult.Success)
            {
                ShowError(editResult.ErrorMessage ?? "Não foi possível associar o dispositivo.");
                return;
            }

            // Passo 2: Verifica status de comunicação do ESP32 em tempo real
            SetLoadingState(false);
            ShowStatusCard(checking: true);

            var verifyResult = await _verificationService.VerifyDeviceAsync(fullCode);
           

            if (!verifyResult.Success)
            {
                // Associação foi salva, mas dispositivo não foi encontrado
                // (código registrado mas ESP32 nunca enviou heartbeat)
                ShowStatusCard(checking: false, status: DeviceVerificationStatus.NotFound);
                return;
            }

            ShowStatusCard(checking: false, status: verifyResult.Status);
            _associationDone = true;
            AssociateButton.IsVisible = false;
            DoneButton.IsVisible = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao associar dispositivo: {ex.Message}");
            ShowError("Ocorreu um erro inesperado. Tente novamente.");
        }
        finally
        {
            SetLoadingState(false);
        }
        
    }

    // ===================== ESTADOS DO CARD DE STATUS =====================

    private void ShowStatusCard(bool checking, DeviceVerificationStatus? status = null)
    {

        StatusCard.IsVisible = true;

        if (checking)
        {
            ApplyStatusCardTheme(
                bgColor: "#F4FBF4",
                strokeColor: "#D7E5D9");

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

            default: // NotFound
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

    private async void OnDoneClicked(object? sender, EventArgs e)
    {
        // Retorna para Detalhes da Planta, que recarregará automaticamente
        // ao aparecer e exibirá o novo status de dispositivo.
        await Shell.Current.GoToAsync("..");
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
*/