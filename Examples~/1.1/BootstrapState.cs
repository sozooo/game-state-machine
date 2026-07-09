using sozooo.GameStateMachine.StateInfrastructure;
using sozooo.GameStateMachine.StateMachine;

namespace Code.Infrastructure.States.GameStates
{
    public class BootstrapState : SimpleState
    {
        private readonly IGameStateMachine _stateMachine;

        public BootstrapState(IGameStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public override void Enter()
        {
            //base.Enter() is empty no need to implement
            //SDK INIT / BOOTSTRAP REGISTERS / CONFIGS LOAD
            
            _stateMachine.Enter<LoadGameSavesState>();
        }
    }
}