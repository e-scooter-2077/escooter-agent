using EScooter.Agent.Raspberry.Model;

namespace EScooter.Agent.Raspberry.IO.Actuators.Mock;

public class MockMagneticBrake : IActuator<bool>
{
    private bool _last = false;

    public void SetValue(bool value)
    {
        if (_last != value)
        {
            Console.WriteLine(value ? "Start breaking..." : "Stop Breaking");
            _last = value;
        }
    }
}
