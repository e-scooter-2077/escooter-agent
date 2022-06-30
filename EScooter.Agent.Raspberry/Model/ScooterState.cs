using Geolocation;
using UnitsNet;

namespace EScooter.Agent.Raspberry.Model;

public record ScooterState(
    bool Locked,
    Fraction BatteryLevel,
    Fraction StandbyThreshold,
    Acceleration Acceleration,
    Speed Speed,
    Speed StandbyMaxSpeed,
    Speed DesiredMaxSpeed,
    Angle Direction,
    Coordinate Position,
    Length DistancePerBatteryPercent,
    Duration UpdateFrequency);
