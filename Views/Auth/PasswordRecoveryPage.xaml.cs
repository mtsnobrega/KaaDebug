using KaaDebug.Core.Interfaces.Auth;
using System.Text.RegularExpressions;

namespace KaaDebug.Views.Auth;

public partial class PasswordRecoveryPage : ContentPage
{
    private enum Step
    {
        Email,
        Code,
        NewPassword,
        Success
    }

    private readonly IPasswordRecoveryService _recoveryService;

    private Step _currentStep = Step.Email;
    private string _email = string.Empty;
    private IDispatcherTimer? _resendCooldownTimer;
    private int _resendSecondsRemaining;

    private Entry[] OtpEntries => new[]
    {
        OtpDigit1, OtpDigit2, OtpDigit3, OtpDigit4, OtpDigit5, OtpDigit6
    };

    public PasswordRecoveryPage(IPasswordRecoveryService recoveryService)
    {
        InitializeComponent();
        _recoveryService = recoveryService;
    }

    // ===================== ETAPA 1: E-MAIL =====================

    private void OnEmailChanged(object? sender, TextChangedEventArgs e)
    {
        if (EmailErrorLabel.IsVisible) EmailErrorLabel.IsVisible = false;
        if (EmailGeneralErrorBorder.IsVisible) EmailGeneralErrorBorder.IsVisible = false;
    }

    private async void OnSendCodeClicked(object? sender, EventArgs e)
    {
        var email = RecoveryEmailEntry.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
        {
            EmailErrorLabel.IsVisible = true;
            return;
        }

        SetEmailStepLoading(true);

        try
        {
            var result = await _recoveryService.RequestCodeAsync(email);

            if (!result.Success)
            {
                /*
                EmailGeneralErrorLabel.Text = result.ErrorMessage ?? "Não foi possível enviar o código.";
                EmailGeneralErrorBorder.IsVisible = true;
                return;
                */
                ClearOtpFields();

                CodeErrorLabel.Text = result.ErrorMessage ?? "Código incorreto.";
                CodeErrorLabel.IsVisible = true;
                return;
            }

            _email = email;
            CodeSentToLabel.Text = $"Enviamos um código de 6 dígitos para {MaskEmail(email)}.";
            GoToStep(Step.Code);
            StartResendCooldown();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao solicitar código: {ex.Message}");
            EmailGeneralErrorLabel.Text = "Ocorreu um erro inesperado. Tente novamente.";
            EmailGeneralErrorBorder.IsVisible = true;
        }
        finally
        {
            SetEmailStepLoading(false);
        }
    }

    private void SetEmailStepLoading(bool isLoading)
    {
        SendCodeButton.IsEnabled = !isLoading;
        SendCodeButton.Text = isLoading ? string.Empty : "Enviar código";
        EmailStepLoadingIndicator.IsVisible = isLoading;
        EmailStepLoadingIndicator.IsRunning = isLoading;
        RecoveryEmailEntry.IsEnabled = !isLoading;
    }

    private static bool IsValidEmail(string email) =>
        Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

    /// <summary>
    /// Mascara parte do e-mail por privacidade visual, ex: "jo***@gmail.com"
    /// </summary>
    private static string MaskEmail(string email)
    {
        var parts = email.Split('@');
        if (parts.Length != 2 || parts[0].Length <= 2)
            return email;

        var visible = parts[0][..2];
        return $"{visible}***@{parts[1]}";
    }

    // ===================== ETAPA 2: CÓDIGO (OTP) =====================

