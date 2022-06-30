namespace EScooter.Agent.Raspberry.Model;

public class Scooter
{
    public ScooterState CurrentState { get; private set; }

    public Scooter(ScooterState initialState)
    {
        CurrentState = initialState;
    }

    public void UpdateState(Func<ScooterState, ScooterState> updateAction)
    {
        CurrentState = updateAction(CurrentState);
    }
}
