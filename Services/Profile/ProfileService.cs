using KaaDebug.Core.Interfaces.Auth;
using KaaDebug.Core.Interfaces.Profile;
using KaaDebug.Core.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Services.Profile
{
    public class ProfileService : IProfileService
    {
        private readonly UserProfile _profile = new()
        {
            Name = "Maria Silva",
            Email = "maria@exemplo.com",
            NotificationsEnabled = true,
            CriticalAlertsOnly = false
        };

        public async Task<ProfileResult> GetProfileAsync()
        {
            await Task.Delay(500);
            return ProfileResult.Ok(_profile);
        }

        public async Task<OperationResult> UpdateProfileAsync(UpdateProfileRequest request)
        {
            await Task.Delay(800);
            _profile.Name = request.Name;
            _profile.NotificationsEnabled = request.NotificationsEnabled;
            _profile.CriticalAlertsOnly = request.CriticalAlertsOnly;
            return OperationResult.Ok();
        }

        public async Task<OperationResult> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            await Task.Delay(900);

            if (currentPassword != "123456")
                return OperationResult.Fail("Senha atual incorreta.");

            return OperationResult.Ok();
        }
    }
}
