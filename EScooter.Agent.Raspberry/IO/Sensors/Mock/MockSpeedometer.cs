using EScooter.Agent.Raspberry.Model;
using UnitsNet;

namespace EScooter.Agent.Raspberry.IO.Sensors.Mock;

public class MockSpeedometer : ISensor<Speed>
{
    public Speed ReadValue() => Speed.FromKilometersPerHour(20);
}
