using KaaDebug.Core.Interfaces.Plants;
using KaaDebug.Core.Models.Plants;
using KaaDebug.Infrastructure.http;

namespace KaaDebug.Services.Plants;
public class PlantTipsService : IPlantTipsService
{
    private readonly ApiClient _apiClient;

    public PlantTipsService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<PlantCareInfoResult> GetCareInfoAsync(string plantId)
    {
        var result = await _apiClient.GetAsync<PlantCareInfo>(
            ApiConstants.Plants.GetCareTips(Guid.Parse(plantId)));

        return result.Success
            ? PlantCareInfoResult.Ok(result.Data!)
            : PlantCareInfoResult.Fail(result.ErrorMessage!);
    }
}