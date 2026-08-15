using KaaDebug.Core.Interfaces.Devices;
using KaaDebug.Infrastructure.http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
