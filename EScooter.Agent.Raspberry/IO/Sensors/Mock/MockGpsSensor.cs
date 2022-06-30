using EScooter.Agent.Raspberry.Model;
using Geolocation;

namespace EScooter.Agent.Raspberry.IO.Sensors.Mock;

public class MockGpsSensor : ISensor<Coordinate>
{
    public Coordinate ReadValue() => new Coordinate(44.143043, 12.247474);
}
