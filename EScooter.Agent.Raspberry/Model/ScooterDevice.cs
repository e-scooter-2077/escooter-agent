using Geolocation;
using UnitsNet;

namespace EScooter.Agent.Raspberry.Model;

public record ScooterDevice(ISensor<Speed> Speedometer, ISensor<Coordinate> Gps, ISensor<Fraction> Battery)
{
    public async Task Setup()
    {
        await Task.WhenAll(
            Speedometer.Setup(),
            Gps.Setup(),
            Battery.Setup());
    }

    public async Task<ScooterSensorsState> ReadSensorsState()
    {
        var batteryTask = Battery.ReadValue();
        var positionTask = Gps.ReadValue();
        var speedTask = Speedometer.ReadValue();

        await Task.WhenAll(batteryTask, positionTask, speedTask);

        return new ScooterSensorsState(batteryTask.Result, speedTask.Result, positionTask.Result);
    }
}

public record ScooterSensorsState(Fraction BatteryLevel, Speed Speed, Coordinate Position);
