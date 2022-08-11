using EScooter.Agent.Raspberry.Model;

namespace EScooter.Agent.Raspberry.IO.Actuators.Gpio;

public class GpioBatteryDisplay : IBatteryDisplay
{
    private readonly LcdDisplay _lcd;

    public GpioBatteryDisplay(LcdDisplay lcd)
    {
        _lcd = lcd;
    }

    public void SetValue(Fraction value) => _lcd.SetBatteryLevel(value);
}
