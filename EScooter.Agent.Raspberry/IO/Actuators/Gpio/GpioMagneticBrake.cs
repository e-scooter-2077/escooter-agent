using EScooter.Agent.Raspberry.Model;
using System.Device.Gpio;

namespace EScooter.Agent.Raspberry.IO.Actuators.Gpio;

public class GpioMagneticBrake : LedController, IMagneticBrake
{
    public GpioMagneticBrake(int pin, GpioController gpioController) : base(pin, gpioController)
    {
    }
}
