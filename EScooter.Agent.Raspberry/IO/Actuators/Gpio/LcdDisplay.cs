using EScooter.Agent.Raspberry.Model;
using Iot.Device.CharacterLcd;
using UnitsNet;

namespace EScooter.Agent.Raspberry.IO.Actuators.Gpio;

public class LcdDisplay
{
    private const int BatteryRow = 0;
    private const int SpeedRow = 1;

    private readonly Lcd1602 _lcd;

    private int _roundedCurrentSpeed;
    private int _roundedMaxSpeed;

    public LcdDisplay(Lcd1602 lcd)
    {
        _lcd = lcd;

        _lcd.BlinkingCursorVisible = false;
        _lcd.UnderlineCursorVisible = false;
        _lcd.Clear();
    }

    public void SetMaxSpeed(Speed speed)
    {
        _roundedMaxSpeed = RoundSpeed(speed);
        UpdateSpeedDisplay();
    }

    public void SetCurrentSpeed(Speed speed)
    {
        _roundedCurrentSpeed = RoundSpeed(speed);
        UpdateSpeedDisplay();
    }

    private static int RoundSpeed(Speed speed) => (int)Math.Round(speed.KilometersPerHour);

    private void UpdateSpeedDisplay()
    {
        _lcd.SetCursorPosition(0, SpeedRow);
        _lcd.Write($"{_roundedCurrentSpeed,2}/{_roundedMaxSpeed,2}km/h");
    }

    public void SetBatteryLevel(Fraction batteryLevel)
    {
        _lcd.SetCursorPosition(0, BatteryRow);
        _lcd.Write($"{batteryLevel.Base100ValueRounded,3}%");
    }
}
