/*
 * Responsabilidade:
 * Definir os dados necessários (Model/DTO) e a operação de cadastro de um novo usuário.
 * 
 * Papel na arquitetura:
 * Contrato de serviço. A classe `RegisterRequest` atua como um DTO de entrada 
 * focado especificamente na payload necessária para a API de criação de conta.
 */
namespace KaaDebug.Core.Interfaces.Auth
{
  
    // Dados necessários para registrar um novo usuário.
    public class RegisterRequest
    {
        public string Name { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }
    public interface IRegisterService
    {
        Task<OperationResult> RegisterAsync(RegisterRequest request);
    }
}
