using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Core.Interfaces.Auth
{
    /// <summary>
    /// Resultado de uma tentativa de login.
    /// Modelado como objeto de resultado (em vez de lançar exceção para
    /// credenciais inválidas) para diferenciar claramente erros de negócio
    /// (senha errada) de erros técnicos (sem internet, servidor fora).
    /// </summary>
    public class LoginResult
    {
        public bool Success { get; init; }
        public string? Token { get; init; }
        public string? ErrorMessage { get; init; }

        public static LoginResult Ok(string token) =>
            new() { Success = true, Token = token };

        public static LoginResult Fail(string message) =>
            new() { Success = false, ErrorMessage = message };
    }

    /// <summary>
    /// Abstração para autenticação contra a API.
    /// A implementação real (LoginService) será criada quando o endpoint
    /// POST /auth/login estiver definido no backend. Até então, usar
    /// FakeLoginService para desenvolvimento e testes de tela.
    /// </summary>
    public interface ILoginService
    {
        Task<LoginResult> LoginAsync(string email, string password);
    }
}
