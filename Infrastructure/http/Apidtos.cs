/*
 * Responsabilidade:
 * Definir os Data Transfer Objects (DTOs) que representam as cargas úteis (payloads) 
 * literais enviadas e recebidas da API.
 * 
 * Papel na arquitetura:
 * Camada de Contrato de Rede. A utilização de "records" garante a imutabilidade 
 * dos dados no trânsito entre a rede e a conversão para os Modelos de Domínio (Core). 
 * Protege o aplicativo contra mudanças acidentais de estado.
 */

namespace KaaDebug.Infrastructure.http
{
    // ── Auth ──────────────────────────────────────────────────────────────────────

    public record LoginResponseDto(
        string Token,
        DateTime ExpiresAt,
        string Name,
        string Email);

    // ── Dashboard ─────────────────────────────────────────────────────────────────

    public record DashboardDto(
        string UserFirstName,
        int TotalPlants,
        int ActiveAlertsCount,
        List<PlantSummaryDto> RecentPlants,
        List<NotificationDto> RecentNotifications);

    // ── Plants ────────────────────────────────────────────────────────────────────

    public record PlantSummaryDto(
        Guid Id,
        string Name,
        string Species,
        string? PhotoUrl,
        string HealthStatus,
        string? StatusReason);

    public record PlantDetailsDto(
        Guid Id,
        string Name,
        string Species,
        string? PhotoUrl,
        string HealthStatus,
        string? StatusReason,
        DeviceStatusDto Device,
        List<SensorIndicatorDto> Indicators,
        List<NotificationDto> RecentNotifications);

    public record DeviceStatusDto(
        string? DeviceCode,
        string ConnectionStatus,
        DateTime? LastReadingAt);

    public record SensorIndicatorDto(
        string SensorType,
        double CurrentValue,
        string Unit,
        IdealRangeDto IdealRange,
        bool IsWithinIdealRange,
        List<SensorReadingPointDto> RecentHistory);

    public record IdealRangeDto(decimal Min, decimal Max, string Unit);

    public record SensorReadingPointDto(DateTime Timestamp, double Value);

    // ── History ───────────────────────────────────────────────────────────────────

    public record PlantHistoryDto(
        string PlantName,
        string Period,
        List<SensorHistoryDto> Sensors);

    public record SensorHistoryDto(
        string SensorType,
        string Unit,
        IdealRangeDto IdealRange,
        double? MinValue,
        double? MaxValue,
        double? AvgValue,
        List<SensorReadingPointDto> Readings);

    // ── Species ───────────────────────────────────────────────────────────────────

    public record SpeciesDto(
        Guid Id,
        string Name,
        string? PhotoUrl,
        IdealRangeDto SoilMoisture,
        IdealRangeDto AirHumidity,
        IdealRangeDto Temperature,
        IdealRangeDto Luminosity);

    // ── Notifications ─────────────────────────────────────────────────────────────

    public record NotificationDto(
        Guid Id,
        Guid PlantId,
        string PlantName,
        string Message,
        string Priority,
        bool IsRead,
        DateTime CreatedAt);

    // ── Diagnosis ─────────────────────────────────────────────────────────────────

    public record DiagnosisResultDto(
        Guid Id,
        bool IsHealthy,
        string? OverallObservation,
        List<DiagnosisIssueDto> Issues,
        DateTime PerformedAt);

    public record DiagnosisIssueDto(
        string Name,
        int ConfidencePercent,
        string Description,
        List<string> Recommendations);

    // ── Device ────────────────────────────────────────────────────────────────────

    public record DeviceVerificationDto(
        string Code,
        string ConnectionStatus,
        DateTime? LastHeartbeatAt);

    // ── Profile ───────────────────────────────────────────────────────────────────

    public record ProfileDto(
        string Name,
        string Email,
        bool NotificationsEnabled,
        bool CriticalAlertsOnly);
}
