﻿using EScooter.Agent.Raspberry.Model;
using System.Device.Gpio;

namespace EScooter.Agent.Raspberry.IO.Actuators.Gpio;

public class GpioMagneticBrake : IActuator<bool>
{
    private readonly int _pin;
    private readonly GpioController _gpioController;

    public GpioMagneticBrake(int pin, GpioController gpioController)
    {
        _pin = pin;
        _gpioController = gpioController;
        _gpioController.OpenPin(_pin, PinMode.Output);
    }

    public void SetValue(bool value) => _gpioController.Write(_pin, value ? PinValue.High : PinValue.Low);
}
