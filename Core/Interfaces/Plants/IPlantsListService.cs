/*
 * Responsabilidade:
 * Contrato para recuperação da lista completa de plantas associadas ao usuário logado.
 * 
 * Papel na arquitetura:
 * Fornece a coleção principal de entidades (PlantSummary) para a listagem principal do app.
 */

using KaaDebug.Core.Models.Dashboard;

namespace KaaDebug.Core.Interfaces.Plants
{
    /// <summary>
    /// Resultado da busca da lista completa de plantas do usuário.
    /// </summary>
    public class PlantsListResult
    {
        public bool Success { get; init; }
        public List<PlantSummary>? Plants { get; init; }
        public string? ErrorMessage { get; init; }

        public static PlantsListResult Ok(List<PlantSummary> plants) => new() { Success = true, Plants = plants };
        public static PlantsListResult Fail(string message) => new() { Success = false, ErrorMessage = message };
    }
    public interface IPlantsListService
    {
        Task<PlantsListResult> GetAllPlantsAsync();
    }
}
