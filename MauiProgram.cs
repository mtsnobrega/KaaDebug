using KaaDebug.Core.Interfaces.Auth;
using KaaDebug.Core.Interfaces.Devices;
using KaaDebug.Core.Interfaces.Diagnostic;
using KaaDebug.Core.Interfaces.Notifications;
using KaaDebug.Core.Interfaces.Plants;
using KaaDebug.Core.Interfaces.Profile;
using KaaDebug.Services.Auth;
using KaaDebug.Services.Devices;
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

/*
namespace Budflow
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMicrocharts()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddTransient<SplashPage>();

            // Quando o LoginService/PlantApiClient forem criados futuramente,
            // registrar aqui também, ex:
            // builder.Services.AddHttpClient<IPlantApiClient, PlantApiClient>(client =>
            // {
            //     client.BaseAddress = new Uri("https://api.Budflow.com/");
            // });



            // LOGIN
            // Por enquanto usamos a Fake, pois a API de autenticação ainda não existe.
            // Quando o endpoint POST /auth/login estiver pronto, troque para:
            // builder.Services.AddHttpClient<ILoginService, LoginService>(client =>
            //   {
            //       client.BaseAddress = new Uri("https://api.Budflow.com/");
            //   });
            builder.Services.AddSingleton<ILoginService, LoginService>();
            builder.Services.AddTransient<LoginPage>();

            builder.Services.AddSingleton<IPasswordRecoveryService, FakePasswordRecoveryService>();
            builder.Services.AddTransient<PasswordRecoveryPage>();


            // CADASTRO DE USUÁRIO
            // Mesma lógica: Fake até o endpoint POST /auth/register existir.
            builder.Services.AddSingleton<IRegisterService, Register>();
            builder.Services.AddTransient<RegisterPage>();

            // DASHBOARD
            // Fake retorna cenário com plantas em status variados.
            // Para testar o estado vazio: new FakeDashboardService(simulateEmptyState: true)
            builder.Services.AddSingleton<IDashboardService, DashboardService>();
            builder.Services.AddTransient<DashboardPage>();

            // LISTA DE PLANTAS
            builder.Services.AddSingleton<IPlantsService, PlantsService>();
            builder.Services.AddTransient<PlantsListPage>();

            // CADASTRO DE PLANTA
            builder.Services.AddSingleton<ISpeciesService, PlantsSpecies>();
            builder.Services.AddSingleton<IPlantRegistrationService, PlantRegistration>();
            builder.Services.AddTransient<RegisterPlantPage>();
            builder.Services.AddTransient<SelectSpeciesPage>();

            // DETALHES DA PLANTA
            builder.Services.AddSingleton<IPlantDetailsService, PlantDetailsService>();
            builder.Services.AddTransient<PlantDetailsPage>();

            // EDITAR PLANTA
            builder.Services.AddSingleton<IPlantEditService, PlantEditService>();
            builder.Services.AddTransient<EditPlantPage>();

            // CADASTRO DE DISPOSITIVO
            builder.Services.AddSingleton<IDeviceVerificationService, DeviceVerificationService>();
            builder.Services.AddTransient<RegisterDevicePage>();

            // CENTRAL DE NOTIFICAÇÕES
            builder.Services.AddSingleton<INotificationsService, NotificationsService>();
            builder.Services.AddTransient<NotificationsPage>();

            // DICAS DE CUIDADOS
            builder.Services.AddSingleton<IBudflowService, BudflowService>();
            builder.Services.AddTransient<PlantTipsPage>();

            // DIAGNÓSTICO INTELIGENTE
            builder.Services.AddSingleton<IAiDiagnosisService, AiDiagnosisService>();
            builder.Services.AddTransient<AiDiagnosisPage>();

            // PERFIL DO USUÁRIO
            builder.Services.AddSingleton<IProfileService, ProfileService>();
            builder.Services.AddTransient<ProfilePage>();

            // HISTÓRICO DE LEITURAS
            builder.Services.AddSingleton<IPlantHistoryService, PlantHistoryService>();
            builder.Services.AddTransient<PlantHistoryPage>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}

*/


