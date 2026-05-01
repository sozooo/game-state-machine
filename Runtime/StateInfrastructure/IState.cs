namespace sozooo.GameStateMachine.StateInfrastructure
{
    public interface IState: IExitableState
    {
        void Enter();
    }
}