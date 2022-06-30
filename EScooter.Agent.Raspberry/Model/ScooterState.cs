using Geolocation;
using UnitsNet;

namespace EScooter.Agent.Raspberry.Model;

public record ScooterState(
    bool Locked,
    Fraction BatteryLevel,
    Speed DesiredMaxSpeed,
    Speed Speed,
    Coordinate Position);