#if USE_SIMULATION
using KaaDebug.Simulation;
#endif

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
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        RegisterServices(builder.Services);
        RegisterPages(builder.Services);

        return builder.Build();
    }

    // =====================================================================
    // SERVIÇOS
    // =====================================================================

    private static void RegisterServices(IServiceCollection services)
    {
#if USE_SIMULATION
        RegisterSimulatedServices(services);
#else
        RegisterFakeServices(services);
#endif
    }

    // ---------------------------------------------------------------------
    // MODO SIMULAÇÃO
    // Ative adicionando USE_SIMULATION em:
    //   Propriedades do projeto → Build → Conditional compilation symbols
    //
    // Cenário inicializado:
    //   • Usuário "Ana Paula" já autenticado (Splash → Dashboard direto)
    //   • Credenciais de login: ana@Budflow.com / 123456
    //   • 4 plantas com status variados (saudável, atenção, crítico, sem dispositivo)
    //   • 3 dispositivos (online, offline, não associado)
    //   • 30 dias de histórico de leituras por sensor
    //   • Notificações em todos os níveis (lidas e não lidas)
    //   • Histórico de diagnósticos de IA
    //
    // Todas as ações (cadastrar planta, editar, excluir, marcar notificação,
    // etc.) são refletidas entre as telas via InMemoryDataStore compartilhado.
    // ---------------------------------------------------------------------

#if USE_SIMULATION
    private static void RegisterSimulatedServices(IServiceCollection services)
    {
        // Store compartilhado entre todos os serviços simulados
        var store = new InMemoryDataStore();
        var simulator = new AppScenarioSimulator(store);
        simulator.Initialize();

        services.AddSingleton(store);

        // Auth
        services.AddSingleton<IAuthService, SimulatedAuthService>();
        services.AddSingleton<ILoginService, SimulatedLoginService>();

        // Registro e recuperação de senha: mantém Fakes
        // (fluxos pontuais que não dependem de estado compartilhado)
        services.AddSingleton<IRegisterService,  RegisterService>();
        services.AddSingleton<IPasswordRecoveryService, PasswordRecoveryService>();

        // Plantas
        services.AddSingleton<IDashboardService, SimulatedDashboardService>();
        services.AddSingleton<IPlantsListService, SimulatedPlantsService>();
        services.AddSingleton<IPlantsCatalogService, SimulatedSpeciesService>();
        services.AddSingleton<IPlantsRegistrationService, SimulatedPlantRegistrationService>();
        services.AddSingleton<IPlantDetailsService, SimulatedPlantDetailsService>();
        services.AddSingleton<IPlantsEditService, SimulatedPlantEditService>();
        services.AddSingleton<IPlantHistoryService, SimulatedPlantHistoryService>();

        // Dispositivos
        services.AddSingleton<IDeviceVerificationService, SimulatedDeviceVerificationService>();

        // Notificações
        services.AddSingleton<INotificationsService, SimulatedNotificationsService>();

        // Conteúdo e IA
        services.AddSingleton<IPlantTipsService, SimulatedBudflowService>();
        services.AddSingleton<IDiagnosisService, SimulatedAiDiagnosisService>();

        // Perfil
        services.AddSingleton<IProfileService, SimulatedProfileService>();
    }
#endif

    // ---------------------------------------------------------------------
    // MODO PADRÃO (FakeServices isolados, comportamento original)
    // ---------------------------------------------------------------------

    private static void RegisterFakeServices(IServiceCollection services)
    {
        // Auth
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<ILoginService, LoginService>();
        services.AddSingleton<IPasswordRecoveryService, PasswordRecoveryService>();
        services.AddSingleton<IRegisterService, RegisterService>();

        // Plantas
        services.AddSingleton<IDashboardService, DashboardService>();
        services.AddSingleton<IPlantsListService, PlantsListService>();
        services.AddSingleton<IPlantsCatalogService, PlantsCatalogService>();
        services.AddSingleton<IPlantsRegistrationService, PlantsRegistrationService>();

        services.AddSingleton<IPlantDetailsService, PlantDetailsService>();
        services.AddSingleton<IPlantsEditService, PlantsEditService>();
        services.AddSingleton<IPlantHistoryService, PlantHistoryService>();

        // Notificações
        services.AddSingleton<INotificationsService, NotificationsService>();

        // Perfil
        services.AddSingleton<IProfileService, ProfileService>();

        // Conteúdo e IA
        services.AddSingleton<IPlantTipsService, PlantTipsService>();
        services.AddSingleton<IDiagnosisService, DiagnosisService>();

        // Dispositivos
        services.AddSingleton<IDeviceVerificationService, DeviceVerificationService>();
    }

    // =====================================================================
    // PÁGINAS (independentes do modo de simulação)
    // =====================================================================

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
