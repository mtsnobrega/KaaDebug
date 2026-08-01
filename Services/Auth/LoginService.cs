using KaaDebug.Core.Interfaces.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Services.Auth
{
    /// <summary>
    /// Implementação temporária de ILoginService, usada enquanto a API de
    /// autenticação não está disponível. Permite testar toda a tela de Login
    /// (estados de loading, erro, sucesso) sem depender do backend.
    ///
    /// SUBSTITUIR por uma implementação real (HttpClient -> POST /auth/login)
    /// quando o endpoint existir. O contrato (ILoginService) não deve mudar,
    /// então a LoginPage não precisará de nenhuma alteração nesse momento.
    /// </summary>
    public class LoginService : ILoginService
    {
        public async Task<LoginResult> LoginAsync(string email, string password)
        {
            // Simula latência de rede
            await Task.Delay(1200);

            // Credencial de teste fixa, só para validar o fluxo da tela
            if (email.Equals("teste@email.com", StringComparison.OrdinalIgnoreCase)
                && password == "123456")
            {
                // Token JWT fake, apenas para validar o fluxo de SecureStorage/expiração.
                // Não é um token real assinado - serve só para desenvolvimento.
                const string fakeJwt =
                    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9." +
                    "eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IlRlc3RlIiwiZXhwIjo5OTk5OTk5OTk5fQ." +
                    "fake_signature_for_dev_purposes_only";

                return LoginResult.Ok(fakeJwt);
            }

            return LoginResult.Fail("E-mail ou senha incorretos.");
        }
    }

    /// <summary>
    /// Esqueleto da implementação real, para referência futura.
    /// Descomentar e ajustar quando o endpoint de login existir.
    /// </summary>
    // public class LoginService : ILoginService
    // {
    //     private readonly HttpClient _httpClient;
    //
    //     public LoginService(HttpClient httpClient)
    //     {
    //         _httpClient = httpClient;
    //     }
    //
    //     public async Task<LoginResult> LoginAsync(string email, string password)
    //     {
    //         try
    //         {
    //             var response = await _httpClient.PostAsJsonAsync("auth/login", new
    //             {
    //                 email,
    //                 password
    //             });
    //
    //             if (!response.IsSuccessStatusCode)
    //             {
    //                 if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
    //                     return LoginResult.Fail("E-mail ou senha incorretos.");
    //
    //                 return LoginResult.Fail("Não foi possível entrar. Tente novamente.");
    //             }
    //
    //             var data = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
    //             return LoginResult.Ok(data!.Token);
    //         }
    //         catch (HttpRequestException)
    //         {
    //             return LoginResult.Fail("Sem conexão com a internet.");
    //         }
    //     }
    // }
    //
    // public class LoginResponseDto
    // {
    //     public string Token { get; set; } = string.Empty;
    // }
}
