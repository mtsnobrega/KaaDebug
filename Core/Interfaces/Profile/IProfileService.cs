/*
 * Responsabilidade:
 * Definir o contrato para gerenciamento dos dados do usuário logado e de sua conta.
 * 
 * Papel na arquitetura:
 * Contrato de domínio (Core). Centraliza as ações de leitura e atualização 
 * de preferências (Notificações) e segurança (Troca de senha).
 */
using KaaDebug.Core.Interfaces.Auth;
using KaaDebug.Core.Models.Auth;

namespace KaaDebug.Core.Interfaces.Profile
{
    public interface IProfileService
    {
        Task<ProfileResult> GetProfileAsync();
        Task<OperationResult> UpdateProfileAsync(UpdateProfileRequest request);
        Task<OperationResult> ChangePasswordAsync(string currentPassword, string newPassword);
    }
}
