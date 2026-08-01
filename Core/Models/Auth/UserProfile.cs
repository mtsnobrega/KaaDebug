using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Core.Models.Auth
{
    public class UserProfile
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool NotificationsEnabled { get; set; }
        public bool CriticalAlertsOnly { get; set; }
    }

    public class ProfileResult
    {
        public bool Success { get; init; }
        public UserProfile? Profile { get; init; }
        public string? ErrorMessage { get; init; }

        public static ProfileResult Ok(UserProfile profile) => new() { Success = true, Profile = profile };
        public static ProfileResult Fail(string message) => new() { Success = false, ErrorMessage = message };
    }

    public class UpdateProfileRequest
    {
        public string Name { get; init; } = string.Empty;
        public bool NotificationsEnabled { get; init; }
        public bool CriticalAlertsOnly { get; init; }
    }
}
