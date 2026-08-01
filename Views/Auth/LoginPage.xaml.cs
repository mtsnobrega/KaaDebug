using KaaDebug.Core.Interfaces.Auth;
using System.Text.RegularExpressions;

namespace KaaDebug.Views.Auth;

public partial class LoginPage : ContentPage
{
    private readonly ILoginService _loginService;
    private readonly IAuthService _authService;
    private bool _isPasswordVisible;

    public LoginPage(ILoginService loginService, IAuthService authService)
    {
        InitializeComponent();
        _loginService = loginService;
        _authService = authService;
    }

    /// <summary>
    /// Limpa mensagens de erro conforme o usuário digita, evitando que
    /// erros antigos fiquem visíveis enquanto ele corrige os dados.
    /// </summary>
    private void OnAnyFieldChanged(object? sender, TextChangedEventArgs e)
    {
        if (GeneralErrorBorder.IsVisible)
            GeneralErrorBorder.IsVisible = false;

        if (sender == EmailEntry && EmailErrorLabel.IsVisible)
            EmailErrorLabel.IsVisible = false;

        if (sender == PasswordEntry && PasswordErrorLabel.IsVisible)
            PasswordErrorLabel.IsVisible = false;
    }

    private void OnTogglePasswordClicked(object? sender, EventArgs e)
    {
        _isPasswordVisible = !_isPasswordVisible;
        PasswordEntry.IsPassword = !_isPasswordVisible;
        TogglePasswordButton.Source = _isPasswordVisible ? "eye_off.png" : "eye.png";
    }

    private async void OnLoginClicked(object? sender, EventArgs e)
    {
        if (!ValidateFields())
            return;

        await PerformLoginAsync();
    }

    /// <summary>
    /// Validação local dos campos antes de chamar a API.
    /// Evita requisições desnecessárias para erros óbvios (campo vazio,
    /// formato de e-mail incorreto).
    /// </summary>
    private bool ValidateFields()
    {
        bool isValid = true;
        var email = EmailEntry.Text?.Trim() ?? string.Empty;
        var password = PasswordEntry.Text ?? string.Empty;

        if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
        {
            EmailErrorLabel.IsVisible = true;
            isValid = false;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            PasswordErrorLabel.IsVisible = true;
            isValid = false;
        }

        return isValid;
    }

    private static bool IsValidEmail(string email)
    {
        // Regex simples, suficiente para validação de UI.
        // A validação definitiva de existência do e-mail é responsabilidade do backend.
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    private async Task PerformLoginAsync()
    {
        SetLoadingState(true);

        try
        {
            var email = EmailEntry.Text!.Trim();
            var password = PasswordEntry.Text!;

            var result = await _loginService.LoginAsync(email, password);

            if (!result.Success)
            {
                ShowGeneralError(result.ErrorMessage ?? "Não foi possível entrar. Tente novamente.");
                return;
            }

            await _authService.SaveSessionAsync(result.Token!);
            await Shell.Current.GoToAsync("//Dashboard");
        }
        catch (Exception ex)
        {
            // Erros inesperados (ex: falha de SecureStorage) não devem
            // travar o usuário sem explicação.
            System.Diagnostics.Debug.WriteLine($"Erro inesperado no login: {ex.Message}");
            ShowGeneralError("Ocorreu um erro inesperado. Tente novamente.");
        }
        finally
        {
            SetLoadingState(false);
        }
    }

    private void SetLoadingState(bool isLoading)
    {
        LoginButton.IsEnabled = !isLoading;
        LoginButton.Text = isLoading ? string.Empty : "Entrar";
        LoginLoadingIndicator.IsVisible = isLoading;
        LoginLoadingIndicator.IsRunning = isLoading;

        EmailEntry.IsEnabled = !isLoading;
        PasswordEntry.IsEnabled = !isLoading;
    }

    private void ShowGeneralError(string message)
    {
        GeneralErrorLabel.Text = message;
        GeneralErrorBorder.IsVisible = true;
    }

    private async void OnForgotPasswordTapped(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//PasswordRecovery");
    }

    private async void OnRegisterTapped(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//Register");
    }

}