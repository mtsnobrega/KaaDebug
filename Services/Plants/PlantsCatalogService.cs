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
public class PlantsCatalogService : IPlantsCatalogService
{
    private readonly ApiClient _apiClient;

    public PlantsCatalogService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<SpeciesCatalogListResult> GetAllSpeciesAsync()
    {
        var result = await _apiClient.GetAsync<List<SpeciesDto>>(ApiConstants.Species.GetAll);

        if (!result.Success)
            return SpeciesCatalogListResult.Fail(result.ErrorMessage!);

        var species = result.Data!.Select(MapSpecies).ToList();
        return SpeciesCatalogListResult.Ok(species);
    }

    private static PlantSpecies MapSpecies(SpeciesDto dto) => new()
    {
        Id = dto.Id.ToString(),
        Name = dto.Name,
        PhotoUrl = dto.PhotoUrl,
        IdealParameters = new SpeciesIdealParameters
        {
            SoilMoisture = new IdealRange { Min = (double)dto.SoilMoisture.Min, Max = (double)dto.SoilMoisture.Max, Unit = dto.SoilMoisture.Unit },
            AirHumidity = new IdealRange { Min = (double)dto.AirHumidity.Min, Max = (double)dto.AirHumidity.Max, Unit = dto.AirHumidity.Unit },
            Temperature = new IdealRange { Min = (double)dto.Temperature.Min, Max = (double)dto.Temperature.Max, Unit = dto.Temperature.Unit },
            Luminosity = new IdealRange { Min = (double)dto.Luminosity.Min, Max = (double)dto.Luminosity.Max, Unit = dto.Luminosity.Unit }
        }
    };
}