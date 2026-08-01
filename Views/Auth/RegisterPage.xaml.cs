using KaaDebug.Core.Interfaces.Auth;
using System.Text.RegularExpressions;


namespace KaaDebug.Views.Auth;

public partial class RegisterPage : ContentPage
{
    private readonly IRegisterService _registerService;
    private bool _isPasswordVisible;

    public RegisterPage(IRegisterService registerService)
    {
        InitializeComponent();
        _registerService = registerService;
    }

    private void OnAnyFieldChanged(object? sender, TextChangedEventArgs e)
    {
        if (GeneralErrorBorder.IsVisible)
            GeneralErrorBorder.IsVisible = false;

        if (sender == NameEntry && NameErrorLabel.IsVisible)
            NameErrorLabel.IsVisible = false;

        if (sender == EmailEntry && EmailErrorLabel.IsVisible)
            EmailErrorLabel.IsVisible = false;

        if (sender == PasswordEntry && PasswordErrorLabel.IsVisible)
            PasswordErrorLabel.IsVisible = false;

        if (sender == ConfirmPasswordEntry && ConfirmPasswordErrorLabel.IsVisible)
            ConfirmPasswordErrorLabel.IsVisible = false;
    }

    private void OnTermsCheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        if (e.Value && TermsErrorLabel.IsVisible)
            TermsErrorLabel.IsVisible = false;
    }

    private void OnTermsLabelTapped(object? sender, EventArgs e)
    {
        // Alterna o checkbox também ao tocar no texto, melhorando a usabilidade
        TermsCheckBox.IsChecked = !TermsCheckBox.IsChecked;

        // TODO: quando as páginas de Termos/Privacidade existirem,
        // abrir aqui via Shell.Current.GoToAsync("TermsOfUse") ou WebView.
    }

    private void OnTogglePasswordClicked(object? sender, EventArgs e)
    {
        _isPasswordVisible = !_isPasswordVisible;
        PasswordEntry.IsPassword = !_isPasswordVisible;
        ConfirmPasswordEntry.IsPassword = !_isPasswordVisible;
        TogglePasswordButton.Source = _isPasswordVisible ? "eye_black_off.png" : "eye_black.png";
    }

    private async void OnRegisterClicked(object? sender, EventArgs e)
    {
        if (!ValidateFields())
            return;

        await PerformRegisterAsync();
    }

    private bool ValidateFields()
    {
        bool isValid = true;

        var name = NameEntry.Text?.Trim() ?? string.Empty;
        var email = EmailEntry.Text?.Trim() ?? string.Empty;
        var password = PasswordEntry.Text ?? string.Empty;
        var confirmPassword = ConfirmPasswordEntry.Text ?? string.Empty;

        if (string.IsNullOrWhiteSpace(name) || name.Length < 3)
        {
            NameErrorLabel.IsVisible = true;
            isValid = false;
        }

        if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
        {
            EmailErrorLabel.IsVisible = true;
            isValid = false;
        }

        if (password.Length < 6)
        {
            PasswordErrorLabel.IsVisible = true;
            isValid = false;
        }

        if (password != confirmPassword)
        {
            ConfirmPasswordErrorLabel.IsVisible = true;
            isValid = false;
        }

        if (!TermsCheckBox.IsChecked)
        {
            TermsErrorLabel.IsVisible = true;
            isValid = false;
        }

        return isValid;
    }

    private static bool IsValidEmail(string email) =>
        Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

    private async Task PerformRegisterAsync()
    {
        SetLoadingState(true);

        try
        {
            var request = new RegisterRequest
            {
                Name = NameEntry.Text!.Trim(),
                Email = EmailEntry.Text!.Trim(),
                Password = PasswordEntry.Text!
            };

            var result = await _registerService.RegisterAsync(request);

            if (!result.Success)
            {
                ShowGeneralError(result.ErrorMessage ?? "Não foi possível criar sua conta. Tente novamente.");
                return;
            }

            await ShowSuccessAndRedirectToLoginAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro inesperado no cadastro: {ex.Message}");
            ShowGeneralError("Ocorreu um erro inesperado. Tente novamente.");
        }
        finally
        {
            SetLoadingState(false);
        }
    }

    /// <summary>
    /// Conforme o fluxo definido: após concluir o cadastro, o usuário é
    /// direcionado para o Login (não há login automático nesta etapa).
    /// </summary>
    private async Task ShowSuccessAndRedirectToLoginAsync()
    {
        await DisplayAlert(
            "Conta criada!",
            "Sua conta foi criada com sucesso. Faça login para continuar.",
            "OK");

        await Shell.Current.GoToAsync("//Login");
    }

    private void SetLoadingState(bool isLoading)
    {
        RegisterButton.IsEnabled = !isLoading;
        RegisterButton.Text = isLoading ? string.Empty : "Criar conta";
        RegisterLoadingIndicator.IsVisible = isLoading;
        RegisterLoadingIndicator.IsRunning = isLoading;

        NameEntry.IsEnabled = !isLoading;
        EmailEntry.IsEnabled = !isLoading;
        PasswordEntry.IsEnabled = !isLoading;
        ConfirmPasswordEntry.IsEnabled = !isLoading;
        TermsCheckBox.IsEnabled = !isLoading;
    }

    private void ShowGeneralError(string message)
    {
        GeneralErrorLabel.Text = message;
        GeneralErrorBorder.IsVisible = true;
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//Login");
    }

    private async void OnLoginTapped(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//Login");
    }
}