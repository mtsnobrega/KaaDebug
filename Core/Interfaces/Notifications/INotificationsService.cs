using KaaDebug.Core.Interfaces.Auth;
using KaaDebug.Core.Models.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Core.Interfaces.Notifications
{
    public class NotificationsListResult
    {
        public bool Success { get; init; }
        public List<NotificationSummary>? Notifications { get; init; }
        public string? ErrorMessage { get; init; }

        public static NotificationsListResult Ok(List<NotificationSummary> notifications) =>
            new() { Success = true, Notifications = notifications };
        public static NotificationsListResult Fail(string message) =>
            new() { Success = false, ErrorMessage = message };
    }

    /// <summary>
    /// Abstração para operações sobre notificações do usuário.
    /// A implementação real dependerá de endpoints futuros:
    ///   GET    /notifications
    ///   PUT    /notifications/{id}/read
    ///   DELETE /notifications
    /// </summary>
    public interface INotificationsService
    {
        Task<NotificationsListResult> GetAllNotificationsAsync();
        Task<OperationResult> MarkAsReadAsync(string notificationId);
        Task<OperationResult> ClearAllNotificationsAsync();
    }
}
