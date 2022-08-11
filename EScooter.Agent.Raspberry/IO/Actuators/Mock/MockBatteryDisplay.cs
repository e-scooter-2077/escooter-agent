using EScooter.Agent.Raspberry.Model;

namespace EScooter.Agent.Raspberry.IO.Actuators.Mock;

public class MockBatteryDisplay : IBatteryDisplay
{
    private Fraction _lastValue;

    public void SetValue(Fraction value)
    {
        if (_lastValue == value)
        {
            return;
        }
        _lastValue = value;
        Console.WriteLine($"Battery level is now {value.Base100ValueRounded}%");
    }
}
