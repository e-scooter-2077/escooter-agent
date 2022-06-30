using UnitsNet;

namespace EScooter.Agent.Raspberry.IO.Sensors.Mock;

public class MockSpeedometer : MockSensorBase<Speed>
{
    protected override Speed ReadValueInternal() => Speed.FromKilometersPerHour(20);
}
