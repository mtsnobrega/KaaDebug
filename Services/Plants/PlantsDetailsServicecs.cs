using KaaDebug.Core.Interfaces.Plants;
using KaaDebug.Core.Models.Dashboard;
using KaaDebug.Core.Models.Plants;
using KaaDebug.Infrastructure.http;

namespace KaaDebug.Services.Plants;
public class PlantDetailsService : IPlantDetailsService
{
    private readonly ApiClient _apiClient;

    public PlantDetailsService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<PlantDetailsResult> GetPlantDetailsAsync(string plantId)
    {
        var result = await _apiClient.GetAsync<PlantDetailsDto>(
            ApiConstants.Plants.GetDetails(Guid.Parse(plantId)));

        if (!result.Success)
            return PlantDetailsResult.Fail(result.ErrorMessage!);

        var dto = result.Data!;
        var details = new PlantDetails
        {
            Id = dto.Id.ToString(),
            Name = dto.Name,
            Species = dto.Species,
            PhotoUrl = dto.PhotoUrl,
            HealthStatus = PlantsUserService.ParseHealthStatus(dto.HealthStatus),
            StatusReason = dto.StatusReason,
            Device = new DeviceInformation
            {
                DeviceCode = dto.Device.DeviceCode,
                ConnectionStatus = ParseConnectionStatus(dto.Device.ConnectionStatus),
                LastReadingAt = dto.Device.LastReadingAt
            },
            Indicators = dto.Indicators.Select(MapIndicator).ToList(),
            RelatedNotifications = dto.RecentNotifications.Select(MapNotification).ToList()
        };

        return PlantDetailsResult.Ok(details);
    }

    private static SensorIndicator MapIndicator(SensorIndicatorDto dto) => new()
    {
        Type = ParseSensorType(dto.SensorType),
        CurrentValue = dto.CurrentValue,
        Unit = dto.Unit,
        IdealRange = new IdealRange
        {
            Min = (double)dto.IdealRange.Min,
            Max = (double)dto.IdealRange.Max,
            Unit = dto.IdealRange.Unit
        },
        IsWithinIdealRange = dto.IsWithinIdealRange,
        RecentHistory = dto.RecentHistory.Select(r =>
            new SensorReadingPoint { Timestamp = r.Timestamp, Value = r.Value }).ToList()
    };

    private static NotificationSummary MapNotification(NotificationDto dto) => new()
    {
        Id = dto.Id.ToString(),
        PlantId = dto.PlantId.ToString(),
        PlantName = dto.PlantName,
        Message = dto.Message,
        Priority = ParsePriority(dto.Priority),
        IsRead = dto.IsRead,
        CreatedAt = dto.CreatedAt
    };

    private static DeviceConnectionStatus ParseConnectionStatus(string value) =>
        value.ToUpper() switch
        {
            "ONLINE" => DeviceConnectionStatus.Online,
            "OFFLINE" => DeviceConnectionStatus.Offline,
            "ASSOCIATED" => DeviceConnectionStatus.Associated,
            _ => DeviceConnectionStatus.NotAssociated
        };

    private static SensorType ParseSensorType(string value) =>
        value.ToUpper() switch
        {
            "AIR_HUMIDITY" => SensorType.AirHumidity,
            "TEMPERATURE" => SensorType.Temperature,
            "LUMINOSITY" => SensorType.Luminosity,
            _ => SensorType.SoilMoisture
        };

    private static NotificationPriority ParsePriority(string value) =>
        value.ToUpper() switch
        {
            "HIGH" => NotificationPriority.High,
            "MEDIUM" => NotificationPriority.Medium,
            _ => NotificationPriority.Low
        };
}
