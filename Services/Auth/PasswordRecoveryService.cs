/*
 * Responsabilidade:
 * Implementar a comunicação HTTP dos três endpoints relacionados ao fluxo de recuperação de senha.
 * 
 * Papel na arquitetura:
 * Camada de Serviço (Service Layer). Encapsula as chamadas POST para RequestCode, 
 * ValidateCode e ResetPassword utilizando o `ApiClient`.
 */

using KaaDebug.Core.Interfaces.Auth;
using KaaDebug.Infrastructure.http;

namespace KaaDebug.Services.Auth;

public class PasswordRecoveryService : IPasswordRecoveryService
{
    private readonly ApiClient _apiClient;

    public PasswordRecoveryService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<OperationResult> RequestCodeAsync(string email)
    {
        var result = await _apiClient.PostAsync(
            ApiConstants.Auth.RequestCode,
            new { email });

        // A API sempre retorna 200 neste endpoint (evita enumeração de usuários)
        return OperationResult.Ok();
    }
    public async Task<OperationResult> ValidateCodeAsync(string email, string code)
    {
        var result = await _apiClient.PostAsync(
            ApiConstants.Auth.ValidateCode,
            new { email, code });

        return result.Success
            ? OperationResult.Ok()
            : OperationResult.Fail(result.ErrorMessage!);
    }

    public async Task<OperationResult> ResetPasswordAsync(string email, string code, string newPassword)
    {
        var result = await _apiClient.PostAsync(
            ApiConstants.Auth.ResetPassword,
            new { email, code, newPassword });

        return result.Success
            ? OperationResult.Ok()
            : OperationResult.Fail(result.ErrorMessage!);
    }
}