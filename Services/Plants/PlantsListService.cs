using KaaDebug.Core.Interfaces.Plants;
using KaaDebug.Core.Models.Dashboard;
using KaaDebug.Infrastructure.http;

namespace KaaDebug.Services.Plants;

public class PlantsListService : IPlantsListService
{
    private readonly ApiClient _apiClient;

    public PlantsListService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<PlantsListResult> GetAllPlantsAsync()
    {
        var result = await _apiClient.GetAsync<List<PlantSummaryDto>>(
            ApiConstants.Plants.GetAll);

        if (!result.Success)
            return PlantsListResult.Fail(result.ErrorMessage!);

        var plants = result.Data!.Select(MapSummary).ToList();
        return PlantsListResult.Ok(plants);
    }

    private static PlantSummary MapSummary(PlantSummaryDto dto) => new()
    {
        Id = dto.Id.ToString(),
        Name = dto.Name,
        Species = dto.Species,
        PhotoUrl = dto.PhotoUrl,
        HealthStatus = ParseHealthStatus(dto.HealthStatus),
        StatusReason = dto.StatusReason
    };

    private static PlantHealthStatus ParseHealthStatus(string value) =>
        value.ToUpper() switch
        {
            "WARNING" => PlantHealthStatus.Attention,
            "CRITICAL" => PlantHealthStatus.Critical,
            _ => PlantHealthStatus.Healthy
        };
}