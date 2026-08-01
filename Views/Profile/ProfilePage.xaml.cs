using KaaDebug.Core.Interfaces.Auth;
using KaaDebug.Core.Interfaces.Profile;
using KaaDebug.Core.Models.Auth;

namespace KaaDebug.Views.Profile;

public partial class ProfilePage : ContentPage
{
    private readonly IProfileService _profileService;
    private readonly IAuthService _authService;
    private bool _hasUnsavedChanges;

    public ProfilePage(IProfileService profileService, IAuthService authService)
    {
        InitializeComponent();
        _profileService = profileService;
        _authService = authService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadProfileAsync();
    }

    private async Task LoadProfileAsync()
    {
        LoadingStateLayout.IsVisible = true;
        ContentLayout.IsVisible = false;

        try
        {
            var result = await _profileService.GetProfileAsync();

            if (!result.Success || result.Profile is null)
            {
                await DisplayAlert("Erro", "Não foi possível carregar o perfil.", "OK");
                return;
            }

            var profile = result.Profile;

            // Iniciais do avatar (primeira letra do nome)
            AvatarInitialsLabel.Text = profile.Name.Length > 0
                ? profile.Name[0].ToString().ToUpperInvariant()
                : "?";

            ProfileNameLabel.Text = profile.Name;
            ProfileEmailLabel.Text = profile.Email;

            NameEntry.Text = profile.Name;
            EmailReadOnlyLabel.Text = profile.Email;

            NotificationsSwitch.IsToggled = profile.NotificationsEnabled;
            CriticalOnlySwitch.IsToggled = profile.CriticalAlertsOnly;

            _hasUnsavedChanges = false;
            UpdateSaveVisibility();

            ContentLayout.IsVisible = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao carregar perfil: {ex.Message}");
            await DisplayAlert("Erro", "Ocorreu um erro inesperado.", "OK");
        }
        finally
        {
            LoadingStateLayout.IsVisible = false;
        }
    }

    // ===================== DETECÇÃO DE MUDANÇAS =====================

    private void OnProfileFieldChanged(object? sender, EventArgs e)
    {
        _hasUnsavedChanges = true;
        UpdateSaveVisibility();
        FeedbackBorder.IsVisible = false;
    }

    private void OnNotificationsSwitchToggled(object? sender, ToggledEventArgs e)
    {
        // Desabilita "apenas críticos" quando notificações estão desligadas
        CriticalOnlySwitch.IsEnabled = e.Value;
        OnProfileFieldChanged(sender, e);
    }

    private void UpdateSaveVisibility()
    {
        SaveButton.IsVisible = _hasUnsavedChanges;
    }

    // ===================== SALVAR =====================

    private async void OnSaveTapped(object? sender, EventArgs e)
    {
        var name = NameEntry.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(name) || name.Length < 3)
        {
            ShowFeedback("Informe um nome válido (mínimo 3 caracteres).", isError: true);
            return;
        }

        SetSaveLoadingState(true);

        try
        {
            var request = new UpdateProfileRequest
            {
                Name = name,
                NotificationsEnabled = NotificationsSwitch.IsToggled,
                CriticalAlertsOnly = CriticalOnlySwitch.IsToggled
            };

            var result = await _profileService.UpdateProfileAsync(request);

            if (!result.Success)
            {
                ShowFeedback(result.ErrorMessage ?? "Não foi possível salvar.", isError: true);
                return;
            }

            // Atualiza o avatar e nome exibidos no topo
            AvatarInitialsLabel.Text = name[0].ToString().ToUpperInvariant();
            ProfileNameLabel.Text = name;

            _hasUnsavedChanges = false;
            UpdateSaveVisibility();
            ShowFeedback("Perfil atualizado com sucesso!", isError: false);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao salvar perfil: {ex.Message}");
            ShowFeedback("Ocorreu um erro inesperado. Tente novamente.", isError: true);
        }
        finally
        {
            SetSaveLoadingState(false);
        }
    }

    private void SetSaveLoadingState(bool isLoading)
    {
        SaveButton.IsEnabled = !isLoading;
        SaveButton.Text = isLoading ? string.Empty : "Salvar alterações";
        SaveLoadingIndicator.IsVisible = isLoading;
        SaveLoadingIndicator.IsRunning = isLoading;
        NameEntry.IsEnabled = !isLoading;
    }

    private void ShowFeedback(string message, bool isError)
    {
        FeedbackLabel.Text = message;
        FeedbackLabel.TextColor = isError ? Color.FromArgb("#C62828") : Color.FromArgb("#2E7D32");
        FeedbackBorder.BackgroundColor = isError ? Color.FromArgb("#FDECEA") : Color.FromArgb("#EAF5EA");
        FeedbackBorder.Stroke = new SolidColorBrush(
            isError ? Color.FromArgb("#F5C2C0") : Color.FromArgb("#A5D6A7"));
        FeedbackBorder.IsVisible = true;
    }

    // ===================== ALTERAR SENHA =====================

    private async void OnChangePasswordTapped(object? sender, EventArgs e)
    {
        // Pede a senha atual via prompt nativo (simples e seguro)
        var currentPassword = await DisplayPromptAsync(
            "Alterar senha",
            "Digite sua senha atual:",
            placeholder: "Senha atual",
            maxLength: 50,
            keyboard: Keyboard.Default);

        if (string.IsNullOrWhiteSpace(currentPassword)) return;

        var newPassword = await DisplayPromptAsync(
            "Alterar senha",
            "Digite a nova senha (mínimo 6 caracteres):",
            placeholder: "Nova senha",
            maxLength: 50,
            keyboard: Keyboard.Default);

        if (string.IsNullOrWhiteSpace(newPassword)) return;

        if (newPassword.Length < 6)
        {
            await DisplayAlert("Senha inválida", "A nova senha deve ter pelo menos 6 caracteres.", "OK");
            return;
        }

        var confirmPassword = await DisplayPromptAsync(
            "Alterar senha",
            "Confirme a nova senha:",
            placeholder: "Confirmar nova senha",
            maxLength: 50,
            keyboard: Keyboard.Default);

        if (confirmPassword != newPassword)
        {
            await DisplayAlert("Erro", "As senhas não coincidem.", "OK");
            return;
        }

        try
        {
            var result = await _profileService.ChangePasswordAsync(currentPassword, newPassword);

            await DisplayAlert(
                result.Success ? "Senha alterada" : "Erro",
                result.Success ? "Sua senha foi alterada com sucesso." : result.ErrorMessage,
                "OK");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao alterar senha: {ex.Message}");
            await DisplayAlert("Erro", "Ocorreu um erro inesperado.", "OK");
        }
    }

    // ===================== LOGOUT =====================

    private async void OnLogoutClicked(object? sender, EventArgs e)
    {
        bool confirmed = await DisplayAlert(
            "Sair da conta",
            "Tem certeza que deseja sair?",
            "Sim, sair",
            "Cancelar");

        if (!confirmed) return;

        try
        {
            await _authService.ClearSessionAsync();
            await Shell.Current.GoToAsync("//Login");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao fazer logout: {ex.Message}");
        }
    }

    private async void OnBackClicked(object? sender, EventArgs e) =>
        await Shell.Current.GoToAsync("//Dashboard");
}