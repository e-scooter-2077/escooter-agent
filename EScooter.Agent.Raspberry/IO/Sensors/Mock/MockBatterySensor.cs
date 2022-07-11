using EScooter.Agent.Raspberry.Model;

namespace EScooter.Agent.Raspberry.IO.Sensors.Mock;

public class MockBatterySensor : ISensor<Fraction>
{
    public Fraction ReadValue() => Fraction.FromPercentage(40);
}
