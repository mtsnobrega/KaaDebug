using KaaDebug.Core.Interfaces.Plants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Services.Plants
{
    /// <summary>
    /// Implementação temporária de IPlantEditService, para desenvolvimento
    /// e testes da tela enquanto os endpoints PUT/DELETE /plants/{id}
    /// não existem.
    ///
    /// Código de dispositivo inválido de teste: "ESP32-XXXX" → retorna erro.
    /// Qualquer outro código (ou nulo/vazio) é aceito.
    ///
    /// SUBSTITUIR pela implementação real (HttpClient) quando os endpoints
    /// estiverem disponíveis.
    /// </summary>
    public class PlantsEditService : IPlantsEditService
    {
        private const string InvalidDeviceCode = "ESP32-XXXX";

        public async Task<EditPlantResult> UpdatePlantAsync(EditPlantRequest request)
        {
            await Task.Delay(1000);

            if (!string.IsNullOrEmpty(request.DeviceCode)
                && request.DeviceCode.Equals(InvalidDeviceCode, StringComparison.OrdinalIgnoreCase))
            {
                return EditPlantResult.Fail("Código de dispositivo inválido ou não encontrado.");
            }

            return EditPlantResult.Ok();
        }

        public async Task<EditPlantResult> DeletePlantAsync(string plantId)
        {
            await Task.Delay(800);
            return EditPlantResult.Ok();
        }
    }

    // Esqueleto da implementação real, para referência futura:
    //
    // public class PlantEditService : IPlantEditService
    // {
    //     private readonly HttpClient _httpClient;
    //
    //     public PlantEditService(HttpClient httpClient) => _httpClient = httpClient;
    //
    //     public async Task<EditPlantResult> UpdatePlantAsync(EditPlantRequest request)
    //     {
    //         try
    //         {
    //             var response = await _httpClient.PutAsJsonAsync($"plants/{request.PlantId}", new
    //             {
    //                 name = request.Name,
    //                 deviceCode = request.DeviceCode
    //             });
    //
    //             if (!response.IsSuccessStatusCode)
    //             {
    //                 if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
    //                     return EditPlantResult.Fail("Código de dispositivo inválido ou não encontrado.");
    //
    //                 return EditPlantResult.Fail("Não foi possível salvar as alterações. Tente novamente.");
    //             }
    //
    //             return EditPlantResult.Ok();
    //         }
    //         catch (HttpRequestException)
    //         {
    //             return EditPlantResult.Fail("Sem conexão com a internet.");
    //         }
    //     }
    //
    //     public async Task<EditPlantResult> DeletePlantAsync(string plantId)
    //     {
    //         try
    //         {
    //             var response = await _httpClient.DeleteAsync($"plants/{plantId}");
    //
    //             return response.IsSuccessStatusCode
    //                 ? EditPlantResult.Ok()
    //                 : EditPlantResult.Fail("Não foi possível excluir a planta. Tente novamente.");
    //         }
    //         catch (HttpRequestException)
    //         {
    //             return EditPlantResult.Fail("Sem conexão com a internet.");
    //         }
    //     }
    // }
}
