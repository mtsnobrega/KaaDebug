/*
 * Responsabilidade:
 * Definir o contrato para a obtenção dos dados consolidados do Dashboard.
 * 
 * Papel na arquitetura:
 * Contrato de serviço (Core). Utiliza o padrão Result (DashboardResult) para 
 * encapsular retornos da API, abstraindo falhas de rede da camada de apresentação.
 */

using KaaDebug.Core.Models.Dashboard;

namespace KaaDebug.Core.Interfaces.Plants
{
    public class DashboardResult
    {
        public bool Success { get; init; }
        public DashboardData? Data { get; init; }
        public string? ErrorMessage { get; init; }

        public static DashboardResult Ok(DashboardData data) => new() { Success = true, Data = data };
        public static DashboardResult Fail(string message) => new() { Success = false, ErrorMessage = message };
    }
    public interface IDashboardService
    {
        Task<DashboardResult> GetDashboardAsync();
    }
}
