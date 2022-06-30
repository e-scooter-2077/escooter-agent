namespace EScooter.Agent.Raspberry.Model;

public interface IActuator<T>
{
    Task Setup();

    Task SetValue(T value);
}
