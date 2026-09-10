/*
 * Responsabilidade:
 * Contrato para obtenção de dicas de cultivo e cuidados baseados na espécie da planta.
 */

using KaaDebug.Core.Models.Plants;

namespace KaaDebug.Core.Interfaces.Plants
{
    public class PlantCareInfoResult
    {
        public bool Success { get; init; }
        public PlantCareInfo? CareInfo { get; init; }
        public string? ErrorMessage { get; init; }

        public static PlantCareInfoResult Ok(PlantCareInfo info) => new() { Success = true, CareInfo = info };
        public static PlantCareInfoResult Fail(string message) => new() { Success = false, ErrorMessage = message };
    }
    public interface IPlantTipsService
    {
        Task<PlantCareInfoResult> GetCareInfoAsync(string plantId);
    }
}
