using EScooter.Agent.Raspberry.Model;
using UnitsNet;

namespace EScooter.Agent.Raspberry.IO.Actuators.Gpio;

public class GpioSpeedDisplay : ISpeedDisplay
{
    private readonly LcdDisplay _lcd;

    public GpioSpeedDisplay(LcdDisplay lcd)
    {
        _lcd = lcd;
    }

    public void SetValue(Speed value) => _lcd.SetCurrentSpeed(value);
}
