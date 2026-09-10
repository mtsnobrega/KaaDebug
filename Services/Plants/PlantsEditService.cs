/*
 * Responsabilidade:
 * Executar as operações de escrita (POST/PUT/DELETE) no domínio de plantas.
 * 
 * Papel na arquitetura:
 * Transforma os Requests internos (ex: EditPlantRequest) em objetos anônimos 
 * para serialização JSON via ApiClient.
 */

using KaaDebug.Core.Interfaces.Plants;
using KaaDebug.Infrastructure.http;

namespace KaaDebug.Services.Plants;
public class PlantsEditService : IPlantsEditService
{
    private readonly ApiClient _apiClient;

    public PlantsEditService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<EditPlantResult> UpdatePlantAsync(EditPlantRequest request)
    {
        var result = await _apiClient.PutAsync(
            ApiConstants.Plants.Update(Guid.Parse(request.PlantId)),
            new
            {
                name = request.Name,
                deviceCode = request.DeviceCode
            });

        return result.Success
            ? EditPlantResult.Ok()
            : EditPlantResult.Fail(result.ErrorMessage!);
    }
    public async Task<EditPlantResult> DeletePlantAsync(string plantId)
    {
        var result = await _apiClient.DeleteAsync(
            ApiConstants.Plants.Delete(Guid.Parse(plantId)));

        return result.Success
            ? EditPlantResult.Ok()
            : EditPlantResult.Fail(result.ErrorMessage!);
    }
}