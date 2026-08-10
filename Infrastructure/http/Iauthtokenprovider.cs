using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Infrastructure.http
{
    /// <summary>
    /// Abstração para leitura do token JWT salvo localmente.
    /// Permite que o ApiClient injete o token sem depender diretamente
    /// do SecureStorage — facilita testes e manutenção.
    /// </summary>
    public interface IAuthTokenProvider
    {
        Task<string?> GetTokenAsync();
    }

    /// <summary>
    /// Implementação real: lê o token JWT do SecureStorage do MAUI.
    /// O token é salvo pelo AuthService após login bem-sucedido.
    /// </summary>
    public class SecureStorageTokenProvider : IAuthTokenProvider
    {
        private const string TokenKey = "auth_token";

        public async Task<string?> GetTokenAsync()
        {
            try
            {
                return await SecureStorage.Default.GetAsync(TokenKey);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao ler token: {ex.Message}");
                return null;
            }
        }
    }
}
