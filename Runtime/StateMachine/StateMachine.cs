using RSG;
using sozooo.GameStateMachine.Factory;
using sozooo.GameStateMachine.StateInfrastructure;

namespace sozooo.GameStateMachine.StateMachine
{
    public class StateMachine : IStateMachine
    {
        private IExitableState _activeState;
        private readonly IStateFactory _stateFactory;

        public StateMachine(IStateFactory stateFactory) => 
            _stateFactory = stateFactory;

        public void Tick()
        {
            if (_activeState is IUpdateable updateableState)
                updateableState.Update();
        }

        public void Enter<TState>() where TState : class, IState =>
            RequestEnter<TState>(null).Done();

        public void Enter<TState>(object[] args) where TState : class, IState =>
            RequestEnter<TState>(args).Done();

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadState<TPayload> =>
            RequestEnter<TState, TPayload>(payload).Done();

        private IPromise<TState> RequestEnter<TState>(object[] args) where TState : class, IState =>
            RequestChangeState<TState>(args)
                .Then(EnterState);

        private IPromise<TState> RequestEnter<TState, TPayload>(TPayload payload) where TState : class, IPayloadState<TPayload> =>
            RequestChangeState<TState>(null)
                .Then(state => EnterPayloadState(state, payload));

        private TState EnterState<TState>(TState state) where TState : class, IState
        {
            _activeState = state;
            state.Enter();
            return state;
        }

        private TState EnterPayloadState<TState, TPayload>(TState state, TPayload payload) where TState : class, IPayloadState<TPayload>
        {
            _activeState = state;
            state.Enter(payload);
            return state;
        }

        private IPromise<TState> RequestChangeState<TState>(object[] args) where TState : class, IExitableState
        {
            if (_activeState != null)
            {
                return _activeState
                    .BeginExit()
                    .Then(_activeState.EndExit)
                    .Then(() => ChangeState<TState>(args));
            }

            return ChangeState<TState>(args);
        }

        private IPromise<TState> ChangeState<TState>(object[] args) where TState : class, IExitableState
        {
            TState state = args != null
                ? _stateFactory.GetState<TState>(args)
                : _stateFactory.GetState<TState>();

            return Promise<TState>.Resolved(state);
        }
    }
}
