using KaaDebug.Core.Interfaces.Plants;
using KaaDebug.Core.Models.Dashboard;
using KaaDebug.Core.Models.Plants;
using KaaDebug.Infrastructure.http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Services.Plants;
public class PlantDetailsService : IPlantDetailsService
{
    private readonly ApiClient _apiClient;

    public PlantDetailsService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<PlantDetailsResult> GetPlantDetailsAsync(string plantId)
    {
        var result = await _apiClient.GetAsync<PlantDetailsDto>(
            ApiConstants.Plants.GetDetails(Guid.Parse(plantId)));

        if (!result.Success)
            return PlantDetailsResult.Fail(result.ErrorMessage!);

        var dto = result.Data!;
        var details = new PlantDetails
        {
            Id = dto.Id.ToString(),
            Name = dto.Name,
            Species = dto.Species,
            PhotoUrl = dto.PhotoUrl,
            HealthStatus = PlantsUserService.ParseHealthStatus(dto.HealthStatus),
            StatusReason = dto.StatusReason,
            Device = new DeviceInformation
            {
                DeviceCode = dto.Device.DeviceCode,
                ConnectionStatus = ParseConnectionStatus(dto.Device.ConnectionStatus),
                LastReadingAt = dto.Device.LastReadingAt
            },
            Indicators = dto.Indicators.Select(MapIndicator).ToList(),
            RelatedNotifications = dto.RecentNotifications.Select(MapNotification).ToList()
        };

        return PlantDetailsResult.Ok(details);
    }

