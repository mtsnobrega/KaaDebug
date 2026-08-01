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
    /// <summary>
    /// Popula o InMemoryDataStore com um cenário completo e realista,
    /// simulando o ciclo de vida de um usuário desde o cadastro até o
    /// monitoramento ativo de múltiplas plantas com dispositivos IoT.
    ///
    /// O cenário cobre:
    ///   ✅ Usuário cadastrado e autenticado
    ///   ✅ 4 plantas com espécies diferentes
    ///   ✅ 3 dispositivos (1 online, 1 offline, 1 não associado)
    ///   ✅ 30 dias de histórico de leituras por sensor por planta
    ///   ✅ Status variados: saudável, atenção, crítico
    ///   ✅ Notificações em todos os níveis de prioridade (lidas e não lidas)
    ///   ✅ Histórico de diagnósticos de IA
    ///   ✅ Uma planta SEM dispositivo (para testar o fluxo de associação)
    ///
    /// USO: chamar Initialize() uma vez, no MauiProgram.cs,
    /// antes de registrar os SimulatedXxxServices no DI.
    /// </summary>
    public class AppScenarioSimulator
    {
        private readonly InMemoryDataStore _store;
        private readonly Random _random = new(123); // seed fixa = dados consistentes

        public AppScenarioSimulator(InMemoryDataStore store)
        {
            _store = store;
        }

        public void Initialize()
        {
            SetupUser();
            SetupSpecies();
            SetupDevices();
            SetupPlants();
            SetupSensorReadings();
            SetupNotifications();
            SetupDiagnosisHistory();
        }

        // ===================== USUÁRIO =====================

        private void SetupUser()
        {
            _store.CurrentUser = new SimulatedUser
            {
                Id = "u1",
                Name = "Ana Paula",
                Email = "ana@Budflow.com",
                Password = "123456",
                NotificationsEnabled = true,
                CriticalAlertsOnly = false
            };

            // Simula sessão já autenticada (Splash Screen → Dashboard direto)
            _store.IsAuthenticated = true;
            _store.SessionToken = GenerateFakeJwt();
        }

        // ===================== ESPÉCIES =====================

        private void SetupSpecies()
        {
            _store.Species = new List<PlantSpecies>
        {
            new()
            {
                Id = "s1", Name = "Samambaia", PhotoUrl = "samambaia.jpg",
                IdealParameters = new SpeciesIdealParameters
                {
                    SoilMoisture = new IdealRange { Min = 60, Max = 80, Unit = "%" },
                    AirHumidity  = new IdealRange { Min = 50, Max = 70, Unit = "%" },
                    Temperature  = new IdealRange { Min = 18, Max = 26, Unit = "°C" },
                    Luminosity   = new IdealRange { Min = 200, Max = 800, Unit = "lux" }
                }
            },
            new()
            {
                Id = "s2", Name = "Suculenta", PhotoUrl = "suculenta.jpg",
                IdealParameters = new SpeciesIdealParameters
                {
                    SoilMoisture = new IdealRange { Min = 10, Max = 30, Unit = "%" },
                    AirHumidity  = new IdealRange { Min = 20, Max = 40, Unit = "%" },
                    Temperature  = new IdealRange { Min = 18, Max = 30, Unit = "°C" },
                    Luminosity   = new IdealRange { Min = 800, Max = 2000, Unit = "lux" }
                }
            },
            new()
            {
                Id = "s3", Name = "Orquídea", PhotoUrl = "orquidea.jpg",
                IdealParameters = new SpeciesIdealParameters
                {
                    SoilMoisture = new IdealRange { Min = 40, Max = 60, Unit = "%" },
                    AirHumidity  = new IdealRange { Min = 60, Max = 80, Unit = "%" },
                    Temperature  = new IdealRange { Min = 18, Max = 24, Unit = "°C" },
                    Luminosity   = new IdealRange { Min = 150, Max = 500, Unit = "lux" }
                }
            },
            new()
            {
                Id = "s4", Name = "Jiboia", PhotoUrl = "jiboia.png",
                IdealParameters = new SpeciesIdealParameters
                {
                    SoilMoisture = new IdealRange { Min = 35, Max = 55, Unit = "%" },
                    AirHumidity  = new IdealRange { Min = 40, Max = 60, Unit = "%" },
                    Temperature  = new IdealRange { Min = 18, Max = 28, Unit = "°C" },
                    Luminosity   = new IdealRange { Min = 150, Max = 600, Unit = "lux" }
                }
            },
            new()
            {
                Id = "s5", Name = "Espada-de-São-Jorge", PhotoUrl = "espada_sj.png",
                IdealParameters = new SpeciesIdealParameters
                {
                    SoilMoisture = new IdealRange { Min = 20, Max = 40, Unit = "%" },
                    AirHumidity  = new IdealRange { Min = 30, Max = 50, Unit = "%" },
                    Temperature  = new IdealRange { Min = 16, Max = 30, Unit = "°C" },
                    Luminosity   = new IdealRange { Min = 100, Max = 1000, Unit = "lux" }
                }
            }
        };
        }

        // ===================== DISPOSITIVOS =====================

        private void SetupDevices()
        {
            _store.Devices = new List<SimulatedDevice>
        {
            new()
            {
                Id = "d1", Code = "ESP32-0001",
                ConnectionStatus = DeviceConnectionStatus.Online,
                LastHeartbeatAt = DateTime.Now.AddMinutes(-3)
            },
            new()
            {
                Id = "d2", Code = "ESP32-0002",
                ConnectionStatus = DeviceConnectionStatus.Offline,
                LastHeartbeatAt = DateTime.Now.AddHours(-26)
            },
            new()
            {
                Id = "d3", Code = "ESP32-0003",
                ConnectionStatus = DeviceConnectionStatus.Online,
                LastHeartbeatAt = DateTime.Now.AddMinutes(-11)
            }
            // d4: intencionalmente sem dispositivo (planta p4)
        };
        }

        // ===================== PLANTAS =====================

        private void SetupPlants()
        {
            _store.Plants = new List<SimulatedPlant>
        {
            // p1: SAUDÁVEL - Samambaia com dispositivo online
            new()
            {
                Id = "p1", Name = "Samambaia da Sala",
                SpeciesId = "s1", SpeciesName = "Samambaia",
                PhotoUrl = "samambaia.jpg",
                DeviceId = "d1",
                HealthStatus = PlantHealthStatus.Healthy,
                CreatedAt = DateTime.Now.AddDays(-45)
            },

            // p2: ATENÇÃO - Suculenta com solo secando
            new()
            {
                Id = "p2", Name = "Suculenta da Janela",
                SpeciesId = "s2", SpeciesName = "Suculenta",
                PhotoUrl = "suculenta.jpg",
                DeviceId = "d3",
                HealthStatus = PlantHealthStatus.Attention,
                StatusReason = "Solo seco há 2 dias",
                CreatedAt = DateTime.Now.AddDays(-30)
            },

            // p3: CRÍTICO - Orquídea com dispositivo offline
            new()
            {
                Id = "p3", Name = "Orquídea da Varanda",
                SpeciesId = "s3", SpeciesName = "Orquídea",
                PhotoUrl = "orquidea.jpg",
                DeviceId = "d2",
                HealthStatus = PlantHealthStatus.Critical,
                StatusReason = "Dispositivo offline há 26h",
                CreatedAt = DateTime.Now.AddDays(-60)
            },

            // p4: SEM DISPOSITIVO - para testar o fluxo de associação de ESP32
            new()
            {
                Id = "p4", Name = "Jiboia da Cozinha",
                SpeciesId = "s4", SpeciesName = "Jiboia",
                PhotoUrl = "jiboia.png",
                DeviceId = null,
                HealthStatus = PlantHealthStatus.Healthy,
                CreatedAt = DateTime.Now.AddDays(-7)
            }
        };

            // Associa dispositivos às plantas
            _store.Devices.First(d => d.Id == "d1").AssociatedPlantId = "p1";
            _store.Devices.First(d => d.Id == "d2").AssociatedPlantId = "p3";
            _store.Devices.First(d => d.Id == "d3").AssociatedPlantId = "p2";
        }

        // ===================== LEITURAS DOS SENSORES =====================

        private void SetupSensorReadings()
        {
            // p1: Samambaia - todos dentro do ideal
            _store.SensorReadings["p1"] = GeneratePlantReadings("p1",
                soilBase: 70, soilVariance: 5,    // dentro: 60-80%
                airBase: 60, airVariance: 4,     // dentro: 50-70%
                tempBase: 22, tempVariance: 2,    // dentro: 18-26°C
                luxBase: 500, luxVariance: 80);   // dentro: 200-800lux

            // p2: Suculenta - solo secando (abaixo do ideal)
            _store.SensorReadings["p2"] = GeneratePlantReadings("p2",
                soilBase: 8, soilVariance: 3,   // ABAIXO do ideal: 10-30%
                airBase: 30, airVariance: 3,    // dentro: 20-40%
                tempBase: 25, tempVariance: 3,   // dentro: 18-30°C
                luxBase: 1200, luxVariance: 200); // dentro: 800-2000lux

            // p3: Orquídea - dispositivo offline (sem leituras recentes)
            _store.SensorReadings["p3"] = GeneratePlantReadings("p3",
                soilBase: 50, soilVariance: 2,
                airBase: 70, airVariance: 3,
                tempBase: 22, tempVariance: 1,
                luxBase: 300, luxVariance: 50,
                stopReadingsHoursAgo: 26);        // para simular o offline

            // p4: Jiboia - sem dispositivo, sem leituras
            _store.SensorReadings["p4"] = new List<SimulatedSensorReading>();
        }

        private List<SimulatedSensorReading> GeneratePlantReadings(
            string plantId,
            double soilBase, double soilVariance,
            double airBase, double airVariance,
            double tempBase, double tempVariance,
            double luxBase, double luxVariance,
            int stopReadingsHoursAgo = 0)
        {
            var readings = new List<SimulatedSensorReading>();
            var now = DateTime.Now;
            var totalHours = 30 * 24; // 30 dias de histórico

            for (int hoursAgo = totalHours; hoursAgo >= stopReadingsHoursAgo; hoursAgo--)
            {
                var timestamp = now.AddHours(-hoursAgo);

                readings.Add(BuildReading(plantId, SensorType.SoilMoisture, soilBase, soilVariance, timestamp));
                readings.Add(BuildReading(plantId, SensorType.AirHumidity, airBase, airVariance, timestamp));
                readings.Add(BuildReading(plantId, SensorType.Temperature, tempBase, tempVariance, timestamp));
                readings.Add(BuildReading(plantId, SensorType.Luminosity, luxBase, luxVariance, timestamp));
            }

            return readings;
        }

        private SimulatedSensorReading BuildReading(
            string plantId, SensorType type,
            double baseValue, double variance, DateTime timestamp)
        {
            var noise = (_random.NextDouble() - 0.5) * 2 * variance;
            return new SimulatedSensorReading
            {
                PlantId = plantId,
                SensorType = type,
                Value = Math.Round(Math.Max(0, baseValue + noise), 1),
                Timestamp = timestamp
            };
        }

        // ===================== NOTIFICAÇÕES =====================

        private void SetupNotifications()
        {
            _store.Notifications = new List<SimulatedNotification>
        {
            // Críticas (não lidas)
            new() { Id = "n1", PlantId = "p3", PlantName = "Orquídea da Varanda",
                Message = "Dispositivo offline há mais de 24h",
                Priority = NotificationPriority.High,
                CreatedAt = DateTime.Now.AddHours(-2), IsRead = false },

            new() { Id = "n2", PlantId = "p2", PlantName = "Suculenta da Janela",
                Message = "Umidade do solo abaixo do ideal",
                Priority = NotificationPriority.Medium,
                CreatedAt = DateTime.Now.AddHours(-6), IsRead = false },

            // Informativas (não lidas)
            new() { Id = "n3", PlantId = "p1", PlantName = "Samambaia da Sala",
                Message = "Leitura de sensores normalizada",
                Priority = NotificationPriority.Low,
                CreatedAt = DateTime.Now.AddHours(-10), IsRead = false },

            // Lidas (histórico)
            new() { Id = "n4", PlantId = "p2", PlantName = "Suculenta da Janela",
                Message = "Solo começando a secar — monitore a rega",
                Priority = NotificationPriority.Medium,
                CreatedAt = DateTime.Now.AddDays(-1), IsRead = true },

            new() { Id = "n5", PlantId = "p3", PlantName = "Orquídea da Varanda",
                Message = "Temperatura dentro do ideal após ventilação",
                Priority = NotificationPriority.Low,
                CreatedAt = DateTime.Now.AddDays(-2), IsRead = true },

            new() { Id = "n6", PlantId = "p1", PlantName = "Samambaia da Sala",
                Message = "Luminosidade abaixo do ideal, considere mudar o local",
                Priority = NotificationPriority.Medium,
                CreatedAt = DateTime.Now.AddDays(-3), IsRead = true },

            new() { Id = "n7", PlantId = "p3", PlantName = "Orquídea da Varanda",
                Message = "Dispositivo ESP32-0002 reconectado",
                Priority = NotificationPriority.Low,
                CreatedAt = DateTime.Now.AddDays(-5), IsRead = true }
        };
        }

        // ===================== DIAGNÓSTICOS =====================

        private void SetupDiagnosisHistory()
        {
            // Histórico de diagnósticos da Suculenta (p2)
            _store.DiagnosisHistory["p2"] = new List<DiagnosisResult>
        {
            new()
            {
                Id = "diag1",
                PerformedAt = DateTime.Now.AddDays(-3),
                OverallObservation = "Planta com boa aparência geral. Folhas firmes e coloração uniforme.",
                Issues = new List<DiagnosisIssue>()  // saudável
            },
            new()
            {
                Id = "diag2",
                PerformedAt = DateTime.Now.AddDays(-15),
                OverallObservation = "Sinais leves de desidratação nas bordas das folhas.",
                Issues = new List<DiagnosisIssue>
                {
                    new()
                    {
                        Name = "Estresse hídrico leve",
                        ConfidencePercent = 72,
                        Description = "Bordas levemente amareladas sugerem período sem rega adequada.",
                        Recommendations = new List<string>
                        {
                            "Verifique a frequência de rega",
                            "Certifique-se de que o solo seca completamente entre as regas"
                        }
                    }
                }
            }
        };

            // Histórico da Samambaia (p1)
            _store.DiagnosisHistory["p1"] = new List<DiagnosisResult>
        {
            new()
            {
                Id = "diag3",
                PerformedAt = DateTime.Now.AddDays(-10),
                OverallObservation = "Planta saudável, crescimento ativo visível.",
                Issues = new List<DiagnosisIssue>()
            }
        };
        }

        // ===================== UTILITÁRIOS =====================

        /// <summary>
        /// JWT fake com expiração em 2099 — suficiente para que o AuthService
        /// local valide como "sessão ativa" e dirija o usuário ao Dashboard.
        /// Não é um token real assinado; serve apenas para o fluxo da Splash.
        /// </summary>
        private static string GenerateFakeJwt()
        {
            // Header.Payload.Signature (todos em base64url falso mas legível pelo JwtSecurityTokenHandler)
            // exp = 4102444800 = 2099-12-31
            return
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9." +
                "eyJzdWIiOiJ1MSIsIm5hbWUiOiJBbmEgUGF1bGEiLCJleHAiOjQxMDI0NDQ4MDB9." +
                "simulation_signature_not_real";
        }
    }
}
