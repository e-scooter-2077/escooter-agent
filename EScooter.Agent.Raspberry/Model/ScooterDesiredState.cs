using UnitsNet;

namespace EScooter.Agent.Raspberry.Model;

public record ScooterDesiredState(
    bool Locked,
    TimeSpan UpdateFrequency,
    Speed MaxSpeed);
