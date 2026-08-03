using KaaDebug.Core.Interfaces.Auth;
using System.Diagnostics;

namespace KaaDebug.Views.Auth;

public partial class SplashPage : ContentPage
{
    private readonly IAuthService _authService;

    // Tempo mínimo de exibição da splash, para evitar "flash" na tela
    // mesmo quando a verificação de sessão é muito rápida.
    private const int MinSplashDurationMs = 1200;

    public SplashPage(IAuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }



    protected override async void OnAppearing()
    {
        base.OnAppearing();
        Debug.WriteLine("====== SPLASH PAGE APARECEU ======");
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
            Debug.WriteLine("Antes");

            await Task.Delay(2000);

            Debug.WriteLine("Depois");

            sessionValid = false;
        }
        catch (Exception ex)
        {
            // Falha ao verificar sessão (ex: erro inesperado local) não deve
            // travar o usuário na Splash. Em caso de erro, tratamos como
            // sessão inválida e seguimos para o Login.
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
