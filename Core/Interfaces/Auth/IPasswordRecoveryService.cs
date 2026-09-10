/*
 * Responsabilidade:
 * Definir o fluxo de 3 etapas para recuperação de senha (Solicitação de Código, 
 * Validação de OTP, e Reset de Senha).
 * 
 * Papel na arquitetura:
 * Contrato de serviço (Core). Utiliza o `OperationResult` para sinalizar sucessos 
 * ou falhas de negócio para a interface (View), sem expor a complexidade HTTP.
 */
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
    public interface IPasswordRecoveryService
    {
        Task<OperationResult> RequestCodeAsync(string email);
        Task<OperationResult> ValidateCodeAsync(string email, string code);
        Task<OperationResult> ResetPasswordAsync(string email, string code, string newPassword);
    }
}
