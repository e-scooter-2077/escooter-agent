using EScooter.Agent.Raspberry.Model;
using UnitsNet;

namespace EScooter.Agent.Raspberry.IO.Actuators.Gpio;

public class GpioMaxSpeedEnforcer : IMaxSpeedEnforcer
{
    private readonly LcdDisplay _lcd;

    public GpioMaxSpeedEnforcer(LcdDisplay lcd)
    {
        _lcd = lcd;
    }

    public void SetValue(Speed speed) => _lcd.SetMaxSpeed(speed);
}
