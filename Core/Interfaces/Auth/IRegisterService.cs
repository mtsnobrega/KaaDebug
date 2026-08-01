namespace KaaDebug.Core.Interfaces.Auth
{
    /// <summary>
    /// Dados necessários para registrar um novo usuário.
    /// </summary>
    public class RegisterRequest
    {
        public string Name { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }

    /// <summary>
    /// Abstração para o cadastro de novos usuários.
    /// A implementação real dependerá do endpoint futuro POST /auth/register.
    /// </summary>
    public interface IRegisterService
    {
        Task<OperationResult> RegisterAsync(RegisterRequest request);
    }
}
