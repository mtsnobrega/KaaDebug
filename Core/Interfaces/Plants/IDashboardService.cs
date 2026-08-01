using KaaDebug.Core.Models.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Core.Interfaces.Plants
{
    /// <summary>
    /// Resultado da busca de dados do Dashboard. Usamos um result object (em
    /// vez de deixar a página capturar exceções de rede diretamente) para que
    /// a tela trate "erro de carregamento" de forma uniforme com as demais.
    /// </summary>
    public class DashboardResult
    {
        public bool Success { get; init; }
        public DashboardData? Data { get; init; }
        public string? ErrorMessage { get; init; }

        public static DashboardResult Ok(DashboardData data) => new() { Success = true, Data = data };
        public static DashboardResult Fail(string message) => new() { Success = false, ErrorMessage = message };
    }

    /// <summary>
    /// Abstração para obtenção dos dados consolidados do Dashboard.
    /// A implementação real dependerá de um endpoint futuro, por exemplo:
    ///   GET /dashboard
    /// que devolve resumo de plantas + alertas + notificações recentes
    /// em uma única resposta.
    /// </summary>
    public interface IDashboardService
    {
        Task<DashboardResult> GetDashboardAsync();
    }
}
