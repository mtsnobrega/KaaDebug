/*
 * Responsabilidade:
 * Armazenar de forma centralizada todas as rotas (endpoints) da BFF API.
 * 
 * Papel na arquitetura:
 * Dicionário estático de rotas. Utiliza diretivas de compilação (#if ANDROID) 
 * para alternar dinamicamente a URL base (BaseUrl) dependendo do ambiente 
 * de execução (Emulador Android vs Computador Local), facilitando o desenvolvimento.
 */

namespace KaaDebug.Infrastructure.http
{
    /// <summary>
    /// Centraliza a URL base e todos os endpoints consumidos pelo app.
    /// Altere BaseUrl conforme o ambiente:
    ///   - Windows (WinUI):         http://localhost:5000
    ///   - Emulador Android:        http://10.0.2.2:5000
    ///   - Dispositivo físico:      http://IP_DA_MAQUINA:5000
    ///   - Produção:                https://api.seudominio.com
    /// </summary>
    public static class ApiConstants
    {
#if ANDROID
    public const string BaseUrl = "http://10.0.2.2:5000/";
#else
        public const string BaseUrl = "http://localhost:5160/";
#endif

        public static class Auth
        {
            public const string Login = "auth/login";
            public const string Register = "auth/register";
            public const string RequestCode = "auth/recovery/request-code";
            public const string ValidateCode = "auth/recovery/validate-code";
            public const string ResetPassword = "auth/recovery/reset-password";
        }

        public static class Dashboard
        {
            public const string Get = "dashboard";
        }

        public static class Species
        {
            public const string GetAll = "species";
        }

        public static class Plants
        {
            public const string GetAll = "plants";
            public const string Create = "plants";
            public static string GetDetails(Guid id) => $"plants/{id}";
            public static string Update(Guid id) => $"plants/{id}";
            public static string Delete(Guid id) => $"plants/{id}";
            public static string GetHistory(Guid id, string period) => $"plants/{id}/history?period={period}";
            public static string GetCareTips(Guid id) => $"plants/{id}/care-tips";
            public static string Diagnose(Guid id) => $"plants/{id}/diagnosis";
            public static string GetDiagnoses(Guid id) => $"plants/{id}/diagnosis";
            public static string AssociateDevice(Guid plantId) => $"plants/{plantId}/device";
        }

        public static class Devices
        {
            public static string Verify(string code) => $"devices/{code}/status";
           // public static string AssociateDevice(Guid plantId) => $"plants/{plantId}/device";
        }

        public static class Notifications
        {
            public const string GetAll = "notifications";
            public static string MarkRead(Guid id) => $"notifications/{id}/read";
            public const string ClearAll = "notifications";
        }

        public static class Profile
        {
            public const string Get = "profile";
            public const string Update = "profile";
            public const string ChangePassword = "profile/change-password";
        }
    }
}
