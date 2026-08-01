/// <summary>
/// Abstração responsável por verificar e gerenciar a sessão do usuário.
/// A implementação concreta (HTTP + JWT) será criada quando os endpoints
/// de autenticação da API estiverem definidos. Até então, usar
/// FakeAuthService para permitir o desenvolvimento e testes das telas.
/// </summary>

namespace KaaDebug.Core.Interfaces.Auth
{
    public interface IAuthService
    {
        /// <summary>
        /// Verifica se existe um token JWT salvo localmente e se ele ainda é válido.
        /// Deve checar: existência do token, expiração (claim "exp") e,
        /// futuramente, possibilidade de refresh automático.
        /// </summary>
        Task<bool> IsSessionValidAsync();

        /// <summary>
        /// Persiste o token JWT recebido após login bem-sucedido.
        /// </summary>
        Task SaveSessionAsync(string token);

        /// <summary>
        /// Remove o token salvo (logout).
        /// </summary>
        Task ClearSessionAsync();
    }
}
