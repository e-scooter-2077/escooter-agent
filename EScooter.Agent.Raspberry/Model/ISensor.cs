namespace EScooter.Agent.Raspberry.Model;

public interface ISensor<T>
{
    T ReadValue();
}
