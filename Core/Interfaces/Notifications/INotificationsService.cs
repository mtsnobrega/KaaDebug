/*
 * Responsabilidade:
 * Definir o contrato para o gerenciamento da caixa de notificações do usuário.
 * 
 * Papel na arquitetura:
 * Contrato de domínio (Core). Centraliza as ações de buscar histórico, marcar 
 * notificações individuais como lidas e limpar o repositório completo, utilizando 
 * o padrão de Result Objects (NotificationsListResult, OperationResult).
 */

using KaaDebug.Core.Interfaces.Auth;
using KaaDebug.Core.Models.Dashboard;

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
    public interface INotificationsService
    {
        Task<NotificationsListResult> GetAllNotificationsAsync();
        Task<OperationResult> MarkAsReadAsync(string notificationId);
        Task<OperationResult> ClearAllNotificationsAsync();
    }
}
