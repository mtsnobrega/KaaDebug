/*
 * Responsabilidade:
 * Contrato para agregação dos detalhes de uma planta (cadastros, sensores, status e notificações).
 * 
 * Papel na arquitetura:
 * Age como o principal ponto de consulta para a tela operacional da planta.
 */
using KaaDebug.Core.Models.Plants;

namespace KaaDebug.Core.Interfaces.Plants
{
    public class PlantDetailsResult
    {
        public bool Success { get; init; }
        public PlantDetails? Details { get; init; }
        public string? ErrorMessage { get; init; }

        public static PlantDetailsResult Ok(PlantDetails details) => new() { Success = true, Details = details };
        public static PlantDetailsResult Fail(string message) => new() { Success = false, ErrorMessage = message };
    }
    public interface IPlantDetailsService
    {
        Task<PlantDetailsResult> GetPlantDetailsAsync(string plantId);
    }
}
