using EScooter.Agent.Raspberry.Model;

namespace EScooter.Agent.Raspberry.Dto;

public record ScooterReportedDto(
    bool Locked,
    bool Standby,
    double MaxSpeed,
    TimeSpan UpdateFrequency)
{
    public static ScooterReportedDto FromReportedState(ScooterReportedState state) => new(
        Locked: state.Locked,
        Standby: state.Standby,
        MaxSpeed: state.MaxSpeed.MetersPerSecond,
        UpdateFrequency: state.UpdateFrequency);
}
