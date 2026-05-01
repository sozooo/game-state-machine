using sozooo.GameStateMachine.StateInfrastructure;
using sozooo.GameStateMachine.StateMachine;

namespace Code.Infrastructure.States.GameStates
{
    public class HomeScreenState : EndOfFrameExitState
    {
        private readonly IGameStateMachine _stateMachine;
        
        public HomeScreenState(IGameStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public override void Enter()
        {
            //base.Enter(); is empty no need to implement
            //INITIALIZE HOMESCREEN FEATURES
        }

        protected override void OnUpdate()
        {
            //base.OnUpdate(); is empty no need to implement
            
            //UPDATE FEATURES
        }

        protected override void ExitOnEndOfFrame()
        {
            //base.ExitOnEndOfFrame(); is empty no need to implement
            
            //CLOSE WINDOWS / CLEANUP / DEACTIVATE META
        }
    }
}