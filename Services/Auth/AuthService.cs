using KaaDebug.Core.Interfaces.Auth;
public class AuthService : IAuthService
{
    private const string TokenKey = "auth_token";
    public async Task<bool> IsSessionValidAsync()
    {
        try
        {
            var token = await SecureStorage.Default.GetAsync(TokenKey);
            if (string.IsNullOrWhiteSpace(token))
                return false;

            return IsTokenValid(token);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao verificar sessão: {ex.Message}");
            return false;
        }
    }

    public async Task SaveSessionAsync(string token)
    {
        await SecureStorage.Default.SetAsync(TokenKey, token);
    }

    public Task ClearSessionAsync()
    {
        SecureStorage.Default.Remove(TokenKey);
        return Task.CompletedTask;
    }

    private static bool IsTokenValid(string token)
    {
        try
        {
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token)) return false;

            var jwt = handler.ReadJwtToken(token);
            return jwt.ValidTo > DateTime.UtcNow;
        }
        catch
        {
            return false;
        }
    }
}