/*
 * Responsabilidade:
 * Implementar o envio dos dados do novo usuário para a API de cadastro.
 * 
 * Papel na arquitetura:
 * Camada de Serviço (Service Layer). Transforma o objeto `RegisterRequest` 
 * em uma payload anônima para envio via `ApiClient`.
 */

using KaaDebug.Core.Interfaces.Auth;
using KaaDebug.Infrastructure.http;

namespace KaaDebug.Services.Auth;

public class RegisterService : IRegisterService
{
    private readonly ApiClient _apiClient;

    public RegisterService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<OperationResult> RegisterAsync(RegisterRequest request)
    {
        var result = await _apiClient.PostAsync(
            ApiConstants.Auth.Register,
            new
            {
                name = request.Name,
                email = request.Email,
                password = request.Password
            });

        return result.Success
            ? OperationResult.Ok()
            : OperationResult.Fail(result.ErrorMessage!);
    }
}