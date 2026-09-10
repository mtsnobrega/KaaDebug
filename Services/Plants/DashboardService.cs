/*
 * Responsabilidade:
 * Implementação concreta do serviço. Consome a BFF API e mapeia o resultado.
 * 
 * Papel na arquitetura:
 * Camada de Serviço (Service Layer). Atua como um tradutor (Adapter): recebe 
 * DTOs da rede (ex: PlantSummaryDto) e os converte para Modelos de Domínio 
 * (ex: PlantSummary), aplicando lógicas de parser (como conversão de strings 
 * para os Enums PlantHealthStatus e SensorType).
 * 
 * Dependências:
 * ApiClient (para execução do HTTP).
 */

using KaaDebug.Core.Interfaces.Plants;
using KaaDebug.Core.Models.Dashboard;
using KaaDebug.Infrastructure.http;

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