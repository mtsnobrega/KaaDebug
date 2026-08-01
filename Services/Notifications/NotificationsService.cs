using KaaDebug.Core.Interfaces.Auth;
using KaaDebug.Core.Interfaces.Notifications;
using KaaDebug.Core.Models.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Services.Notifications
{
    /// <summary>
    /// Implementação temporária de INotificationsService.
    /// Cobre todos os cenários visuais: alta/média/baixa prioridade,
    /// lidas e não lidas, múltiplas plantas.
    ///
    /// SUBSTITUIR pela implementação real (HttpClient) quando os endpoints
    /// estiverem disponíveis.
    /// </summary>
    public class NotificationsService : INotificationsService
    {
        // Lista mutável em memória para simular marcar como lida / limpar
        private List<NotificationSummary> _notifications = new()
    {
        new()
        {
            Id = "n1", PlantId = "p3", PlantName = "Orquídea da Varanda",
            Message = "Umidade do solo crítica — rega necessária urgente",
            Priority = NotificationPriority.High,
            CreatedAt = DateTime.Now.AddHours(-1),
            IsRead = false
        },
        new()
        {
            Id = "n2", PlantId = "p2", PlantName = "Suculenta da Janela",
            Message = "Solo começando a secar",
            Priority = NotificationPriority.Medium,
            CreatedAt = DateTime.Now.AddHours(-5),
            IsRead = false
        },
        new()
        {
            Id = "n3", PlantId = "p3", PlantName = "Orquídea da Varanda",
            Message = "Temperatura acima do ideal",
            Priority = NotificationPriority.Medium,
            CreatedAt = DateTime.Now.AddHours(-8),
            IsRead = false
        },
        new()
        {
            Id = "n4", PlantId = "p1", PlantName = "Samambaia da Sala",
            Message = "Luminosidade abaixo do ideal",
            Priority = NotificationPriority.Low,
            CreatedAt = DateTime.Now.AddDays(-1),
            IsRead = true
        },
        new()
        {
            Id = "n5", PlantId = "p2", PlantName = "Suculenta da Janela",
            Message = "Rega realizada — solo normalizado",
            Priority = NotificationPriority.Low,
            CreatedAt = DateTime.Now.AddDays(-2),
            IsRead = true
        },
        new()
        {
            Id = "n6", PlantId = "p4", PlantName = "Jiboia da Cozinha",
            Message = "Dispositivo offline há mais de 24h",
            Priority = NotificationPriority.High,
            CreatedAt = DateTime.Now.AddDays(-3),
            IsRead = true
        }
    };

        public async Task<NotificationsListResult> GetAllNotificationsAsync()
        {
            await Task.Delay(700);
            return NotificationsListResult.Ok(_notifications.ToList());
        }

        public async Task<OperationResult> MarkAsReadAsync(string notificationId)
        {
            await Task.Delay(300);

            var notification = _notifications.FirstOrDefault(n => n.Id == notificationId);
            if (notification is null)
                return OperationResult.Fail("Notificação não encontrada.");

            notification.IsRead = true;
            return OperationResult.Ok();
        }

        public async Task<OperationResult> ClearAllNotificationsAsync()
        {
            await Task.Delay(500);
            _notifications.Clear();
            return OperationResult.Ok();
        }
    }

    // Esqueleto da implementação real, para referência futura:
    //
    // public class NotificationsService : INotificationsService
    // {
    //     private readonly HttpClient _httpClient;
    //     public NotificationsService(HttpClient httpClient) => _httpClient = httpClient;
    //
    //     public async Task<NotificationsListResult> GetAllNotificationsAsync()
    //     {
    //         try
    //         {
    //             var response = await _httpClient.GetAsync("notifications");
    //             if (!response.IsSuccessStatusCode)
    //                 return NotificationsListResult.Fail("Não foi possível carregar as notificações.");
    //             var list = await response.Content.ReadFromJsonAsync<List<NotificationSummary>>();
    //             return NotificationsListResult.Ok(list ?? new());
    //         }
    //         catch (HttpRequestException) { return NotificationsListResult.Fail("Sem conexão."); }
    //     }
    //
    //     public async Task<OperationResult> MarkAsReadAsync(string notificationId)
    //     {
    //         var response = await _httpClient.PutAsync($"notifications/{notificationId}/read", null);
    //         return response.IsSuccessStatusCode ? OperationResult.Ok() : OperationResult.Fail("Erro ao marcar como lida.");
    //     }
    //
    //     public async Task<OperationResult> ClearAllNotificationsAsync()
    //     {
    //         var response = await _httpClient.DeleteAsync("notifications");
    //         return response.IsSuccessStatusCode ? OperationResult.Ok() : OperationResult.Fail("Erro ao limpar notificações.");
    //     }
    // }
}
