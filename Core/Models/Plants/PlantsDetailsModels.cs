using KaaDebug.Core.Models.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Core.Models.Plants
{
    public enum SensorType
    {
        SoilMoisture,
        AirHumidity,
        Temperature,
        Luminosity
    }

    /// <summary>
    /// Um ponto de leitura histórica de um sensor, usado para montar o
    /// mini-gráfico de tendência das últimas 24h.
    /// </summary>
    public class SensorReadingPoint
    {
        public DateTime Timestamp { get; set; }
        public double Value { get; set; }
    }

    /// <summary>
    /// Estado atual de um indicador ambiental, incluindo o histórico recente
    /// (para o mini-gráfico) e se o valor está dentro da faixa ideal da espécie.
    /// </summary>
    public class SensorIndicator
    {
        public SensorType Type { get; set; }
        public double CurrentValue { get; set; }
        public string Unit { get; set; } = string.Empty;
        public IdealRange IdealRange { get; set; } = new();

        /// <summary>True quando CurrentValue está dentro de IdealRange.</summary>
        public bool IsWithinIdealRange { get; set; }

        /// <summary>Últimas leituras (24h), usadas no mini-gráfico de tendência.</summary>
        public List<SensorReadingPoint> RecentHistory { get; set; } = new();
    }

    public enum DeviceConnectionStatus
    {
        Online,
        Offline,
        NotAssociated
    }

    public class DeviceInformation
    {
        public string? DeviceCode { get; set; }
        public DeviceConnectionStatus ConnectionStatus { get; set; }
        public DateTime? LastReadingAt { get; set; }
    }

    /// <summary>
    /// Payload completo exibido na tela de Detalhes da Planta - a tela
    /// operacional principal do app. Reúne dados cadastrais, indicadores
    /// ambientais em tempo real (com histórico recente), status do
    /// dispositivo associado, e notificações relacionadas a esta planta.
    /// </summary>
    public class PlantDetails
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Species { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
        public PlantHealthStatus HealthStatus { get; set; }
        public string? StatusReason { get; set; }

        public DeviceInformation Device { get; set; } = new();
        public List<SensorIndicator> Indicators { get; set; } = new();
        public List<NotificationSummary> RelatedNotifications { get; set; } = new();
    }
}
