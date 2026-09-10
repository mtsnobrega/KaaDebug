/*
 * Responsabilidade:
 * Implementar a comunicação com a API BFF para as ações de perfil do usuário.
 * 
 * Papel na arquitetura:
 * Camada de Serviço (Service Layer). Utiliza o ApiClient para realizar GET, PUT 
 * e POST. Transforma e encapsula as respostas da rede em Result Objects 
 * (ProfileResult e OperationResult) para uso seguro pela interface.
 */
using KaaDebug.Core.Interfaces.Auth;
using KaaDebug.Core.Interfaces.Profile;
using KaaDebug.Core.Models.Auth;
using KaaDebug.Infrastructure.http;

namespace KaaDebug.Services.Profile;
public class ProfileService : IProfileService
{
    private readonly ApiClient _apiClient;

    public ProfileService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<ProfileResult> GetProfileAsync()
    {
        var result = await _apiClient.GetAsync<ProfileDto>(ApiConstants.Profile.Get);

        if (!result.Success)
            return ProfileResult.Fail(result.ErrorMessage!);

        return ProfileResult.Ok(new UserProfile
        {
            Name = result.Data!.Name,
            Email = result.Data.Email,
            NotificationsEnabled = result.Data.NotificationsEnabled,
            CriticalAlertsOnly = result.Data.CriticalAlertsOnly
        });
    }

    public async Task<OperationResult> UpdateProfileAsync(UpdateProfileRequest request)
    {
        var result = await _apiClient.PutAsync(
            ApiConstants.Profile.Update,
            new
            {
                name = request.Name,
                notificationsEnabled = request.NotificationsEnabled,
                criticalAlertsOnly = request.CriticalAlertsOnly
            });

        return result.Success
            ? OperationResult.Ok()
            : OperationResult.Fail(result.ErrorMessage!);
    }

    public async Task<OperationResult> ChangePasswordAsync(string currentPassword, string newPassword)
    {
        var result = await _apiClient.PostAsync(
            ApiConstants.Profile.ChangePassword,
            new { currentPassword, newPassword });

        return result.Success
            ? OperationResult.Ok()
            : OperationResult.Fail(result.ErrorMessage!);
    }
}