    private void OnOtpDigitChanged(object? sender, TextChangedEventArgs e)
    {
        //if (CodeErrorLabel.IsVisible) CodeErrorLabel.IsVisible = false;
        if (!string.IsNullOrEmpty(e.NewTextValue))
            CodeErrorLabel.IsVisible = false;

        if (sender is not Entry currentEntry) return;


        var entries = OtpEntries;
        var index = Array.IndexOf(entries, currentEntry);

        // Aceita apenas dígitos
        if (!string.IsNullOrEmpty(e.NewTextValue) && !char.IsDigit(e.NewTextValue[^1]))
        {
            currentEntry.Text = e.OldTextValue;
            return;
        }

        // Auto-avança para o próximo campo ao digitar
        if (!string.IsNullOrEmpty(e.NewTextValue) && index < entries.Length - 1)
        {
            entries[index + 1].Focus();
        }

        // Habilita o botão "Validar código" somente quando todos os 6 dígitos estiverem preenchidos
        bool isValid = entries.All(entry => !string.IsNullOrEmpty(entry.Text));

        ValidateCodeButton.IsEnabled = isValid;

        // Altera para a cor ativa ou volta para a cor padrão (cinza)
        ValidateCodeButton.BackgroundColor = isValid
            ? Color.FromArgb("#2E7D32") 
            : Color.FromArgb("#CCCCCC");
    }

    private string GetOtpCode() => string.Concat(OtpEntries.Select(entry => entry.Text ?? string.Empty));

    private async void OnValidateCodeClicked(object? sender, EventArgs e)
    {
        var code = GetOtpCode();

        if (code.Length != 6)
        {
            CodeErrorLabel.Text = "Informe os 6 dígitos do código.";
            CodeErrorLabel.IsVisible = true;
            return;
        }

        SetCodeStepLoading(true);

        try
        {
            var result = await _recoveryService.ValidateCodeAsync(_email, code);

            if (!result.Success)
            {
                CodeErrorLabel.Text = result.ErrorMessage ?? "Código incorreto.";
                CodeErrorLabel.IsVisible = true;
                ClearOtpFields();
                return;
            }

            GoToStep(Step.NewPassword);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao validar código: {ex.Message}");
            CodeErrorLabel.Text = "Ocorreu um erro inesperado. Tente novamente.";
            CodeErrorLabel.IsVisible = true;

        }
        finally
        {
            SetCodeStepLoading(false);
        }
    }

    private void ClearOtpFields()
    {
        foreach (var entry in OtpEntries)
            entry.Text = string.Empty;

        OtpDigit1.Focus();
        ValidateCodeButton.IsEnabled = false;
    }

    private void SetCodeStepLoading(bool isLoading)
    {
        ValidateCodeButton.Text = isLoading ? string.Empty : "Validar código";
        CodeStepLoadingIndicator.IsVisible = isLoading;
        CodeStepLoadingIndicator.IsRunning = isLoading;

        foreach (var entry in OtpEntries)
            entry.IsEnabled = !isLoading;
    }

    private async void OnResendCodeTapped(object? sender, EventArgs e)
    {
        if (_resendSecondsRemaining > 0)
            return; // ainda em cooldown, ignora toque

        try
        {
            await _recoveryService.RequestCodeAsync(_email);
            ClearOtpFields();
            StartResendCooldown();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao reenviar código: {ex.Message}");
        }
    }

    /// <summary>
    /// Inicia cooldown de 60s antes de permitir novo reenvio,
    /// evitando spam de solicitações de código.
    /// </summary>
    private void StartResendCooldown()
    {
        _resendSecondsRemaining = 60;
        UpdateResendLabel();

        _resendCooldownTimer ??= Dispatcher.CreateTimer();
        _resendCooldownTimer.Interval = TimeSpan.FromSeconds(1);
        _resendCooldownTimer.Tick += OnResendCooldownTick;
        _resendCooldownTimer.Start();
    }

    private void OnResendCooldownTick(object? sender, EventArgs e)
    {
        _resendSecondsRemaining--;
        UpdateResendLabel();

        if (_resendSecondsRemaining <= 0)
        {
            _resendCooldownTimer?.Stop();
        }
    }