    private static SensorIndicator MapIndicator(SensorIndicatorDto dto) => new()
    {
        Type = ParseSensorType(dto.SensorType),
        CurrentValue = dto.CurrentValue,
        Unit = dto.Unit,
        IdealRange = new IdealRange
        {
            Min = (double)dto.IdealRange.Min,
            Max = (double)dto.IdealRange.Max,
            Unit = dto.IdealRange.Unit
        },
        IsWithinIdealRange = dto.IsWithinIdealRange,
        RecentHistory = dto.RecentHistory.Select(r =>
            new SensorReadingPoint { Timestamp = r.Timestamp, Value = r.Value }).ToList()
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

    private static DeviceConnectionStatus ParseConnectionStatus(string value) =>
        value.ToUpper() switch
        {
            "ONLINE" => DeviceConnectionStatus.Online,
            "OFFLINE" => DeviceConnectionStatus.Offline,
            "ASSOCIATED" => DeviceConnectionStatus.Associated,
            _ => DeviceConnectionStatus.NotAssociated
        };

    private static SensorType ParseSensorType(string value) =>
        value.ToUpper() switch
        {
            "AIR_HUMIDITY" => SensorType.AirHumidity,
            "TEMPERATURE" => SensorType.Temperature,
            "LUMINOSITY" => SensorType.Luminosity,
            _ => SensorType.SoilMoisture
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

/// <summary>
/// Implementação temporária de IPlantDetailsService, para desenvolvimento
/// e testes da tela enquanto o endpoint GET /plants/{id} não existe.
///
/// Gera um histórico sintético de 24h (leitura a cada hora) para cada
/// sensor, permitindo validar visualmente os mini-gráficos de tendência.
///
/// SUBSTITUIR pela implementação real (HttpClient) quando o endpoint
/// estiver disponível.
/// </summary>
public class PlantDetailsService : IPlantDetailsService
{
    public async Task<PlantDetailsResult> GetPlantDetailsAsync(string plantId)
    {
        await Task.Delay(900);

        // Cenário fixo de exemplo: planta em estado de "Atenção" (solo abaixo do ideal)
        var details = new PlantDetails
        {
            Id = plantId,
            Name = "Suculenta da Janela",
            Species = "Suculenta",
            PhotoUrl = "suculenta.jpg",
            HealthStatus = PlantHealthStatus.Attention,
            StatusReason = "Solo seco há 2 dias",

            Device = new DeviceInformation
            {
                DeviceCode = "ESP32-0001",
                ConnectionStatus = DeviceConnectionStatus.Offline, // Online, Offline, NotAssociated
                LastReadingAt = DateTime.Now.AddMinutes(-8)
            },

            Indicators = new List<SensorIndicator>
        {
            BuildIndicator(SensorType.SoilMoisture, currentValue: 05, unit: "%",
                idealMin: 10, idealMax: 30, trendStart: 10, trendEnd: 14),

            BuildIndicator(SensorType.AirHumidity, currentValue: 35, unit: "%",
                idealMin: 20, idealMax: 40, trendStart: 32, trendEnd: 35),

            BuildIndicator(SensorType.Temperature, currentValue: 24, unit: "°C",
                idealMin: 18, idealMax: 30, trendStart: 22, trendEnd: 24),

            BuildIndicator(SensorType.Luminosity, currentValue: 950, unit: "lux",
                idealMin: 800, idealMax: 2000, trendStart: 700, trendEnd: 950)
        },

            RelatedNotifications = new List<NotificationSummary>
        {
            new()
            {
                Id = "n2",
                PlantId = plantId,
                PlantName = "Suculenta da Janela",
                Message = "Solo começando a secar",
                Priority = NotificationPriority.Medium,
                CreatedAt = DateTime.Now.AddHours(-5),
                IsRead = false
            },
            new()
            {
                Id = "n3",
                PlantId = plantId,
                PlantName = "Suculenta da Janela",
                Message = "Rega realizada com sucesso",
                Priority = NotificationPriority.Low,
                CreatedAt = DateTime.Now.AddDays(-2),
                IsRead = true
            }
        }
        };

        return PlantDetailsResult.Ok(details);
    }

    /// <summary>
    /// Gera um indicador com histórico sintético de 24h, interpolando
    /// linearmente entre um valor inicial e final (suficiente para
    /// visualizar a tendência no mini-gráfico).
    /// </summary>
    private static SensorIndicator BuildIndicator(
        SensorType type, double currentValue, string unit,
        double idealMin, double idealMax, double trendStart, double trendEnd)
    {
        var history = new List<SensorReadingPoint>();
        var now = DateTime.Now;

        for (int hoursAgo = 23; hoursAgo >= 0; hoursAgo--)
        {
            var progress = (23 - hoursAgo) / 23.0;
            var value = trendStart + (trendEnd - trendStart) * progress;

            history.Add(new SensorReadingPoint
            {
                Timestamp = now.AddHours(-hoursAgo),
                Value = Math.Round(value, 1)
            });
        }

        return new SensorIndicator
        {
            Type = type,
            CurrentValue = currentValue,
            Unit = unit,
            IdealRange = new IdealRange { Min = idealMin, Max = idealMax, Unit = unit },
            IsWithinIdealRange = currentValue >= idealMin && currentValue <= idealMax,
            RecentHistory = history
        };
    }
}

// Esqueleto da implementação real, para referência futura:
//
// public class PlantDetailsService : IPlantDetailsService
// {
//     private readonly HttpClient _httpClient;
//
//     public PlantDetailsService(HttpClient httpClient) => _httpClient = httpClient;
//
//     public async Task<PlantDetailsResult> GetPlantDetailsAsync(string plantId)
//     {
//         try
//         {
//             var response = await _httpClient.GetAsync($"plants/{plantId}");
//
//             if (!response.IsSuccessStatusCode)
//                 return PlantDetailsResult.Fail("Não foi possível carregar os dados da planta.");
//
//             var details = await response.Content.ReadFromJsonAsync<PlantDetails>();
//             return PlantDetailsResult.Ok(details!);
//         }
//         catch (HttpRequestException)
//         {
//             return PlantDetailsResult.Fail("Sem conexão com a internet.");
//         }
//     }
// }
*/