namespace EScooter.Agent.Raspberry.Model;

public interface IActuator<T>
{
    void SetValue(T value);
}