    private void UpdateResendLabel()
    {
        ResendCodeLabel.Text = _resendSecondsRemaining > 0
            ? $"Reenviar ({_resendSecondsRemaining}s)"
            : "Reenviar";

        ResendCodeLabel.TextColor = _resendSecondsRemaining > 0
            ? Color.FromArgb("#8A9A8C")
            : Color.FromArgb("#2E7D32");
    }

    // ===================== ETAPA 3: NOVA SENHA =====================

    private void OnNewPasswordFieldsChanged(object? sender, TextChangedEventArgs e)
    {
        if (NewPasswordErrorLabel.IsVisible) NewPasswordErrorLabel.IsVisible = false;
        if (ConfirmPasswordErrorLabel.IsVisible) ConfirmPasswordErrorLabel.IsVisible = false;
        if (ResetGeneralErrorBorder.IsVisible) ResetGeneralErrorBorder.IsVisible = false;
    }

    private async void OnResetPasswordClicked(object? sender, EventArgs e)
    {
        var newPassword = NewPasswordEntry.Text ?? string.Empty;
        var confirmPassword = ConfirmPasswordEntry.Text ?? string.Empty;

        bool isValid = true;

        if (newPassword.Length < 6)
        {
            NewPasswordErrorLabel.IsVisible = true;
            isValid = false;
        }

        if (newPassword != confirmPassword)
        {
            ConfirmPasswordErrorLabel.IsVisible = true;
            isValid = false;
        }

        if (!isValid)
            return;

        SetNewPasswordStepLoading(true);

        try
        {
            var code = GetOtpCode();
            var result = await _recoveryService.ResetPasswordAsync(_email, code, newPassword);

            if (!result.Success)
            {
                ResetGeneralErrorLabel.Text = result.ErrorMessage ?? "Não foi possível redefinir a senha.";
                ResetGeneralErrorBorder.IsVisible = true;
                return;
            }

            GoToStep(Step.Success);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao redefinir senha: {ex.Message}");
            ResetGeneralErrorLabel.Text = "Ocorreu um erro inesperado. Tente novamente.";
            ResetGeneralErrorBorder.IsVisible = true;
        }
        finally
        {
            SetNewPasswordStepLoading(false);
        }
    }

    private void SetNewPasswordStepLoading(bool isLoading)
    {
        ResetPasswordButton.IsEnabled = !isLoading;
        ResetPasswordButton.Text = isLoading ? string.Empty : "Redefinir senha";
        NewPasswordStepLoadingIndicator.IsVisible = isLoading;
        NewPasswordStepLoadingIndicator.IsRunning = isLoading;
        NewPasswordEntry.IsEnabled = !isLoading;
        ConfirmPasswordEntry.IsEnabled = !isLoading;
    }

    // ===================== NAVEGAÇÃO ENTRE ETAPAS =====================

    private void GoToStep(Step step)
    {
        _currentStep = step;

        EmailStepLayout.IsVisible = step == Step.Email;
        CodeStepLayout.IsVisible = step == Step.Code;
        NewPasswordStepLayout.IsVisible = step == Step.NewPassword;
        SuccessStepLayout.IsVisible = step == Step.Success;

        UpdateStepDots(step);
    }

    private void UpdateStepDots(Step step)
    {
        var active = Color.FromArgb("#2E7D32");
        var inactive = Color.FromArgb("#D7E5D9");

        StepDot1.Color = step >= Step.Email ? active : inactive;
        StepDot2.Color = step >= Step.Code ? active : inactive;
        StepDot3.Color = step >= Step.NewPassword ? active : inactive;
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        // Dentro do wizard, "voltar" retorna uma etapa; na primeira etapa, sai da tela.
        switch (_currentStep)
        {
            case Step.Code:
                GoToStep(Step.Email);
                break;
            case Step.NewPassword:
                GoToStep(Step.Code);
                break;
            default:
                await Shell.Current.GoToAsync("//Login");
                break;
        }
    }

    private async void OnGoToLoginClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//Login");
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _resendCooldownTimer?.Stop();
    }
}