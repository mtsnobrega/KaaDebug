using KaaDebug.Core.Models.Plants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaaDebug.Core.Interfaces.Plants
{
    public enum HistoryPeriod
    {
        Last24Hours,
        Last7Days,
        Last30Days
    }

    public class SensorHistory
    {
        public SensorType Type { get; set; }
        public string Unit { get; set; } = string.Empty;
        public IdealRange IdealRange { get; set; } = new();
        public List<SensorReadingPoint> Readings { get; set; } = new();

        public double? MinValue => Readings.Count > 0 ? Readings.Min(r => r.Value) : null;
        public double? MaxValue => Readings.Count > 0 ? Readings.Max(r => r.Value) : null;
        public double? AvgValue => Readings.Count > 0 ? Readings.Average(r => r.Value) : null;
    }

    public class PlantHistoryData
    {
        public string PlantName { get; set; } = string.Empty;
        public HistoryPeriod Period { get; set; }
        public List<SensorHistory> Sensors { get; set; } = new();
    }

    public class PlantHistoryResult
    {
        public bool Success { get; init; }
        public PlantHistoryData? Data { get; init; }
        public string? ErrorMessage { get; init; }

        public static PlantHistoryResult Ok(PlantHistoryData data) =>
            new() { Success = true, Data = data };
        public static PlantHistoryResult Fail(string message) =>
            new() { Success = false, ErrorMessage = message };
    }

    /// <summary>
    /// Abstração para obtenção do histórico completo de leituras de uma planta.
    /// A implementação real dependerá de um endpoint futuro:
    ///   GET /plants/{id}/history?period=24h|7d|30d
    /// </summary>
    public interface IPlantHistoryService
    {
        Task<PlantHistoryResult> GetHistoryAsync(string plantId, HistoryPeriod period);
    }
}
