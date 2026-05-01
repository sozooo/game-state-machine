using sozooo.GameStateMachine.StateInfrastructure;

namespace sozooo.GameStateMachine.Factory
{
    public interface IStateFactory
    {
        T GetState<T>() where T : class, IExitableState;
    }
}