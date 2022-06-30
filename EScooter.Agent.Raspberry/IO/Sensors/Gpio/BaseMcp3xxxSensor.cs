using EScooter.Agent.Raspberry.Model;
using Iot.Device.Adc;

namespace EScooter.Agent.Raspberry.IO.Sensors.Gpio;

public abstract class BaseMcp3xxxSensor<T> : ISensor<T>
{
    public const int MaxAdcValue = 1023;

    private readonly Mcp3xxx _mcp;
    private readonly int _mcpChannel;

    public BaseMcp3xxxSensor(Mcp3xxx mcp, int mcpChannel)
    {
        _mcp = mcp;
        _mcpChannel = mcpChannel;
    }

    public Task Setup() => Task.CompletedTask;

    public Task<T> ReadValue() => Task.Run(() => ConvertRawValue(Fraction.FromFraction(_mcp.Read(_mcpChannel) / (double)MaxAdcValue)));

    protected abstract T ConvertRawValue(Fraction fraction);
}
