using EScooter.Agent.Raspberry.Model;
using UnitsNet;

namespace EScooter.Agent.Raspberry.IO.Actuators.Gpio;

public class GpioMaxSpeedEnforcer : IActuator<Speed>
{
    public Task Setup() => Task.CompletedTask;

    public Task SetValue(Speed value) => Task.CompletedTask;
}
