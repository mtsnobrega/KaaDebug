/*
 * Responsabilidade:
 * Code-Behind da tela inicial de carregamento (Splash Screen). Responsável por 
 * decidir qual fluxo o aplicativo deve seguir (Logado vs Deslogado).
 * 
 * Papel na arquitetura:
 * Camada de Apresentação/Roteador inicial. Consome o `IAuthService` para validar 
 * a expiração do Token JWT antes de permitir a entrada no sistema.
 * 
 * Fluxo:
 * OnAppearing -> IsSessionValidAsync (Check JWT) -> Delay Mínimo -> Shell.GoToAsync.
 */
using KaaDebug.Core.Interfaces.Auth;
using System.Diagnostics;

namespace KaaDebug.Views.Auth;

public partial class SplashPage : ContentPage
{
    private readonly IAuthService _authService;

    // Tempo mínimo de exibição da splash, para evitar "flash" na tela
    private const int MinSplashDurationMs = 1200;

    public SplashPage(IAuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await InitializeAppAsync();
    }

    /// <summary>
    /// Executa as verificações iniciais do app (sessão, preferências locais)
    /// e direciona o usuário para a tela correta.
    /// </summary>
    private async Task InitializeAppAsync()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        bool sessionValid;

        try
        {
            StatusLabel.Text = "Verificando sessão...";
            StatusLabel.IsVisible = true;

            //sessionValid = await _authService.IsSessionValidAsync();
            await Task.Delay(2000);
            sessionValid = false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao verificar sessão: {ex.Message}");
            sessionValid = false;
        }

        // Garante tempo mínimo de exibição da splash (evita "flash" na tela)
        var elapsed = stopwatch.ElapsedMilliseconds;
        if (elapsed < MinSplashDurationMs)
        {
            await Task.Delay((int)(MinSplashDurationMs - elapsed));
        }

        await NavigateAsync(sessionValid);
    }

    private async Task NavigateAsync(bool sessionValid)
    {
        var route = sessionValid ? "//Dashboard" : "//Login";

        try
        {
            await Shell.Current.GoToAsync(route);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            System.Diagnostics.Debug.WriteLine($"Erro de navegação a partir da Splash: {ex.Message}");
        }
    }
}
