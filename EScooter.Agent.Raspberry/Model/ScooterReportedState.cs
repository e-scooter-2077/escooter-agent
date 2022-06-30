using UnitsNet;

namespace EScooter.Agent.Raspberry.Model;

public record ScooterReportedState(
    bool Locked,
    bool Standby,
    Speed MaxSpeed,
    TimeSpan UpdateFrequency);
