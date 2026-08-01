using KaaDebug.Core.Interfaces.Plants;
using KaaDebug.Core.Models.Dashboard;
using KaaDebug.Core.Models.Plants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Simulation
{
    public class SimulatedPlantDetailsService : IPlantDetailsService
    {
        private readonly InMemoryDataStore _store;

        public SimulatedPlantDetailsService(InMemoryDataStore store) => _store = store;

        public async Task<PlantDetailsResult> GetPlantDetailsAsync(string plantId)
        {
            await Task.Delay(600);

            var plant = _store.Plants.FirstOrDefault(p => p.Id == plantId);
            if (plant is null)
                return PlantDetailsResult.Fail("Planta não encontrada.");

            var species = _store.Species.FirstOrDefault(s => s.Id == plant.SpeciesId);
            var device = _store.Devices.FirstOrDefault(d => d.Id == plant.DeviceId);
            var readings = _store.SensorReadings.GetValueOrDefault(plantId) ?? new();
            var notifications = _store.Notifications
                .Where(n => n.PlantId == plantId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(5)
                .Select(ToNotificationSummary)
                .ToList();

            var indicators = BuildIndicators(readings, species);

            var details = new PlantDetails
            {
                Id = plant.Id,
                Name = plant.Name,
                Species = plant.SpeciesName,
                PhotoUrl = plant.PhotoUrl,
                HealthStatus = plant.HealthStatus,
                StatusReason = plant.StatusReason,
                Device = new DeviceInformation
                {
                    DeviceCode = device?.Code,
                    ConnectionStatus = device?.ConnectionStatus ?? DeviceConnectionStatus.NotAssociated,
                    LastReadingAt = device?.LastHeartbeatAt
                },
                Indicators = indicators,
                RelatedNotifications = notifications
            };

            return PlantDetailsResult.Ok(details);
        }

        private List<SensorIndicator> BuildIndicators(
            List<SimulatedSensorReading> readings,
            PlantSpecies? species)
        {
            var sensorTypes = new[]
            {
            SensorType.SoilMoisture,
            SensorType.AirHumidity,
            SensorType.Temperature,
            SensorType.Luminosity
        };

            var indicators = new List<SensorIndicator>();
            var last24h = DateTime.Now.AddHours(-24);

            foreach (var type in sensorTypes)
            {
                var typeReadings = readings
                    .Where(r => r.SensorType == type)
                    .OrderBy(r => r.Timestamp)
                    .ToList();

                var recent = typeReadings.Where(r => r.Timestamp >= last24h).ToList();
                var currentValue = typeReadings.LastOrDefault()?.Value ?? 0;
                var idealRange = GetIdealRange(type, species);

                indicators.Add(new SensorIndicator
                {
                    Type = type,
                    CurrentValue = currentValue,
                    Unit = idealRange.Unit,
                    IdealRange = idealRange,
                    IsWithinIdealRange = currentValue >= idealRange.Min && currentValue <= idealRange.Max,
                    RecentHistory = recent.Select(r => new SensorReadingPoint
                    {
                        Timestamp = r.Timestamp,
                        Value = r.Value
                    }).ToList()
                });
            }

            return indicators;
        }

        private static IdealRange GetIdealRange(SensorType type, PlantSpecies? species)
        {
            if (species is null)
                return new IdealRange { Min = 0, Max = 100, Unit = "?" };

            return type switch
            {
                SensorType.SoilMoisture => species.IdealParameters.SoilMoisture,
                SensorType.AirHumidity => species.IdealParameters.AirHumidity,
                SensorType.Temperature => species.IdealParameters.Temperature,
                SensorType.Luminosity => species.IdealParameters.Luminosity,
                _ => new IdealRange()
            };
        }

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

    public class SimulatedPlantEditService : IPlantsEditService
    {
        private readonly InMemoryDataStore _store;

        public SimulatedPlantEditService(InMemoryDataStore store) => _store = store;

        public async Task<EditPlantResult> UpdatePlantAsync(EditPlantRequest request)
        {
            await Task.Delay(700);

            var plant = _store.Plants.FirstOrDefault(p => p.Id == request.PlantId);
            if (plant is null) return EditPlantResult.Fail("Planta não encontrada.");

            if (!string.IsNullOrEmpty(request.Name))
                plant.Name = request.Name;

            // Gestão de dispositivo
            if (request.DeviceCode is not null)
            {
                if (request.DeviceCode == string.Empty)
                {
                    // Desassociar
                    if (plant.DeviceId is not null)
                    {
                        var oldDevice = _store.Devices.FirstOrDefault(d => d.Id == plant.DeviceId);
                        if (oldDevice is not null) oldDevice.AssociatedPlantId = null;
                        plant.DeviceId = null;
                    }
                }
                else
                {
                    // Associar/substituir
                    var device = _store.Devices.FirstOrDefault(d =>
                        d.Code.Equals(request.DeviceCode, StringComparison.OrdinalIgnoreCase));

                    if (device is null)
                        return EditPlantResult.Fail("Código de dispositivo inválido ou não encontrado.");

                    device.AssociatedPlantId = plant.Id;
                    plant.DeviceId = device.Id;
                }
            }

            return EditPlantResult.Ok();
        }

        public async Task<EditPlantResult> DeletePlantAsync(string plantId)
        {
            await Task.Delay(600);

            var plant = _store.Plants.FirstOrDefault(p => p.Id == plantId);
            if (plant is null) return EditPlantResult.Fail("Planta não encontrada.");

            // Desassocia o dispositivo antes de remover
            if (plant.DeviceId is not null)
            {
                var device = _store.Devices.FirstOrDefault(d => d.Id == plant.DeviceId);
                if (device is not null) device.AssociatedPlantId = null;
            }

            _store.Plants.Remove(plant);
            _store.SensorReadings.Remove(plantId);
            _store.Notifications.RemoveAll(n => n.PlantId == plantId);
            _store.DiagnosisHistory.Remove(plantId);

            return EditPlantResult.Ok();
        }
    }

    public class SimulatedPlantRegistrationService : IPlantsRegistrationService
    {
        private readonly InMemoryDataStore _store;
        private int _nextPlantIndex = 5;

        public SimulatedPlantRegistrationService(InMemoryDataStore store) => _store = store;

        public async Task<CreatePlantResult> CreatePlantAsync(CreatePlantRequest request)
        {
            await Task.Delay(900);

            var species = _store.Species.FirstOrDefault(s => s.Id == request.SpeciesId);
            if (species is null)
                return CreatePlantResult.Fail("Espécie não encontrada.");

            SimulatedDevice? device = null;
            if (!string.IsNullOrEmpty(request.DeviceCode))
            {
                device = _store.Devices.FirstOrDefault(d =>
                    d.Code.Equals(request.DeviceCode, StringComparison.OrdinalIgnoreCase));

                if (device is null)
                    return CreatePlantResult.Fail("Código de dispositivo inválido ou não encontrado.");
            }

            var plantId = $"p{_nextPlantIndex++}";
            var newPlant = new SimulatedPlant
            {
                Id = plantId,
                Name = request.Name,
                SpeciesId = species.Id,
                SpeciesName = species.Name,
                PhotoUrl = species.PhotoUrl,
                DeviceId = device?.Id,
                HealthStatus = PlantHealthStatus.Healthy,
                CreatedAt = DateTime.Now
            };

            _store.Plants.Add(newPlant);
            _store.SensorReadings[plantId] = new List<SimulatedSensorReading>();

            if (device is not null)
                device.AssociatedPlantId = plantId;

            return CreatePlantResult.Ok(plantId);
        }
    }
}
