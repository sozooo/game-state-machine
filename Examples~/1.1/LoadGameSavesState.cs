using sozooo.GameStateMachine.StateInfrastructure;
using sozooo.GameStateMachine.StateMachine;

namespace Code.Infrastructure.States.GameStates
{
    public class LoadGameSavesState : SimpleState
    {
        private readonly IGameStateMachine _stateMachine;

        public LoadGameSavesState(IGameStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public override void Enter()
        {
            //base.Enter(); is empty no need to implement
            //LOAD SAVES
            
            _stateMachine.Enter<LoadHomeScreenState>();
        }
    }
}