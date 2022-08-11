using EScooter.Agent.Raspberry.Model;
using UnitsNet;

namespace EScooter.Agent.Raspberry.IO.Actuators.Gpio;

public class GpioSpeedDisplay : ISpeedDisplay
{
    private readonly SpeedLcd _speedLcd;

    public GpioSpeedDisplay(SpeedLcd speedLcd)
    {
        _speedLcd = speedLcd;
    }

    public void SetValue(Speed value) => _speedLcd.SetCurrentSpeed(value);
}
