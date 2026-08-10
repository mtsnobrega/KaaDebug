using KaaDebug.Core.Interfaces.Auth;
using KaaDebug.Core.Interfaces.Profile;
using KaaDebug.Core.Models.Auth;
using KaaDebug.Infrastructure.http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
/*

public class ProfileService : IProfileService
{
    private readonly UserProfile _profile = new()
    {
        Name = "Maria Silva",
        Email = "maria@exemplo.com",
        NotificationsEnabled = true,
        CriticalAlertsOnly = false
    };

    public async Task<ProfileResult> GetProfileAsync()
    {
        await Task.Delay(500);
        return ProfileResult.Ok(_profile);
    }

    public async Task<OperationResult> UpdateProfileAsync(UpdateProfileRequest request)
    {
        await Task.Delay(800);
        _profile.Name = request.Name;
        _profile.NotificationsEnabled = request.NotificationsEnabled;
        _profile.CriticalAlertsOnly = request.CriticalAlertsOnly;
        return OperationResult.Ok();
    }

    public async Task<OperationResult> ChangePasswordAsync(string currentPassword, string newPassword)
    {
        await Task.Delay(900);

        if (currentPassword != "123456")
            return OperationResult.Fail("Senha atual incorreta.");

        return OperationResult.Ok();
    }
}
*/