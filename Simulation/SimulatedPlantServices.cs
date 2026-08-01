using KaaDebug.Core.Interfaces.Plants;
using KaaDebug.Core.Models.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Simulation
{
    public class SimulatedDashboardService : IDashboardService
    {
        private readonly InMemoryDataStore _store;

        public SimulatedDashboardService(InMemoryDataStore store) => _store = store;

        public async Task<DashboardResult> GetDashboardAsync()
        {
            await Task.Delay(600);

            var user = _store.CurrentUser ?? throw new InvalidOperationException("Usuário não inicializado.");

            var recentPlants = _store.Plants
                .Take(3)
                .Select(ToPlantSummary)
                .ToList();

            var recentNotifications = _store.Notifications
                .Where(n => !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .Take(3)
                .Select(ToNotificationSummary)
                .ToList();

            var data = new DashboardData
            {
                UserFirstName = user.Name.Split(' ')[0],
                TotalPlants = _store.Plants.Count,
                ActiveAlertsCount = _store.Notifications.Count(n => !n.IsRead),
                RecentPlants = recentPlants,
                RecentNotifications = recentNotifications
            };

            return DashboardResult.Ok(data);
        }

        private static PlantSummary ToPlantSummary(SimulatedPlant p) => new()
        {
            Id = p.Id,
            Name = p.Name,
            Species = p.SpeciesName,
            PhotoUrl = p.PhotoUrl,
            HealthStatus = p.HealthStatus,
            StatusReason = p.StatusReason
        };

        private static NotificationSummary ToNotificationSummary(SimulatedNotification n) => new()
        {
            Id = n.Id,
            PlantId = n.PlantId,
            PlantName = n.PlantName,
            Message = n.Message,
            Priority = n.Priority,
            CreatedAt = n.CreatedAt,
            IsRead = n.IsRead
        };
    }

    public class SimulatedPlantsService : IPlantsListService
    {
        private readonly InMemoryDataStore _store;

        public SimulatedPlantsService(InMemoryDataStore store) => _store = store;

        public async Task<PlantsListResult> GetAllPlantsAsync()
        {
            await Task.Delay(500);

            var plants = _store.Plants
                .Select(p => new PlantSummary
                {
                    Id = p.Id,
                    Name = p.Name,
                    Species = p.SpeciesName,
                    PhotoUrl = p.PhotoUrl,
                    HealthStatus = p.HealthStatus,
                    StatusReason = p.StatusReason
                })
                .ToList();

            return PlantsListResult.Ok(plants);
        }
    }
}
