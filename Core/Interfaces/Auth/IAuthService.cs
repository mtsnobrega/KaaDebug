/*
 * Responsabilidade:
 * Definir o contrato para o gerenciamento do ciclo de vida da sessão (token JWT) no dispositivo.
 * 
 * Papel na arquitetura:
 * Atua como a interface de persistência de segurança (Core/Interfaces). Isola a lógica 
 * de como o token é validado e armazenado (neste caso, usando SecureStorage) das 
 * demais regras de negócio e de interface.
 * 
 * Dependências:
 * A implementação (AuthService) consome o `SecureStorage` nativo do MAUI e a 
 * biblioteca de manipulação de JWT (JwtSecurityTokenHandler).
 */

namespace KaaDebug.Core.Interfaces.Auth
{
    public interface IAuthService
    {
        /// <summary>
        /// Verifica se existe um token JWT salvo localmente e se ele ainda é válido.
        /// Deve checar: existência do token, expiração (claim "exp") e futuramente, possibilidade de refresh automático.
        /// </summary>
        Task<bool> IsSessionValidAsync();

        /// Persiste o token JWT recebido após login bem-sucedido.
        Task SaveSessionAsync(string token);

        /// Remove o token salvo (logout).
        Task ClearSessionAsync();
    }
}
