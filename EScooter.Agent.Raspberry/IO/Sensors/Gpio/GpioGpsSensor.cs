using EScooter.Agent.Raspberry.Model;
using Geolocation;

namespace EScooter.Agent.Raspberry.IO.Sensors.Gpio;

public class GpioGpsSensor : ISensor<Coordinate>
{
    public Task Setup() => throw new NotImplementedException();

    public Task<Coordinate> ReadValue() => throw new NotImplementedException();
}
