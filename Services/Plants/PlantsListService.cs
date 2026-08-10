using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaaDebug.Core.Interfaces.Plants;
using KaaDebug.Core.Models.Dashboard;
using KaaDebug.Infrastructure.http;

namespace KaaDebug.Services.Plants;
/// <summary>
/// Implementação real de IPlantsListService.
/// Substitui o PlantsListService fake — consome GET /plants da API.
/// </summary>
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
/*

/// <summary>
/// Implementação temporária de IPlantsService, para desenvolvimento e
/// testes da tela enquanto o endpoint GET /plants não existe.
///
/// Devolve 5 plantas com status variados, consistente com o
/// TotalPlants = 5 usado no FakeDashboardService.
///
/// SUBSTITUIR pela implementação real (HttpClient) quando o endpoint
/// estiver disponível.
/// </summary>
public class PlantsListService : IPlantsListService
{
    private readonly bool _simulateEmptyState;
    private readonly bool _simulateError;

    public PlantsListService(bool simulateEmptyState = false, bool simulateError = false)
    {
        _simulateEmptyState = simulateEmptyState;
        _simulateError = simulateError;
    }

    public async Task<PlantsListResult> GetAllPlantsAsync()
    {
        await Task.Delay(800);

        if (_simulateError)
            return PlantsListResult.Fail("Não foi possível carregar suas plantas. Verifique sua conexão.");

        if (_simulateEmptyState)
            return PlantsListResult.Ok(new List<PlantSummary>());

        var plants = new List<PlantSummary>
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
        },
        new()
        {
            Id = "p4",
            Name = "Jiboia da Cozinha",
            Species = "Jiboia",
            PhotoUrl = "jiboia.png",
            HealthStatus = PlantHealthStatus.Healthy
        },
        new()
        {
            Id = "p5",
            Name = "Espada-de-São-Jorge",
            Species = "Espada-de-São-Jorge",
            PhotoUrl = "espada_sj.png",
            HealthStatus = PlantHealthStatus.Healthy
        }
    };

        return PlantsListResult.Ok(plants);
    }
}

// Esqueleto da implementação real, para referência futura:
//
// public class PlantsService : IPlantsService
// {
//     private readonly HttpClient _httpClient;
//
//     public PlantsService(HttpClient httpClient) => _httpClient = httpClient;
//
//     public async Task<PlantsListResult> GetAllPlantsAsync()
//     {
//         try
//         {
//             var response = await _httpClient.GetAsync("plants");
//
//             if (!response.IsSuccessStatusCode)
//                 return PlantsListResult.Fail("Não foi possível carregar suas plantas.");
//
//             var plants = await response.Content.ReadFromJsonAsync<List<PlantSummary>>();
//             return PlantsListResult.Ok(plants ?? new List<PlantSummary>());
//         }
//         catch (HttpRequestException)
//         {
//             return PlantsListResult.Fail("Sem conexão com a internet.");
//         }
//     }
// }
*/