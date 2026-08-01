using KaaDebug.Core.Interfaces.Plants;
using KaaDebug.Core.Models.Plants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Services.Plants
{
    /// <summary>
    /// Implementação temporária de IPlantHistoryService.
    /// Gera leituras sintéticas com variação aleatória realista para cada
    /// sensor, cobrindo os 3 períodos (24h, 7d, 30d).
    ///
    /// SUBSTITUIR pela implementação real (HttpClient -> GET /plants/{id}/history)
    /// quando o endpoint estiver disponível.
    /// </summary>
    public class PlantHistoryService : IPlantHistoryService
    {
        private readonly Random _random = new(42); // seed fixa = dados consistentes entre cargas

        public async Task<PlantHistoryResult> GetHistoryAsync(string plantId, HistoryPeriod period)
        {
            await Task.Delay(800);

            var (pointCount, intervalMinutes) = period switch
            {
                HistoryPeriod.Last24Hours => (24, 60),      // 1 ponto por hora
                HistoryPeriod.Last7Days => (28, 360),       // 1 ponto a cada 6h
                HistoryPeriod.Last30Days => (30, 1440),     // 1 ponto por dia
                _ => (24, 60)
            };

            var data = new PlantHistoryData
            {
                PlantName = "Suculenta da Janela",
                Period = period,
                Sensors = new List<SensorHistory>
            {
                BuildSensorHistory(
                    SensorType.SoilMoisture, "%",
                    idealMin: 10, idealMax: 30,
                    baseValue: 18, variance: 6,
                    pointCount, intervalMinutes),

                BuildSensorHistory(
                    SensorType.AirHumidity, "%",
                    idealMin: 20, idealMax: 40,
                    baseValue: 32, variance: 4,
                    pointCount, intervalMinutes),

                BuildSensorHistory(
                    SensorType.Temperature, "°C",
                    idealMin: 18, idealMax: 30,
                    baseValue: 24, variance: 3,
                    pointCount, intervalMinutes),

                BuildSensorHistory(
                    SensorType.Luminosity, "lux",
                    idealMin: 800, idealMax: 2000,
                    baseValue: 1100, variance: 300,
                    pointCount, intervalMinutes)
            }
            };

            return PlantHistoryResult.Ok(data);
        }

        private SensorHistory BuildSensorHistory(
            SensorType type, string unit,
            double idealMin, double idealMax,
            double baseValue, double variance,
            int pointCount, int intervalMinutes)
        {
            var readings = new List<SensorReadingPoint>();
            var now = DateTime.Now;

            for (int i = pointCount - 1; i >= 0; i--)
            {
                var noise = (_random.NextDouble() - 0.5) * 2 * variance;
                var value = Math.Round(Math.Max(0, baseValue + noise), 1);

                readings.Add(new SensorReadingPoint
                {
                    Timestamp = now.AddMinutes(-(i * intervalMinutes)),
                    Value = value
                });
            }

            return new SensorHistory
            {
                Type = type,
                Unit = unit,
                IdealRange = new IdealRange { Min = idealMin, Max = idealMax, Unit = unit },
                Readings = readings
            };
        }
    }

    // Esqueleto da implementação real:
    //
    // public class PlantHistoryService : IPlantHistoryService
    // {
    //     private readonly HttpClient _httpClient;
    //     public PlantHistoryService(HttpClient httpClient) => _httpClient = httpClient;
    //
    //     public async Task<PlantHistoryResult> GetHistoryAsync(string plantId, HistoryPeriod period)
    //     {
    //         try
    //         {
    //             var periodParam = period switch
    //             {
    //                 HistoryPeriod.Last24Hours => "24h",
    //                 HistoryPeriod.Last7Days   => "7d",
    //                 HistoryPeriod.Last30Days  => "30d",
    //                 _ => "24h"
    //             };
    //
    //             var response = await _httpClient.GetAsync($"plants/{plantId}/history?period={periodParam}");
    //
    //             if (!response.IsSuccessStatusCode)
    //                 return PlantHistoryResult.Fail("Não foi possível carregar o histórico.");
    //
    //             var data = await response.Content.ReadFromJsonAsync<PlantHistoryData>();
    //             return PlantHistoryResult.Ok(data!);
    //         }
    //         catch (HttpRequestException)
    //         {
    //             return PlantHistoryResult.Fail("Sem conexão com a internet.");
    //         }
    //     }
    // }
}
