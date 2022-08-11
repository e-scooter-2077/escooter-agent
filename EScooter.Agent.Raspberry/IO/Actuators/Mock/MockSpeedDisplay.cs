using EScooter.Agent.Raspberry.Model;
using UnitsNet;

namespace EScooter.Agent.Raspberry.IO.Actuators.Mock;

public class MockSpeedDisplay : ISpeedDisplay
{
    private Speed _lastValue;

    public void SetValue(Speed value)
    {
        if (_lastValue == value)
        {
            return;
        }
        _lastValue = value;
        Console.WriteLine($"Current speed is {value.KilometersPerHour}km/h");
    }
}
