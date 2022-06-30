namespace EScooter.Agent.Raspberry.Model;

public interface ISensor<T>
{
    Task Setup();

    Task<T> ReadValue();
}
