using EScooter.Agent.Raspberry.Model;
using System.Device.Gpio;
using UnitsNet;

namespace EScooter.Agent.Raspberry.IO.Sensors.Gpio;

public class GpioSpeedometer : ISensor<Speed>
{
    private readonly GpioController _gpioController;

    public GpioSpeedometer(GpioController gpioController)
    {
        _gpioController = gpioController;
    }

    public Task Setup() => throw new NotImplementedException();

    public Task<Speed> ReadValue() => throw new NotImplementedException();
}
