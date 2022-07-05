using EScooter.Agent.Raspberry.Model;
using UnitsNet;

namespace EScooter.Agent.Raspberry.IO.Actuators.Mock;

internal class MockMaxSpeedEnforcer : IActuator<Speed>
{
    public void SetValue(Speed value) => Console.WriteLine($"Setting Max Speed to {value.KilometersPerHour}km/h");
}