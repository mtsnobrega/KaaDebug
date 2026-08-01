using KaaDebug.Core.Interfaces.Plants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Services.Plants
{
    /// <summary>
    /// Implementação temporária de IPlantRegistrationService, para
    /// desenvolvimento e testes da tela enquanto o endpoint POST /plants
    /// não existe.
    ///
    /// Código de dispositivo de teste válido: "ESP32-0001"
    /// Qualquer outro código informado retorna erro de validação.
    ///
    /// SUBSTITUIR pela implementação real (HttpClient) quando o endpoint
    /// estiver disponível.
    /// </summary>
    public class PlantsRegistrationService : IPlantsRegistrationService
    {
        private const string ValidDeviceCode = "ESP32-0001";

        public async Task<CreatePlantResult> CreatePlantAsync(CreatePlantRequest request)
        {
            await Task.Delay(1200);

            if (!string.IsNullOrWhiteSpace(request.DeviceCode)
                && !request.DeviceCode.Equals(ValidDeviceCode, StringComparison.OrdinalIgnoreCase))
            {
                return CreatePlantResult.Fail("Código de dispositivo inválido ou não encontrado.");
            }

            // Simula o id gerado pelo backend
            var newPlantId = $"p_{Guid.NewGuid():N}"[..8];
            return CreatePlantResult.Ok(newPlantId);
        }
    }

    // Esqueleto da implementação real, para referência futura:
    //
    // public class PlantRegistrationService : IPlantRegistrationService
    // {
    //     private readonly HttpClient _httpClient;
    //
    //     public PlantRegistrationService(HttpClient httpClient) => _httpClient = httpClient;
    //
    //     public async Task<CreatePlantResult> CreatePlantAsync(CreatePlantRequest request)
    //     {
    //         try
    //         {
    //             var response = await _httpClient.PostAsJsonAsync("plants", new
    //             {
    //                 name = request.Name,
    //                 speciesId = request.SpeciesId,
    //                 deviceCode = request.DeviceCode
    //             });
    //
    //             if (!response.IsSuccessStatusCode)
    //             {
    //                 if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
    //                     return CreatePlantResult.Fail("Código de dispositivo inválido ou não encontrado.");
    //
    //                 return CreatePlantResult.Fail("Não foi possível cadastrar a planta. Tente novamente.");
    //             }
    //
    //             var data = await response.Content.ReadFromJsonAsync<CreatePlantResponseDto>();
    //             return CreatePlantResult.Ok(data!.PlantId);
    //         }
    //         catch (HttpRequestException)
    //         {
    //             return CreatePlantResult.Fail("Sem conexão com a internet.");
    //         }
    //     }
    // }
    //
    // public class CreatePlantResponseDto
    // {
    //     public string PlantId { get; set; } = string.Empty;
    // }
}
