using EScooter.Agent.Raspberry.Model;

namespace EScooter.Agent.Raspberry.IO.Sensors.Mock;

public abstract class MockSensorBase<T> : ISensor<T>
{
    public Task Setup() => Task.CompletedTask;

    public Task<T> ReadValue() => throw new NotImplementedException();

    protected abstract T ReadValueInternal();
}
