using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Core.Models.Dashboard
{
    /// <summary>
    /// Nível de saúde da planta, calculado pelo backend com base nos
    /// parâmetros ideais da espécie x leituras recentes dos sensores.
    /// </summary>
    public enum PlantHealthStatus
    {
        Healthy,    // dentro dos parâmetros ideais
        Attention,  // próximo dos limites (ex: solo começando a secar)
        Critical    // fora dos parâmetros ideais (ex: sem água há muito tempo)
    }

    /// <summary>
    /// Representação resumida de uma planta, usada em listagens (Dashboard,
    /// Lista de Plantas). Não contém o histórico completo de sensores -
    /// isso fica na tela de Detalhes.
    /// </summary>
    public class PlantSummary
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Species { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
        public PlantHealthStatus HealthStatus { get; set; }

        /// <summary>
        /// Texto curto explicando o motivo do status, quando não saudável.
        /// Ex: "Solo seco há 2 dias". Nulo quando HealthStatus = Healthy.
        /// </summary>
        public string? StatusReason { get; set; }
    }

    public enum NotificationPriority
    {
        Low,
        Medium,
        High
    }

    public class NotificationSummary
    {
        public string Id { get; set; } = string.Empty;
        public string PlantId { get; set; } = string.Empty;
        public string PlantName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationPriority Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }

    /// <summary>
    /// Payload agregado retornado pelo endpoint do Dashboard.
    /// Agregamos no backend (em vez de fazer 3 chamadas separadas no app)
    /// para reduzir round-trips em uma tela que é acessada com alta frequência.
    /// </summary>
    public class DashboardData
    {
        public string UserFirstName { get; set; } = string.Empty;
        public int TotalPlants { get; set; }
        public int ActiveAlertsCount { get; set; }
        public List<PlantSummary> RecentPlants { get; set; } = new();
        public List<NotificationSummary> RecentNotifications { get; set; } = new();
    }
}
