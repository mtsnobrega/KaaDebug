using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Core.Interfaces.Devices
{
    public enum DeviceVerificationStatus
    {
        Checking,
        Online,
        Offline,
        NotFound
    }

    public class DeviceVerificationResult
    {
        public bool Success { get; init; }
        public DeviceVerificationStatus Status { get; init; }
        public string? ErrorMessage { get; init; }

        public static DeviceVerificationResult Ok(DeviceVerificationStatus status) =>
            new() { Success = true, Status = status };

        public static DeviceVerificationResult Fail(string message) =>
            new() { Success = false, ErrorMessage = message, Status = DeviceVerificationStatus.NotFound };
    }

    /// <summary>
    /// Abstração para verificação do status de comunicação de um dispositivo
    /// ESP32 após a associação, confirmando se ele está acessível na rede.
    ///
    /// A implementação real dependerá de um endpoint futuro, por exemplo:
    ///   GET /devices/{code}/status
    /// que consulta o broker MQTT ou o registro de heartbeats do dispositivo.
    /// </summary>
    public interface IDeviceVerificationService
    {
        Task<DeviceVerificationResult> VerifyDeviceAsync(string deviceCode);
    }
}
