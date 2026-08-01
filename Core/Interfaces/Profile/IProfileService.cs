using KaaDebug.Core.Interfaces.Auth;
using KaaDebug.Core.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Core.Interfaces.Profile
{
    /// <summary>
    /// Abstração para leitura e atualização do perfil do usuário.
    /// Endpoints futuros:
    ///   GET  /profile
    ///   PUT  /profile
    ///   POST /profile/change-password
    /// </summary>
    public interface IProfileService
    {
        Task<ProfileResult> GetProfileAsync();
        Task<OperationResult> UpdateProfileAsync(UpdateProfileRequest request);
        Task<OperationResult> ChangePasswordAsync(string currentPassword, string newPassword);
    }
}
