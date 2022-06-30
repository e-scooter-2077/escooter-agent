using Geolocation;
using UnitsNet;

namespace EScooter.Agent.Raspberry.Model;

public record ScooterDevice(ISensor<Speed> Speedometer, ISensor<Coordinate> Gps, ISensor<Fraction> Battery, IActuator<bool> MagneticBrakes, IActuator<Speed> MaxSpeedEnforcer)
{
    public async Task Setup()
    {
        await Speedometer.Setup();
        await Gps.Setup();
        await Battery.Setup();
        await MagneticBrakes.Setup();
        await MaxSpeedEnforcer.Setup();
    }

    public async Task<ScooterSensorsState> ReadSensorsState()
    {
        var battery = await Battery.ReadValue();
        var position = await Gps.ReadValue();
        var speed = await Speedometer.ReadValue();

        return new ScooterSensorsState(battery, speed, position);
    }

    public async Task SetDesiredState(ScooterDesiredState desiredState)
    {
        await MagneticBrakes.SetValue(desiredState.Locked);
        await MaxSpeedEnforcer.SetValue(desiredState.MaxSpeed);
    }
}

public record ScooterSensorsState(Fraction BatteryLevel, Speed Speed, Coordinate Position);
