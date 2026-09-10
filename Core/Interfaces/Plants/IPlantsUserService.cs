using KaaDebug.Core.Models.Dashboard;

namespace KaaDebug.Core.Interfaces.Plants
{
    /// <summary>
    /// Resultado da busca da lista completa de plantas do usuário.
    /// </summary>
    public class PlantsUserListResult
    {
        public bool Success { get; init; }
        public List<PlantSummary>? Plants { get; init; }
        public string? ErrorMessage { get; init; }

        public static PlantsUserListResult Ok(List<PlantSummary> plants) => new() { Success = true, Plants = plants };
        public static PlantsUserListResult Fail(string message) => new() { Success = false, ErrorMessage = message };
    }
    public interface IPlantsUserService
    {
        Task<PlantsUserListResult> GetAllPlantsAsync();
    }
}
