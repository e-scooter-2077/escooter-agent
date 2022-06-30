using EScooter.Agent.Raspberry.Model;
using System.Device.Gpio;

namespace EScooter.Agent.Raspberry.IO.Sensors.Gpio;

public class GpioBatterySensor : ISensor<Fraction>
{
    private readonly GpioController _gpioController;

    public GpioBatterySensor(GpioController gpioController)
    {
        _gpioController = gpioController;
    }

    public Task Setup() => throw new NotImplementedException();

    public Task<Fraction> ReadValue() => throw new NotImplementedException();
}
