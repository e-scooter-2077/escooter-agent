using EScooter.Agent.Raspberry.Model;

namespace EScooter.Agent.Raspberry.IO.Sensors.Mock;

public class MockBatterySensor : IBatterySensor
{
    public static readonly TimeSpan TotalBatteryDuration = TimeSpan.FromMinutes(5); // Simulate a battery that runs out in 5 minutes.

    private readonly DateTime _created;

    public MockBatterySensor()
    {
        _created = DateTime.UtcNow;
    }

    public Fraction ReadValue()
    {
        var elapsed = DateTime.UtcNow - _created;
        var elapsedProportion = elapsed / TotalBatteryDuration;
        var remainingProportion = Math.Max(1 - elapsedProportion, 0);
        return Fraction.FromFraction(remainingProportion);
    }
}
