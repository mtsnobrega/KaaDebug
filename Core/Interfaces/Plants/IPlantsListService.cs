using KaaDebug.Core.Models.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    /// <summary>
    /// Abstração para operações sobre o conjunto completo de plantas do usuário.
    /// A implementação real dependerá de um endpoint futuro, por exemplo:
    ///   GET /plants
    /// Diferente do IDashboardService (que traz um resumo agregado e limitado),
    /// este serviço é responsável por trazer TODAS as plantas cadastradas,
    /// usado na tela de Lista de Plantas.
    /// </summary>
    public interface IPlantsListService
    {
        Task<PlantsListResult> GetAllPlantsAsync();
    }
}
