using EScooter.Agent.Raspberry.Model;

namespace EScooter.Agent.Raspberry.Dto;

public record ScooterTelemetryDto(
    double? BatteryLevel,
    double? Speed,
    double? Latitude,
    double? Longitude)
{
    public static ScooterTelemetryDto FromSensorsState(ScooterSensorsState sensorsState) => new(
        sensorsState.BatteryLevel.Base1Value,
        sensorsState.Speed.MetersPerSecond,
        sensorsState.Position.Latitude,
        sensorsState.Position.Longitude);
}
