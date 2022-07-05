using Geolocation;
using UnitsNet;

namespace EScooter.Agent.Raspberry.Model;

public record ScooterHardware(
    ISensor<Speed> Speedometer,
    ISensor<Coordinate> Gps,
    ISensor<Fraction> Battery,
    IActuator<bool> MagneticBrakes,
    IActuator<Speed> MaxSpeedEnforcer);
