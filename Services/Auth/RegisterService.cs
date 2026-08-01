using KaaDebug.Core.Interfaces.Auth;

namespace KaaDebug.Services.Auth
{
    /// <summary>
    /// Implementação temporária de IRegisterService, para desenvolvimento e
    /// testes da tela enquanto a API de cadastro não existe.
    ///
    /// Simula a regra de negócio "e-mail já cadastrado" para um endereço fixo,
    /// permitindo testar esse cenário de erro na tela.
    ///
    /// SUBSTITUIR pela implementação real (HttpClient -> POST /auth/register)
    /// quando o endpoint estiver disponível.
    /// </summary>
    public class RegisterService : IRegisterService
    {
        private const string AlreadyRegisteredEmail = "teste@Budflow.com";

        public async Task<OperationResult> RegisterAsync(RegisterRequest request)
        {
            await Task.Delay(1200);

            if (request.Email.Equals(AlreadyRegisteredEmail, StringComparison.OrdinalIgnoreCase))
            {
                return OperationResult.Fail("Este e-mail já está cadastrado.");
            }

            return OperationResult.Ok();
        }
    }

    // Esqueleto da implementação real, para referência futura:
    //
    // public class RegisterService : IRegisterService
    // {
    //     private readonly HttpClient _httpClient;
    //
    //     public RegisterService(HttpClient httpClient) => _httpClient = httpClient;
    //
    //     public async Task<OperationResult> RegisterAsync(RegisterRequest request)
    //     {
    //         try
    //         {
    //             var response = await _httpClient.PostAsJsonAsync("auth/register", new
    //             {
    //                 name = request.Name,
    //                 email = request.Email,
    //                 password = request.Password
    //             });
    //
    //             if (!response.IsSuccessStatusCode)
    //             {
    //                 if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
    //                     return OperationResult.Fail("Este e-mail já está cadastrado.");
    //
    //                 return OperationResult.Fail("Não foi possível criar sua conta. Tente novamente.");
    //             }
    //
    //             return OperationResult.Ok();
    //         }
    //         catch (HttpRequestException)
    //         {
    //             return OperationResult.Fail("Sem conexão com a internet.");
    //         }
    //     }
    // }
}
