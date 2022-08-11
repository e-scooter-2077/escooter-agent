using UnitsNet;

namespace EScooter.Agent.Raspberry.Model;

public interface IMagneticBrake : IActuator<bool>
{
}

public interface IMaxSpeedEnforcer : IActuator<Speed>
{
}

public interface ISpeedDisplay : IActuator<Speed>
{
}

public interface IBatteryDisplay : IActuator<Fraction>
{
}

public interface IStandbyIndicator : IActuator<bool>
{
}
