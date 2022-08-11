using Geolocation;
using UnitsNet;

namespace EScooter.Agent.Raspberry.Model;

public interface IGpsSensor : ISensor<Coordinate>
{
}

public interface IBatterySensor : ISensor<Fraction>
{
}

public interface ISpeedometer : ISensor<Speed>
{
}
