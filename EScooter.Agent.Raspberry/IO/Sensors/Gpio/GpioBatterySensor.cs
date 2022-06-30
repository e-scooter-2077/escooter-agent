using EScooter.Agent.Raspberry.Model;
using Iot.Device.Adc;

namespace EScooter.Agent.Raspberry.IO.Sensors.Gpio;

public class GpioBatterySensor : BaseMcp3xxxSensor<Fraction>
{
    public GpioBatterySensor(Mcp3xxx mcp, int mcpChannel) : base(mcp, mcpChannel)
    {
    }

    protected override Fraction ConvertRawValue(Fraction fraction) => fraction;
}
