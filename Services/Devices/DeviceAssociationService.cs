/*
 * Responsabilidade:
 * Implementar a requisição HTTP (PUT) para associar o dispositivo à planta.
 * 
 * Papel na arquitetura:
 * Camada de Serviço (Service Layer). Encapsula o uso do ApiClient e 
 * converte a resposta da rede em um DeviceAssociationResult uniforme.
 */
using KaaDebug.Core.Interfaces.Devices;
using KaaDebug.Infrastructure.http;

namespace KaaDebug.Services.Devices
{
    public class DeviceAssociationService : IDeviceAssociationService
    {
        private readonly ApiClient _apiClient;

        public DeviceAssociationService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<DeviceAssociationResult> AssociateAsync(string plantId, string deviceCode)
        {
            var result = await _apiClient.PutAsync(
                ApiConstants.Plants.AssociateDevice(Guid.Parse(plantId)),
                new { deviceCode });

            return result.Success
                ? DeviceAssociationResult.Ok()
                : DeviceAssociationResult.Fail(result.ErrorMessage!);
        }
    }
}
