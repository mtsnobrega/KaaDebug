using KaaDebug.Core.Interfaces.Auth;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Services.Auth
{
    /// <summary>
    /// Implementação de IAuthService baseada em SecureStorage do MAUI.
    /// Já é funcional para a parte local (salvar/ler/limpar token e checar expiração).
    /// O que ainda depende da API futura é apenas o LOGIN em si (feito na LoginPage),
    /// que vai chamar um endpoint e então usar SaveSessionAsync().
    /// </summary>
    public class AuthService : IAuthService
    {

        private const string TokenKey = "auth_token";

        public async Task<bool> IsSessionValidAsync()
        {
            Debug.WriteLine("1 - Entrou em IsSessionValidAsync");
            string? token;

            try
            {
                token = await SecureStorage.Default.GetAsync(TokenKey);
                Debug.WriteLine($"2 - Token: {token}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("4 - Token vazio");

                // SecureStorage pode falhar em alguns dispositivos/cenários (ex: keystore corrompido)
                System.Diagnostics.Debug.WriteLine($"Erro ao ler SecureStorage: {ex.Message}");
                return false;
            }

            if (string.IsNullOrWhiteSpace(token))
                return false;

            return IsTokenValid(token);
        }

        public async Task SaveSessionAsync(string token)
        {
            await SecureStorage.Default.SetAsync(TokenKey, token);
        }

        public async Task ClearSessionAsync()
        {
            SecureStorage.Default.Remove(TokenKey);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Decodifica o JWT localmente (sem validar assinatura, pois isso é
        /// responsabilidade do backend) apenas para checar a claim "exp".
        /// Requer o pacote NuGet: System.IdentityModel.Tokens.Jwt
        /// </summary>
        private static bool IsTokenValid(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();

                if (!handler.CanReadToken(token))
                    return false;

                var jwt = handler.ReadJwtToken(token);
                return jwt.ValidTo > DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Token inválido: {ex.Message}");
                return false;
            }
        }
    }
}
