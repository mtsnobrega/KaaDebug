using KaaDebug.Core.Interfaces.Plants;
using KaaDebug.Core.Models.Dashboard;
using KaaDebug.Infrastructure.http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Services.Plants;
public class DashboardService : IDashboardService
{
    private readonly ApiClient _apiClient;

    public DashboardService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<DashboardResult> GetDashboardAsync()
    {
        var result = await _apiClient.GetAsync<DashboardDto>(ApiConstants.Dashboard.Get);

        if (!result.Success)
            return DashboardResult.Fail(result.ErrorMessage!);

        var dto = result.Data!;

        var data = new DashboardData
        {
            UserFirstName = dto.UserFirstName,
            TotalPlants = dto.TotalPlants,
            ActiveAlertsCount = dto.ActiveAlertsCount,
            RecentPlants = dto.RecentPlants.Select(MapPlantSummary).ToList(),
            RecentNotifications = dto.RecentNotifications.Select(MapNotification).ToList()
        };

        return DashboardResult.Ok(data);
    }

    private static PlantSummary MapPlantSummary(PlantSummaryDto dto) => new()
    {
        Id = dto.Id.ToString(),
        Name = dto.Name,
        Species = dto.Species,
        PhotoUrl = dto.PhotoUrl,
        HealthStatus = ParseHealthStatus(dto.HealthStatus),
        StatusReason = dto.StatusReason
    };

    private static NotificationSummary MapNotification(NotificationDto dto) => new()
    {
        Id = dto.Id.ToString(),
        PlantId = dto.PlantId.ToString(),
        PlantName = dto.PlantName,
        Message = dto.Message,
        Priority = ParsePriority(dto.Priority),
        IsRead = dto.IsRead,
        CreatedAt = dto.CreatedAt
    };

    private static PlantHealthStatus ParseHealthStatus(string value) =>
        value.ToUpper() switch
        {
            "WARNING" => PlantHealthStatus.Attention,
            "CRITICAL" => PlantHealthStatus.Critical,
            _ => PlantHealthStatus.Healthy
        };

    private static NotificationPriority ParsePriority(string value) =>
        value.ToUpper() switch
        {
            "HIGH" => NotificationPriority.High,
            "MEDIUM" => NotificationPriority.Medium,
            _ => NotificationPriority.Low
        };
}

/*
{
    /// <summary>
    /// Implementação temporária de IDashboardService, para desenvolvimento e
    /// testes da tela enquanto o endpoint agregado não existe.
    ///
    /// Por padrão devolve um cenário com 3 plantas em estados diferentes
    /// (saudável, atenção, crítico) e 2 notificações, para validar visualmente
    /// todos os estados do Dashboard. Para testar o estado vazio (usuário sem
    /// plantas), troque o construtor para simulateEmptyState: true.
    ///
    /// SUBSTITUIR pela implementação real (HttpClient -> GET /dashboard)
    /// quando o endpoint estiver disponível.
    /// </summary>
    public class DashboardService : IDashboardService
    {
        private readonly bool _simulateEmptyState;
        private readonly bool _simulateError;

        public DashboardService(bool simulateEmptyState = false, bool simulateError = false)
        {
            _simulateEmptyState = simulateEmptyState;
            _simulateError = simulateError;
        }

        public async Task<DashboardResult> GetDashboardAsync()
        {
            await Task.Delay(900);

            if (_simulateError)
                return DashboardResult.Fail("Não foi possível carregar seus dados. Verifique sua conexão.");

            if (_simulateEmptyState)
            {
                return DashboardResult.Ok(new DashboardData
                {
                    UserFirstName = "Maria",
                    TotalPlants = 0,
                    ActiveAlertsCount = 0,
                    RecentPlants = new List<PlantSummary>(),
                    RecentNotifications = new List<NotificationSummary>()
                });
            }

            var data = new DashboardData
            {
                UserFirstName = "Maria",
                TotalPlants = 5,
                ActiveAlertsCount = 2,
                RecentPlants = new List<PlantSummary>
            {
                new()
                {
                    Id = "p1",
                    Name = "Samambaia da Sala",
                    Species = "Samambaia",
                    PhotoUrl = "samambaia.jpg",
                    HealthStatus = PlantHealthStatus.Healthy
                },
                new()
                {
                    Id = "p2",
                    Name = "Suculenta da Janela",
                    Species = "Suculenta",
                    PhotoUrl = "suculenta.jpg",
                    HealthStatus = PlantHealthStatus.Attention,
                    StatusReason = "Solo seco há 2 dias"
                },
                new()
                {
                    Id = "p3",
                    Name = "Orquídea da Varanda",
                    Species = "Orquídea",
                    PhotoUrl = "orquidea.jpg",
                    HealthStatus = PlantHealthStatus.Critical,
                    StatusReason = "Sem rega há 6 dias"
                }
            },
                RecentNotifications = new List<NotificationSummary>
            {
                new()
                {
                    Id = "n1",
                    PlantId = "p3",
                    PlantName = "Orquídea da Varanda",
                    Message = "Umidade do solo crítica",
                    Priority = NotificationPriority.High,
                    CreatedAt = DateTime.Now.AddHours(-1),
                    IsRead = false
                },
                new()
                {
                    Id = "n2",
                    PlantId = "p2",
                    PlantName = "Suculenta da Janela",
                    Message = "Solo começando a secar",
                    Priority = NotificationPriority.Medium,
                    CreatedAt = DateTime.Now.AddHours(-5),
                    IsRead = false
                }
            }
            };

            return DashboardResult.Ok(data);
        }
    }

    // Esqueleto da implementação real, para referência futura:
    //
    // public class DashboardService : IDashboardService
    // {
    //     private readonly HttpClient _httpClient;
    //
    //     public DashboardService(HttpClient httpClient) => _httpClient = httpClient;
    //
    //     public async Task<DashboardResult> GetDashboardAsync()
    //     {
    //         try
    //         {
    //             var response = await _httpClient.GetAsync("dashboard");
    //
    //             if (!response.IsSuccessStatusCode)
    //                 return DashboardResult.Fail("Não foi possível carregar seus dados.");
    //
    //             var data = await response.Content.ReadFromJsonAsync<DashboardData>();
    //             return DashboardResult.Ok(data!);
    //         }
    //         catch (HttpRequestException)
    //         {
    //             return DashboardResult.Fail("Sem conexão com a internet.");
    //         }
    //     }
    // }
}
*/