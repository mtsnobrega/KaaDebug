/*
 * Responsabilidade:
 * Ponto de entrada e configuração global do aplicativo KaaDebug.
 * 
 * Papel na arquitetura:
 * Atua como o Composition Root para Injeção de Dependência (DI). É responsável por 
 * orquestrar a criação do HttpClient, registrar os provedores de segurança (Token) 
 * e configurar o tempo de vida (Singleton vs Transient) de todos os Services e Pages.
 * 
 * Dependências principais:
 * - ApiClient e ApiConstants (Infraestrutura)
 * - Interfaces de Serviço e suas implementações
 * - Views (Páginas do aplicativo)
 * 
 * Fluxo:
 * Chamado pelo sistema operacional no momento do "cold start" (inicialização do app) 
 * para construir o contêiner de dependências antes da primeira tela ser renderizada.
 */

using KaaDebug.Services.Diagnostic;
using KaaDebug.Services.Notifications;
using KaaDebug.Services.Plants;
using KaaDebug.Services.Profile;
using KaaDebug.Views.Auth;
using KaaDebug.Views.Care;
using KaaDebug.Views.Dashboard;
using KaaDebug.Views.Devices;
using KaaDebug.Views.Notifications;
using KaaDebug.Views.Plants;
using KaaDebug.Views.Profile;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using KaaDebug.Infrastructure.http;
using KaaDebug.Core.Interfaces.Auth;
using KaaDebug.Services.Auth;
using KaaDebug.Core.Interfaces.Plants;
using KaaDebug.Core.Interfaces.Devices;
using KaaDebug.Services.Devices;
using KaaDebug.Core.Interfaces.Notifications;
using KaaDebug.Core.Interfaces.Diagnostic;
using KaaDebug.Core.Interfaces.Profile;

namespace KaaDebug;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSansRegular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSansSemibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        RegisterHttpClient(builder.Services);
        RegisterServices(builder.Services);
        RegisterPages(builder.Services);

        return builder.Build();
    }

    // ══════════════════════════════════════════════════════════════════════════
    // HTTP CLIENT
    // Configura o ApiClient com a URL base da API e o provedor de token JWT.
    // O HttpClient é registrado como Singleton via AddHttpClient para reutilizar
    // conexões e evitar o problema de socket exhaustion.
    // ══════════════════════════════════════════════════════════════════════════
    private static void RegisterHttpClient(IServiceCollection services)
    {
        services.AddSingleton<IAuthTokenProvider, SecureStorageTokenProvider>();

        services.AddHttpClient<ApiClient>(client =>
        {
            client.BaseAddress = new Uri(ApiConstants.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });
    }

    // ══════════════════════════════════════════════════════════════════════════
    // SERVIÇOS
    // Todos os serviços reais que consomem a API via ApiClient.
    // Registrados como Transient: cada tela recebe sua própria instância,
    // garantindo que não haja estado compartilhado entre navegações.
    // ══════════════════════════════════════════════════════════════════════════
    private static void RegisterServices(IServiceCollection services)
    {
        // Auth
        services.AddSingleton<IAuthService, AuthService>();
        services.AddTransient<ILoginService, LoginService>();
        services.AddTransient<IRegisterService, RegisterService>();
        services.AddTransient<IPasswordRecoveryService, PasswordRecoveryService>();

        // Dashboard
        services.AddTransient<IDashboardService, DashboardService>();

        // Plantas
        services.AddTransient<IPlantsListService, PlantsListService>();
        services.AddSingleton<IPlantsCatalogService, PlantsCatalogService>();
        services.AddSingleton<IPlantsRegistrationService, PlantsRegistrationService>();
        services.AddTransient<IPlantDetailsService, PlantDetailsService>();
        services.AddSingleton<IPlantsEditService, PlantsEditService>();
        services.AddSingleton<IPlantHistoryService, PlantHistoryService>();

        // Dispositivos
        services.AddSingleton<IDeviceVerificationService, DeviceVerificationService>();
        services.AddTransient<IDeviceAssociationService, DeviceAssociationService>();

        // Notificações
        services.AddSingleton<INotificationsService, NotificationsService>();

        // Conteúdo e IA
        services.AddSingleton<IPlantTipsService, PlantTipsService>();
        services.AddSingleton<IDiagnosisService, DiagnosisService>();

        // Perfil
        services.AddSingleton<IProfileService, ProfileService>();
    }

    // ══════════════════════════════════════════════════════════════════════════
    // PÁGINAS
    // Todas as páginas como Transient: cada navegação cria uma instância
    // limpa, evitando estado residual de navegações anteriores.
    // ══════════════════════════════════════════════════════════════════════════
    private static void RegisterPages(IServiceCollection services)
    {
        // Autenticação
        services.AddTransient<SplashPage>();
        services.AddTransient<LoginPage>();
        services.AddTransient<RegisterPage>();
        services.AddTransient<PasswordRecoveryPage>();

        // Principal
        services.AddTransient<DashboardPage>();

        // Plantas
        services.AddTransient<PlantsListPage>();
        services.AddTransient<SelectSpeciesPage>();
        services.AddTransient<RegisterPlantPage>();
        //services.AddTransient<PlantDetailsService>();
        services.AddTransient<PlantDetailsService>();
        services.AddTransient<PlantsEditService>();
        services.AddTransient<PlantHistoryPage>();

        // Dispositivos
        services.AddTransient<RegisterDevicePage>();


        // Notificações
        services.AddTransient<NotificationsPage>();

        // Conteúdo e IA
        services.AddTransient<PlantTipsPage>();
        services.AddTransient<DiagnosisService>();

        // Perfil
        services.AddTransient<ProfilePage>();
    }
}