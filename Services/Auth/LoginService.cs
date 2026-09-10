/*
 * Responsabilidade:
 * Implementar a comunicação real de login, conectando a View à BFF API.
 * 
 * Papel na arquitetura:
 * Camada de Serviço (Service Layer). Depende do `ApiClient` para executar a chamada HTTP 
 * e do `IAuthService` para persistir o token recebido caso a operação seja bem-sucedida.
 * 
 * Fluxo:
 * Executa POST (email, senha) -> Verifica sucesso via Result -> Salva Token -> Retorna Result.
 */

using KaaDebug.Core.Interfaces.Auth;
using KaaDebug.Infrastructure.http;

namespace KaaDebug.Services.Auth;

public class LoginService : ILoginService
{
    private readonly ApiClient _apiClient;
    private readonly IAuthService _authService;

    public LoginService(ApiClient apiClient, IAuthService authService)
    {
        _apiClient = apiClient;
        _authService = authService;
    }
    public async Task<LoginResult> LoginAsync(string email, string password)
    {
        var result = await _apiClient.PostAsync<LoginResponseDto>(
            ApiConstants.Auth.Login,
            new { email, password });

        if (!result.Success)
            return LoginResult.Fail(result.ErrorMessage!);

        await _authService.SaveSessionAsync(result.Data!.Token);

        return LoginResult.Ok(result.Data.Token);
    }
}