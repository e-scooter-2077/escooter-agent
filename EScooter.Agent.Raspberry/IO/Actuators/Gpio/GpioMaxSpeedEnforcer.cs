using EScooter.Agent.Raspberry.Model;
using UnitsNet;

namespace EScooter.Agent.Raspberry.IO.Actuators.Gpio;

public class GpioMaxSpeedEnforcer : IMaxSpeedEnforcer
{
    private readonly SpeedLcd _speedDisplay;

    public GpioMaxSpeedEnforcer(SpeedLcd speedDisplay)
    {
        _speedDisplay = speedDisplay;
    }

    public void SetValue(Speed speed) => _speedDisplay.SetMaxSpeed(speed);
}
