/*
 * Responsabilidade:
 * Definir o contrato para a operação de autenticação (Login) perante a API.
 * 
 * Papel na arquitetura:
 * Atua como contrato de serviço de negócio. A utilização da classe `LoginResult` 
 * (Result Pattern) demonstra a intenção de não usar `Exceptions` para controle de fluxo 
 * (como senha incorreta), reservando as exceções apenas para falhas técnicas críticas.
 */

namespace KaaDebug.Core.Interfaces.Auth
{
    public class LoginResult
    {
        public bool Success { get; init; }
        public string? Token { get; init; }
        public string? ErrorMessage { get; init; }

        public static LoginResult Ok(string token) =>
            new() { Success = true, Token = token };

        public static LoginResult Fail(string message) =>
            new() { Success = false, ErrorMessage = message };
    }
    public interface ILoginService
    {
        Task<LoginResult> LoginAsync(string email, string password);
    }
}
