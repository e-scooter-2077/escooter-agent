using Geolocation;
using UnitsNet;

namespace EScooter.Agent.Raspberry.Model;

public record ScooterSensorsState(Fraction BatteryLevel, Speed Speed, Coordinate Position);
