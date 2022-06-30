using EScooter.Agent.Raspberry.Model;
using Iot.Device.Adc;
using UnitsNet;

namespace EScooter.Agent.Raspberry.IO.Sensors.Gpio;

public class GpioSpeedometer : BaseMcp3xxxSensor<Speed>
{
    private static readonly Speed _theoreticalMaxSpeed = Speed.FromKilometersPerHour(40);

    public GpioSpeedometer(Mcp3xxx mcp, int mcpChannel) : base(mcp, mcpChannel)
    {
    }

    protected override Speed ConvertRawValue(Fraction fraction) => _theoreticalMaxSpeed * fraction.Base1Value;
}
