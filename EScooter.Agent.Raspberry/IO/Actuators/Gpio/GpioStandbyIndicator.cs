using EScooter.Agent.Raspberry.Model;
using System.Device.Gpio;

namespace EScooter.Agent.Raspberry.IO.Actuators.Gpio;

public class GpioStandbyIndicator : LedController, IStandbyIndicator
{
    public GpioStandbyIndicator(int pin, GpioController gpioController) : base(pin, gpioController)
    {
    }
}
