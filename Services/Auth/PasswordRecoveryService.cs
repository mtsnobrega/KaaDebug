using KaaDebug.Core.Interfaces.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Services.Auth
{
    /// <summary>
    /// Implementação temporária de IPasswordRecoveryService, para desenvolvimento
    /// e testes da tela enquanto a API não existe.
    ///
    /// Código de teste fixo: 123456 (qualquer e-mail é aceito na etapa 1).
    ///
    /// SUBSTITUIR pela implementação real (HttpClient) quando os endpoints
    /// de recuperação de senha estiverem disponíveis no backend.
    /// </summary>
    public class PasswordRecoveryService : IPasswordRecoveryService
    {
        private const string FakeValidCode = "123456";

        public async Task<OperationResult> RequestCodeAsync(string email)
        {
            await Task.Delay(1000);

            // Em um cenário real, e-mails não cadastrados normalmente também
            // retornam sucesso por segurança (evitar enumeração de usuários),
            // mas isso é uma decisão de backend a ser confirmada futuramente.
            return OperationResult.Ok();
        }

        public async Task<OperationResult> ValidateCodeAsync(string email, string code)
        {
            await Task.Delay(800);

            return code == FakeValidCode
                ? OperationResult.Ok()
                : OperationResult.Fail("Código incorreto. Verifique e tente novamente.");
        }

        public async Task<OperationResult> ResetPasswordAsync(string email, string code, string newPassword)
        {
            await Task.Delay(1000);

            if (code != FakeValidCode)
                return OperationResult.Fail("O código expirou. Solicite um novo.");

            return OperationResult.Ok();
        }
    }

    // Esqueleto da implementação real, para referência futura:
    //
    // public class PasswordRecoveryService : IPasswordRecoveryService
    // {
    //     private readonly HttpClient _httpClient;
    //
    //     public PasswordRecoveryService(HttpClient httpClient) => _httpClient = httpClient;
    //
    //     public async Task<OperationResult> RequestCodeAsync(string email)
    //     {
    //         var response = await _httpClient.PostAsJsonAsync("auth/recovery/request-code", new { email });
    //         return response.IsSuccessStatusCode
    //             ? OperationResult.Ok()
    //             : OperationResult.Fail("Não foi possível enviar o código. Tente novamente.");
    //     }
    //
    //     public async Task<OperationResult> ValidateCodeAsync(string email, string code)
    //     {
    //         var response = await _httpClient.PostAsJsonAsync("auth/recovery/validate-code", new { email, code });
    //         return response.IsSuccessStatusCode
    //             ? OperationResult.Ok()
    //             : OperationResult.Fail("Código incorreto. Verifique e tente novamente.");
    //     }
    //
    //     public async Task<OperationResult> ResetPasswordAsync(string email, string code, string newPassword)
    //     {
    //         var response = await _httpClient.PostAsJsonAsync("auth/recovery/reset-password", new { email, code, newPassword });
    //         return response.IsSuccessStatusCode
    //             ? OperationResult.Ok()
    //             : OperationResult.Fail("Não foi possível redefinir a senha. Tente novamente.");
    //     }
    // }
}
