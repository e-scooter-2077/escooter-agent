using EScooter.Agent.Raspberry.Model;

namespace EScooter.Agent.Raspberry.IO.Sensors.Mock;

public class MockBatterySensor : MockSensorBase<Fraction>
{
    protected override Fraction ReadValueInternal() => Fraction.FromPercentage(100);
}
