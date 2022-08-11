using EScooter.Agent.Raspberry.Model;
using Geolocation;

namespace EScooter.Agent.Raspberry.IO.Sensors.Mock;

public class MockGpsSensor : IGpsSensor
{
    public Coordinate ReadValue() => new(44.143043, 12.247474);
}
