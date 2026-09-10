/*
 * Responsabilidade:
 * Representar a estrutura de dados e as entidades de negócio utilizadas pelo aplicativo.
 * 
 * Papel na arquitetura:
 * Camada de Domínio (Core Models). Estes modelos são independentes de framework visual 
 * e de bibliotecas de rede. Eles atuam como a "Linguagem Ubíqua" do sistema, 
 * definindo Enums (como PlantHealthStatus), agregados de informações 
 * (como DashboardData) e Tipos de Valor (como IdealRange).
 */

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
