/*
 * Responsabilidade:
 * Implementar a comunicação com a API BFF para o domínio de notificações.
 * 
 * Papel na arquitetura:
 * Camada de Serviço (Service Layer). Responsável por executar GET, PUT e DELETE 
 * via ApiClient. Atua como tradutor, convertendo a string de prioridade ("HIGH") 
 * para o enumerador interno (NotificationPriority.High).
 */

using KaaDebug.Core.Interfaces.Auth;
using KaaDebug.Core.Interfaces.Notifications;
using KaaDebug.Core.Models.Dashboard;
using KaaDebug.Infrastructure.http;

namespace KaaDebug.Services.Notifications;
public class NotificationsService : INotificationsService
{
    private readonly ApiClient _apiClient;

    public NotificationsService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<NotificationsListResult> GetAllNotificationsAsync()
    {
        var result = await _apiClient.GetAsync<List<NotificationDto>>(
            ApiConstants.Notifications.GetAll);

        if (!result.Success)
            return NotificationsListResult.Fail(result.ErrorMessage!);

        var notifications = result.Data!.Select(dto => new NotificationSummary
        {
            Id = dto.Id.ToString(),
            PlantId = dto.PlantId.ToString(),
            PlantName = dto.PlantName,
            Message = dto.Message,
            Priority = dto.Priority.ToUpper() switch
            {
                "HIGH" => NotificationPriority.High,
                "MEDIUM" => NotificationPriority.Medium,
                _ => NotificationPriority.Low
            },
            IsRead = dto.IsRead,
            CreatedAt = dto.CreatedAt
        }).ToList();

        return NotificationsListResult.Ok(notifications);
    }

    public async Task<OperationResult> MarkAsReadAsync(string notificationId)
    {
        var result = await _apiClient.PutAsync(
            ApiConstants.Notifications.MarkRead(Guid.Parse(notificationId)));

        return result.Success
            ? OperationResult.Ok()
            : OperationResult.Fail(result.ErrorMessage!);
    }

    public async Task<OperationResult> ClearAllNotificationsAsync()
    {
        var result = await _apiClient.DeleteAsync(ApiConstants.Notifications.ClearAll);

        return result.Success
            ? OperationResult.Ok()
            : OperationResult.Fail(result.ErrorMessage!);
    }
}