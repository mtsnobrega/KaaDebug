using KaaDebug.Core.Interfaces.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Services.Devices
{
    /// <summary>
    /// Implementação temporária de IDeviceVerificationService, para
    /// desenvolvimento e testes da tela enquanto o endpoint de status
    /// do dispositivo não existe.
    ///
    /// Regras de teste:
    ///   "ESP32-0001" → Online  (dispositivo ativo)
    ///   "ESP32-0002" → Offline (dispositivo registrado mas sem heartbeat recente)
    ///   qualquer outro → NotFound (código não registrado no sistema)
    ///
    /// SUBSTITUIR pela implementação real (HttpClient -> GET /devices/{code}/status)
    /// quando o endpoint estiver disponível.
    /// </summary>
    public class DeviceVerificationService : IDeviceVerificationService
    {
        public async Task<DeviceVerificationResult> VerifyDeviceAsync(string deviceCode)
        {
            // Simula a latência de consulta ao broker MQTT
            await Task.Delay(2000);

            var status = deviceCode.ToUpperInvariant() switch
            {
                "ESP32-0001" => DeviceVerificationStatus.Online,
                "ESP32-0002" => DeviceVerificationStatus.Offline,
                _ => DeviceVerificationStatus.NotFound
            };

            if (status == DeviceVerificationStatus.NotFound)
                return DeviceVerificationResult.Fail("Dispositivo não encontrado. Verifique o código e tente novamente.");

            return DeviceVerificationResult.Ok(status);
        }
    }

    // Esqueleto da implementação real, para referência futura:
    //
    // public class DeviceVerificationService : IDeviceVerificationService
    // {
    //     private readonly HttpClient _httpClient;
    //
    //     public DeviceVerificationService(HttpClient httpClient) => _httpClient = httpClient;
    //
    //     public async Task<DeviceVerificationResult> VerifyDeviceAsync(string deviceCode)
    //     {
    //         try
    //         {
    //             var response = await _httpClient.GetAsync($"devices/{deviceCode}/status");
    //
    //             if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
    //                 return DeviceVerificationResult.Fail("Dispositivo não encontrado.");
    //
    //             if (!response.IsSuccessStatusCode)
    //                 return DeviceVerificationResult.Fail("Não foi possível verificar o dispositivo.");
    //
    //             var data = await response.Content.ReadFromJsonAsync<DeviceStatusDto>();
    //
    //             var status = data!.IsOnline
    //                 ? DeviceVerificationStatus.Online
    //                 : DeviceVerificationStatus.Offline;
    //
    //             return DeviceVerificationResult.Ok(status);
    //         }
    //         catch (HttpRequestException)
    //         {
    //             return DeviceVerificationResult.Fail("Sem conexão com a internet.");
    //         }
    //     }
    // }
    //
    // public class DeviceStatusDto
    // {
    //     public bool IsOnline { get; set; }
    //     public DateTime? LastHeartbeatAt { get; set; }
    // }
}
