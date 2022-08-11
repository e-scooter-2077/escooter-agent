using Iot.Device.CharacterLcd;
using UnitsNet;

namespace EScooter.Agent.Raspberry.IO.Actuators.Gpio;

public class SpeedLcd
{
    private readonly Lcd1602 _lcd;
    private readonly int _row;

    private int _roundedCurrentSpeed;
    private int _roundedMaxSpeed;

    public SpeedLcd(Lcd1602 lcd, int row)
    {
        _lcd = lcd;
        _row = row;
    }

    public void SetMaxSpeed(Speed speed)
    {
        _roundedMaxSpeed = RoundSpeed(speed);
        UpdateDisplay();
    }

    public void SetCurrentSpeed(Speed speed)
    {
        _roundedCurrentSpeed = RoundSpeed(speed);
        UpdateDisplay();
    }

    private static int RoundSpeed(Speed speed) => (int)Math.Round(speed.KilometersPerHour);

    private void UpdateDisplay()
    {
        _lcd.SetCursorPosition(0, _row);

        _lcd.Write($"{_roundedCurrentSpeed:2}/{_roundedMaxSpeed:2} km/h");
    }
}
