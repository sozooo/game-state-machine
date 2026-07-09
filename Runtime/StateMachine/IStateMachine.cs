using sozooo.GameStateMachine.StateInfrastructure;

namespace sozooo.GameStateMachine.StateMachine
{
    public interface IStateMachine
    {
        void Enter<TState>() where TState : class, IState;
        void Enter<TState>(object[] args) where TState : class, IState;
        void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadState<TPayload>;
        void Tick();
    }
}
