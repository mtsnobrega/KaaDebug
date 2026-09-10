using KaaDebug.Core.Interfaces.Plants;
using KaaDebug.Infrastructure.http;

namespace KaaDebug.Services.Plants;
public class PlantsRegistrationService : IPlantsRegistrationService
{
    private readonly ApiClient _apiClient;

    public PlantsRegistrationService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<CreatePlantResult> CreatePlantAsync(CreatePlantRequest request)
    {
        var result = await _apiClient.PostAsync<PlantSummaryDto>(
            ApiConstants.Plants.Create,
            new
            {
                name = request.Name,
                speciesId = Guid.Parse(request.SpeciesId),
                deviceCode = request.DeviceCode
            });

        if (!result.Success)
            return CreatePlantResult.Fail(result.ErrorMessage!);

        return CreatePlantResult.Ok(result.Data!.Id.ToString());
    }
}