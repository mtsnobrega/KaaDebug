using System;
using KaaDebug.Infrastructure.http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Core.Interfaces.Devices
{
    // ── Interface ─────────────────────────────────────────────────────────────────

    public class DeviceAssociationResult
    {
        public bool Success { get; init; }
        public string? ErrorMessage { get; init; }

        public static DeviceAssociationResult Ok() => new() { Success = true };
        public static DeviceAssociationResult Fail(string message) =>
            new() { Success = false, ErrorMessage = message };
    }

    /// <summary>
    /// Abstração exclusiva para associar/desassociar um dispositivo ESP32 a uma planta.
    /// Separada do IPlantsEditService para que a RegisterDevicePage não precise
    /// enviar o nome da planta — operação que não tem relação com dados cadastrais.
    /// </summary>
    public interface IDeviceAssociationService
    {
        /// <summary>
        /// Associa um ESP32 à planta informada.
        /// deviceCode ""           → desassocia o dispositivo atual
        /// deviceCode "ESP32-XXXX" → associa/substitui
        /// </summary>
        Task<DeviceAssociationResult> AssociateAsync(string plantId, string deviceCode);
    }
}
