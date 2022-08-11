using EScooter.Agent.Raspberry.Model;
using UnitsNet;

namespace EScooter.Agent.Raspberry.IO.Sensors.Mock;

public class MockSpeedometer : ISpeedometer
{
    public Speed ReadValue() => Speed.FromKilometersPerHour(20);
}
