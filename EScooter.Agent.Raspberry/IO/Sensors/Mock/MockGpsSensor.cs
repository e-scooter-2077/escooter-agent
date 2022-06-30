using Geolocation;

namespace EScooter.Agent.Raspberry.IO.Sensors.Mock;

public class MockGpsSensor : MockSensorBase<Coordinate>
{
    protected override Coordinate ReadValueInternal() => new Coordinate(44.143043, 12.247474);
}
