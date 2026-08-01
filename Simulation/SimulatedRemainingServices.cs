using KaaDebug.Core.Interfaces.Auth;
using KaaDebug.Core.Interfaces.Devices;
using KaaDebug.Core.Interfaces.Diagnostic;
using KaaDebug.Core.Interfaces.Notifications;
using KaaDebug.Core.Interfaces.Plants;
using KaaDebug.Core.Interfaces.Profile;
using KaaDebug.Core.Models.Auth;
using KaaDebug.Core.Models.Dashboard;
using KaaDebug.Core.Models.Diagnostic;
using KaaDebug.Core.Models.Plants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Simulation
{
    // ===================== ESPÉCIES =====================

    public class SimulatedSpeciesService : IPlantsCatalogService
    {
        private readonly InMemoryDataStore _store;
        public SimulatedSpeciesService(InMemoryDataStore store) => _store = store;

        public async Task<SpeciesCatalogListResult> GetAllSpeciesAsync()
        {
            await Task.Delay(400);
            return SpeciesCatalogListResult.Ok(_store.Species.ToList());
        }
    }

    // ===================== NOTIFICAÇÕES =====================

    public class SimulatedNotificationsService : INotificationsService
    {
        private readonly InMemoryDataStore _store;
        public SimulatedNotificationsService(InMemoryDataStore store) => _store = store;

        public async Task<NotificationsListResult> GetAllNotificationsAsync()
        {
            await Task.Delay(400);
            var notifications = _store.Notifications
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationSummary
                {
                    Id = n.Id,
                    PlantId = n.PlantId,
                    PlantName = n.PlantName,
                    Message = n.Message,
                    Priority = n.Priority,
                    CreatedAt = n.CreatedAt,
                    IsRead = n.IsRead
                })
                .ToList();

            return NotificationsListResult.Ok(notifications);
        }

        public async Task<OperationResult> MarkAsReadAsync(string notificationId)
        {
            await Task.Delay(200);
            var n = _store.Notifications.FirstOrDefault(n => n.Id == notificationId);
            if (n is null) return OperationResult.Fail("Notificação não encontrada.");
            n.IsRead = true;
            return OperationResult.Ok();
        }

        public async Task<OperationResult> ClearAllNotificationsAsync()
        {
            await Task.Delay(400);
            _store.Notifications.Clear();
            return OperationResult.Ok();
        }
    }

    // ===================== DISPOSITIVO =====================

    public class SimulatedDeviceVerificationService : IDeviceVerificationService
    {
        private readonly InMemoryDataStore _store;
        public SimulatedDeviceVerificationService(InMemoryDataStore store) => _store = store;

        public async Task<DeviceVerificationResult> VerifyDeviceAsync(string deviceCode)
        {
            await Task.Delay(1500);

            var device = _store.Devices.FirstOrDefault(d =>
                d.Code.Equals(deviceCode, StringComparison.OrdinalIgnoreCase));

            if (device is null)
                return DeviceVerificationResult.Fail("Dispositivo não encontrado.");

            // DeviceConnectionStatus (Models) e DeviceVerificationStatus (Services)
            // são enums distintos com semânticas parecidas mas escopos diferentes:
            // um descreve o estado persistido da planta, o outro o resultado pontual
            // de uma verificação de conectividade. O mapeamento é feito aqui.
            var verificationStatus = device.ConnectionStatus switch
            {
                DeviceConnectionStatus.Online => DeviceVerificationStatus.Online,
                DeviceConnectionStatus.Offline => DeviceVerificationStatus.Offline,
                _ => DeviceVerificationStatus.NotFound
            };

            return DeviceVerificationResult.Ok(verificationStatus);
        }
    }

    // ===================== HISTÓRICO DE SENSORES =====================

    public class SimulatedPlantHistoryService : IPlantHistoryService
    {
        private readonly InMemoryDataStore _store;
        public SimulatedPlantHistoryService(InMemoryDataStore store) => _store = store;

        public async Task<PlantHistoryResult> GetHistoryAsync(string plantId, HistoryPeriod period)
        {
            await Task.Delay(600);

            var plant = _store.Plants.FirstOrDefault(p => p.Id == plantId);
            if (plant is null) return PlantHistoryResult.Fail("Planta não encontrada.");

            var species = _store.Species.FirstOrDefault(s => s.Id == plant.SpeciesId);
            var allReadings = _store.SensorReadings.GetValueOrDefault(plantId) ?? new();

            var cutoff = period switch
            {
                HistoryPeriod.Last24Hours => DateTime.Now.AddHours(-24),
                HistoryPeriod.Last7Days => DateTime.Now.AddDays(-7),
                HistoryPeriod.Last30Days => DateTime.Now.AddDays(-30),
                _ => DateTime.Now.AddHours(-24)
            };

            var filtered = allReadings.Where(r => r.Timestamp >= cutoff).ToList();

            var sensorTypes = new[]
            {
            SensorType.SoilMoisture,
            SensorType.AirHumidity,
            SensorType.Temperature,
            SensorType.Luminosity
        };

            var sensors = sensorTypes.Select(type =>
            {
                var typeReadings = filtered
                    .Where(r => r.SensorType == type)
                    .OrderBy(r => r.Timestamp)
                    .Select(r => new SensorReadingPoint { Timestamp = r.Timestamp, Value = r.Value })
                    .ToList();

                var idealRange = GetIdealRange(type, species);

                return new SensorHistory
                {
                    Type = type,
                    Unit = idealRange.Unit,
                    IdealRange = idealRange,
                    Readings = typeReadings
                };
            }).ToList();

            var data = new PlantHistoryData
            {
                PlantName = plant.Name,
                Period = period,
                Sensors = sensors
            };

            return PlantHistoryResult.Ok(data);
        }

        private static IdealRange GetIdealRange(SensorType type, PlantSpecies? species)
        {
            if (species is null) return new IdealRange { Min = 0, Max = 100, Unit = "?" };
            return type switch
            {
                SensorType.SoilMoisture => species.IdealParameters.SoilMoisture,
                SensorType.AirHumidity => species.IdealParameters.AirHumidity,
                SensorType.Temperature => species.IdealParameters.Temperature,
                SensorType.Luminosity => species.IdealParameters.Luminosity,
                _ => new IdealRange()
            };
        }
    }

    // ===================== DICAS DE CUIDADOS =====================

    public class SimulatedBudflowService : IPlantTipsService
    {
        private readonly InMemoryDataStore _store;
        public SimulatedBudflowService(InMemoryDataStore store) => _store = store;

        public async Task<PlantCareInfoResult> GetCareInfoAsync(string plantId)
        {
            await Task.Delay(500);

            var plant = _store.Plants.FirstOrDefault(p => p.Id == plantId);
            var speciesName = plant?.SpeciesName ?? "Planta";

            // Dicas variam conforme a espécie da planta no store
            var info = new PlantCareInfo
            {
                SpeciesName = speciesName,
                Summary = GetSummary(speciesName),
                Curiosity = GetCuriosity(speciesName),
                Tips = GetTips(speciesName)
            };

            return PlantCareInfoResult.Ok(info);
        }

        private static string GetSummary(string species) => species switch
        {
            "Samambaia" => "Samambaias são plantas de sombra que prosperam em ambientes úmidos. Ideais para interiores com boa circulação de ar.",
            "Suculenta" => "Suculentas são resistentes e perfeitas para iniciantes. Armazenam água nos tecidos, tolerando períodos de seca.",
            "Orquídea" => "Orquídeas são elegantes e delicadas. Exigem atenção à umidade e luminosidade para florescer regularmente.",
            "Jiboia" => "A Jiboia é uma das plantas de interior mais resistentes, se adaptando bem a diferentes condições de luz.",
            _ => $"{species} é uma planta que requer cuidados específicos conforme suas características naturais."
        };

        private static string? GetCuriosity(string species) => species switch
        {
            "Samambaia" => "As samambaias existem há mais de 360 milhões de anos — são anteriores até aos dinossauros!",
            "Suculenta" => "O nome vem do latim sucus (seiva), referência à sua capacidade de armazenar água nos tecidos.",
            "Orquídea" => "Existem mais de 25.000 espécies de orquídeas — é a maior família de plantas com flores do mundo.",
            "Jiboia" => "A Jiboia pode purificar o ar removendo toxinas como formaldeído e benzeno de ambientes fechados.",
            _ => null
        };

        private static List<CareTip> GetTips(string species) => new()
    {
        new() { Icon = "💧", Title = "Rega",
            Description = species switch {
                "Samambaia" => "Mantenha o solo sempre úmido, mas sem encharcar. Regue 2-3x por semana.",
                "Suculenta"  => "Regue apenas quando o solo estiver completamente seco. A cada 10-14 dias no verão.",
                "Orquídea"   => "Regue semanalmente, permitindo que o substrato seque levemente entre as regas.",
                _ => "Regue conforme a necessidade, verificando a umidade do solo antes de cada rega."
            }
        },
        new() { Icon = "☀️", Title = "Luminosidade",
            Description = species switch {
                "Samambaia" => "Prefere luz indireta. Evite sol direto que queima as folhas.",
                "Suculenta"  => "Necessita de 4-6h de luz direta por dia. Coloque próximo a janelas ensolaradas.",
                "Orquídea"   => "Luz indireta brilhante. Nunca exposta ao sol direto.",
                _ => "Adapte a luminosidade conforme as necessidades específicas da espécie."
            }
        },
        new() { Icon = "🌡️", Title = "Temperatura",
            Description = "Mantenha em ambiente com temperatura estável, longe de correntes de ar frio e ar-condicionado." },
        new() { Icon = "✂️", Title = "Poda",
            Description = "Remova folhas secas ou danificadas com tesoura limpa para estimular o crescimento saudável." },
        new() { Icon = "🌿", Title = "Adubação",
            Description = "Adubar mensalmente na primavera e verão com fertilizante balanceado diluído à metade da dose." }
    };
    }

    // ===================== DIAGNÓSTICO IA =====================

    public class SimulatedAiDiagnosisService : IDiagnosisService
    {
        private readonly InMemoryDataStore _store;
        public SimulatedAiDiagnosisService(InMemoryDataStore store) => _store = store;

        public async Task<IADiagnosisResult> AnalyzeImageAsync(string plantId, byte[] imageBytes)
        {
            await Task.Delay(2500); // simula processamento da IA

            var plant = _store.Plants.FirstOrDefault(p => p.Id == plantId);
            var isHealthy = plant?.HealthStatus == PlantHealthStatus.Healthy;

            var diagnosis = new DiagnosisResult
            {
                Id = $"diag_{Guid.NewGuid():N}"[..12],
                PerformedAt = DateTime.Now,
                OverallObservation = isHealthy
                    ? "Planta com boa aparência geral. Nenhuma anomalia visual identificada."
                    : "Foram identificados sinais de estresse na planta. Veja os detalhes abaixo.",
                Issues = isHealthy ? new() : new List<DiagnosisIssue>
            {
                new()
                {
                    Name = "Estresse hídrico",
                    ConfidencePercent = 81,
                    Description = "Folhas com leve amarelamento nas bordas, indicando período de rega inadequada.",
                    Recommendations = new List<string>
                    {
                        "Verifique a umidade do solo antes de regar",
                        "Ajuste a frequência de rega conforme os parâmetros ideais da espécie"
                    }
                }
            }
            };

            // Persiste no histórico do store
            if (!_store.DiagnosisHistory.ContainsKey(plantId))
                _store.DiagnosisHistory[plantId] = new();

            _store.DiagnosisHistory[plantId].Insert(0, diagnosis);

            return IADiagnosisResult.Ok(diagnosis);
        }

        public async Task<List<DiagnosisResult>> GetDiagnosisHistoryAsync(string plantId)
        {
            await Task.Delay(400);
            return _store.DiagnosisHistory.GetValueOrDefault(plantId) ?? new();
        }
    }

    // ===================== PERFIL =====================

    public class SimulatedProfileService : IProfileService
    {
        private readonly InMemoryDataStore _store;
        public SimulatedProfileService(InMemoryDataStore store) => _store = store;

        public async Task<ProfileResult> GetProfileAsync()
        {
            await Task.Delay(400);
            var user = _store.CurrentUser;
            if (user is null) return ProfileResult.Fail("Usuário não encontrado.");

            return ProfileResult.Ok(new UserProfile
            {
                Name = user.Name,
                Email = user.Email,
                NotificationsEnabled = user.NotificationsEnabled,
                CriticalAlertsOnly = user.CriticalAlertsOnly
            });
        }

        public async Task<OperationResult> UpdateProfileAsync(UpdateProfileRequest request)
        {
            await Task.Delay(600);
            if (_store.CurrentUser is null) return OperationResult.Fail("Usuário não encontrado.");

            _store.CurrentUser.Name = request.Name;
            _store.CurrentUser.NotificationsEnabled = request.NotificationsEnabled;
            _store.CurrentUser.CriticalAlertsOnly = request.CriticalAlertsOnly;
            return OperationResult.Ok();
        }

        public async Task<OperationResult> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            await Task.Delay(700);
            if (_store.CurrentUser is null) return OperationResult.Fail("Usuário não encontrado.");

            if (_store.CurrentUser.Password != currentPassword)
                return OperationResult.Fail("Senha atual incorreta.");

            _store.CurrentUser.Password = newPassword;
            return OperationResult.Ok();
        }
    }
}
