using KaaDebug.Core.Models.Dashboard;
using KaaDebug.Core.Models.Diagnostic;
using KaaDebug.Core.Models.Plants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Simulation
{
    /// <summary>
    /// Banco de dados em memória compartilhado por todos os serviços simulados.
    /// Representa o estado completo do sistema para um usuário de teste,
    /// desde o cadastro até o monitoramento das plantas.
    ///
    /// É um singleton: todos os SimulatedXxxService leem e escrevem aqui,
    /// garantindo consistência entre telas (ex: planta cadastrada aparece
    /// na lista, notificações refletem o estado dos sensores, etc).
    ///
    /// NÃO mexe em nenhum FakeService já existente.
    /// </summary>
    public class InMemoryDataStore
    {
        // ===================== USUÁRIO =====================
        public SimulatedUser? CurrentUser { get; set; }

        // ===================== PLANTAS =====================
        public List<SimulatedPlant> Plants { get; set; } = new();

        // ===================== DISPOSITIVOS =====================
        public List<SimulatedDevice> Devices { get; set; } = new();

        // ===================== ESPÉCIES =====================
        public List<PlantSpecies> Species { get; set; } = new();

        // ===================== LEITURAS DOS SENSORES =====================
        // Chave: PlantId
        public Dictionary<string, List<SimulatedSensorReading>> SensorReadings { get; set; } = new();

        // ===================== NOTIFICAÇÕES =====================
        public List<SimulatedNotification> Notifications { get; set; } = new();

        // ===================== DIAGNÓSTICOS =====================
        // Chave: PlantId
        public Dictionary<string, List<DiagnosisResult>> DiagnosisHistory { get; set; } = new();

        // ===================== SESSÃO =====================
        public bool IsAuthenticated { get; set; }
        public string? SessionToken { get; set; }
    }

    // ===================== MODELOS INTERNOS DO STORE =====================

    public class SimulatedUser
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool NotificationsEnabled { get; set; } = true;
        public bool CriticalAlertsOnly { get; set; }
    }

    public class SimulatedPlant
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string SpeciesId { get; set; } = string.Empty;
        public string SpeciesName { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
        public string? DeviceId { get; set; }
        public PlantHealthStatus HealthStatus { get; set; }
        public string? StatusReason { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class SimulatedDevice
    {
        public string Id { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? AssociatedPlantId { get; set; }
        public DeviceConnectionStatus ConnectionStatus { get; set; }
        public DateTime? LastHeartbeatAt { get; set; }
    }

    public class SimulatedSensorReading
    {
        public string PlantId { get; set; } = string.Empty;
        public SensorType SensorType { get; set; }
        public double Value { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class SimulatedNotification
    {
        public string Id { get; set; } = string.Empty;
        public string PlantId { get; set; } = string.Empty;
        public string PlantName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationPriority Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }
}
