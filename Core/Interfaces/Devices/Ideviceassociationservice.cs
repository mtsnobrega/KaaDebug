/*
 * Responsabilidade:
 * Definir o contrato exclusivo para a associação e desassociação de um 
 * dispositivo físico (ESP32) a uma planta específica.
 * 
 * Papel na arquitetura:
 * Demonstra a aplicação do Princípio da Segregação de Interfaces (ISP do SOLID). 
 * Foi separado do IPlantsEditService para que telas focadas apenas no hardware 
 * não precisem conhecer ou transitar dados botânicos (como nome ou espécie).
 */

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
