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
using KaaDebug.Core.Models.Plants;
using KaaDebug.Infrastructure.http;

namespace KaaDebug.Services.Plants;
public class PlantHistoryService : IPlantHistoryService
{
    private readonly ApiClient _apiClient;

    public PlantHistoryService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<PlantHistoryResult> GetHistoryAsync(string plantId, HistoryPeriod period)
    {
        var periodParam = period switch
        {
            HistoryPeriod.Last7Days => "7d",
            HistoryPeriod.Last30Days => "30d",
            _ => "24h"
        };

        var result = await _apiClient.GetAsync<PlantHistoryDto>(
            ApiConstants.Plants.GetHistory(Guid.Parse(plantId), periodParam));

        if (!result.Success)
            return PlantHistoryResult.Fail(result.ErrorMessage!);

        var dto = result.Data!;
        var data = new PlantHistoryData
        {
            PlantName = dto.PlantName,
            Period = period,
            Sensors = dto.Sensors.Select(s => new SensorHistory
            {
                Type = ParseSensorType(s.SensorType),
                Unit = s.Unit,
                IdealRange = new IdealRange
                {
                    Min = (double)s.IdealRange.Min,
                    Max = (double)s.IdealRange.Max,
                    Unit = s.IdealRange.Unit
                },
                Readings = s.Readings.Select(r =>
                    new SensorReadingPoint { Timestamp = r.Timestamp, Value = r.Value }).ToList()
            }).ToList()
        };

        return PlantHistoryResult.Ok(data);
    }

    private static SensorType ParseSensorType(string value) =>
        value.ToUpper() switch
        {
            "AIR_HUMIDITY" => SensorType.AirHumidity,
            "TEMPERATURE" => SensorType.Temperature,
            "LUMINOSITY" => SensorType.Luminosity,
            _ => SensorType.SoilMoisture
        };
}