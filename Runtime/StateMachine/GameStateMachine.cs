using sozooo.GameStateMachine.StateInfrastructure;

#if ZENJECT
using Zenject;
#elif VCONTAINER
using VContainer.Unity;
#endif

namespace sozooo.GameStateMachine.StateMachine
{
    public class GameStateMachine : IGameStateMachine, ITickable
    {
        private readonly IStateMachine _stateMachine;
        
        public GameStateMachine(IStateMachine stateMachine) => 
            _stateMachine = stateMachine;

        public void Tick() => 
            _stateMachine.Tick();

        public void Enter<TState>() where TState : class, IState =>
            _stateMachine.Enter<TState>();

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadState<TPayload> =>
            _stateMachine.Enter<TState, TPayload>(payload);
    }
}