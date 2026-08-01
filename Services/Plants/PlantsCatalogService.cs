using KaaDebug.Core.Interfaces.Plants;
using KaaDebug.Core.Models.Dashboard;
using KaaDebug.Core.Models.Plants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Services.Plants
{
    /// <summary>
    /// Implementação temporária de ISpeciesService, para desenvolvimento e
    /// testes da tela enquanto o endpoint GET /species não existe.
    ///
    /// SUBSTITUIR pela implementação real (HttpClient) quando o endpoint
    /// estiver disponível.
    /// </summary>
    ///  ISpeciesService
    public class PlantsCatalogService : IPlantsCatalogService
    {
        public async Task<SpeciesCatalogListResult> GetAllSpeciesAsync()
        {
            await Task.Delay(700);

            var species = new List<PlantSpecies> //especies cadastradas no sistema 
        {
            new()
            {
                Id = "s1",
                Name = "Samambaia",
                PhotoUrl = "samambaia.jpg",
                IdealParameters = new SpeciesIdealParameters
                {
                    SoilMoisture = new IdealRange { Min = 60, Max = 80, Unit = "%" },
                    AirHumidity = new IdealRange { Min = 50, Max = 70, Unit = "%" },
                    Temperature = new IdealRange { Min = 18, Max = 26, Unit = "°C" },
                    Luminosity = new IdealRange { Min = 200, Max = 800, Unit = "lux" }
                }
            },
            new()
            {
                Id = "s2",
                Name = "Suculenta",
                PhotoUrl = "suculenta.jpg",
                IdealParameters = new SpeciesIdealParameters
                {
                    SoilMoisture = new IdealRange { Min = 10, Max = 30, Unit = "%" },
                    AirHumidity = new IdealRange { Min = 20, Max = 40, Unit = "%" },
                    Temperature = new IdealRange { Min = 18, Max = 30, Unit = "°C" },
                    Luminosity = new IdealRange { Min = 800, Max = 2000, Unit = "lux" }
                }
            },
            new()
            {
                Id = "s3",
                Name = "Orquídea",
                PhotoUrl = "orquidea.jpg",
                IdealParameters = new SpeciesIdealParameters
                {
                    SoilMoisture = new IdealRange { Min = 40, Max = 60, Unit = "%" },
                    AirHumidity = new IdealRange { Min = 60, Max = 80, Unit = "%" },
                    Temperature = new IdealRange { Min = 18, Max = 24, Unit = "°C" },
                    Luminosity = new IdealRange { Min = 150, Max = 500, Unit = "lux" }
                }
            },
            new()
            {
                Id = "s4",
                Name = "Jiboia",
                PhotoUrl = "jiboia.png",
                IdealParameters = new SpeciesIdealParameters
                {
                    SoilMoisture = new IdealRange { Min = 35, Max = 55, Unit = "%" },
                    AirHumidity = new IdealRange { Min = 40, Max = 60, Unit = "%" },
                    Temperature = new IdealRange { Min = 18, Max = 28, Unit = "°C" },
                    Luminosity = new IdealRange { Min = 150, Max = 600, Unit = "lux" }
                }
            },
            new()
            {
                Id = "s5",
                Name = "Espada-de-São-Jorge",
                PhotoUrl = "espada_sj.png",
                IdealParameters = new SpeciesIdealParameters
                {
                    SoilMoisture = new IdealRange { Min = 20, Max = 40, Unit = "%" },
                    AirHumidity = new IdealRange { Min = 30, Max = 50, Unit = "%" },
                    Temperature = new IdealRange { Min = 16, Max = 30, Unit = "°C" },
                    Luminosity = new IdealRange { Min = 100, Max = 1000, Unit = "lux" }
                }
            }
        };

            return SpeciesCatalogListResult.Ok(species);
        }
    }

    // Esqueleto da implementação real, para referência futura:
    //
    // public class SpeciesService : ISpeciesService
    // {
    //     private readonly HttpClient _httpClient;
    //
    //     public SpeciesService(HttpClient httpClient) => _httpClient = httpClient;
    //
    //     public async Task<SpeciesListResult> GetAllSpeciesAsync()
    //     {
    //         try
    //         {
    //             var response = await _httpClient.GetAsync("species");
    //
    //             if (!response.IsSuccessStatusCode)
    //                 return SpeciesListResult.Fail("Não foi possível carregar as espécies.");
    //
    //             var species = await response.Content.ReadFromJsonAsync<List<PlantSpecies>>();
    //             return SpeciesListResult.Ok(species ?? new List<PlantSpecies>());
    //         }
    //         catch (HttpRequestException)
    //         {
    //             return SpeciesListResult.Fail("Sem conexão com a internet.");
    //         }
    //     }
    // }
}
