using EScooter.Agent.Raspberry.Model;

namespace EScooter.Agent.Raspberry.IO.Actuators.Mock;

public class MockStandbyIndicator : IStandbyIndicator
{
    private bool _lastValue;

    public void SetValue(bool value)
    {
        if (_lastValue == value)
        {
            return;
        }
        _lastValue = value;
        Console.WriteLine(value ? "Entering standby mode" : "Leaving standby mode");
    }
}
