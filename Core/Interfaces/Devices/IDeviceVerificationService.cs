/*
 * Responsabilidade:
 * Definir o contrato para consultar o estado de comunicação (heartbeat/MQTT) 
 * e disponibilidade de um ESP32 antes de permitir sua associação.
 * 
 * Papel na arquitetura:
 * Fornece um enumerador (DeviceVerificationStatus) e um Result Object, 
 * criando uma camada de abstração entre a regra de negócio de IoT e 
 * as respostas JSON da BFF API.
 */

namespace KaaDebug.Core.Interfaces.Devices
{
    public enum DeviceVerificationStatus
    {
        Checking,
        Associated,
        Online,
        Offline,
        Unassociated,
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
    public interface IDeviceVerificationService
    {
        Task<DeviceVerificationResult> VerifyDeviceAsync(string deviceCode);
    }
}
