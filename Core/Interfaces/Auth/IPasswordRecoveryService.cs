namespace KaaDebug.Core.Interfaces.Auth
{
    /// <summary>
    /// Resultado genérico de operação, usado nas etapas de recuperação de senha.
    /// Evita exceptions para fluxos de erro esperados (e-mail não cadastrado,
    /// código incorreto, etc).
    /// </summary>
    public class OperationResult
    {
        public bool Success { get; init; }
        public string? ErrorMessage { get; init; }

        public static OperationResult Ok() => new() { Success = true };
        public static OperationResult Fail(string message) => new() { Success = false, ErrorMessage = message };
    }

    /// <summary>
    /// Abstração para o fluxo de recuperação de senha em 3 etapas:
    /// 1. Solicitar envio do código para o e-mail
    /// 2. Validar o código recebido
    /// 3. Definir a nova senha
    ///
    /// A implementação real dependerá de 3 endpoints futuros:
    ///   POST /auth/recovery/request-code
    ///   POST /auth/recovery/validate-code
    ///   POST /auth/recovery/reset-password
    /// </summary>
    public interface IPasswordRecoveryService
    {
        Task<OperationResult> RequestCodeAsync(string email);

        Task<OperationResult> ValidateCodeAsync(string email, string code);

        /// <summary>
        /// Redefine a senha. O código é reenviado aqui também (e não só na
        /// validação) porque, por segurança, o backend deve revalidar o código
        /// no momento efetivo da troca - ele pode ter expirado entre as etapas.
        /// </summary>
        Task<OperationResult> ResetPasswordAsync(string email, string code, string newPassword);
    }
}
