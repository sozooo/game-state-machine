using sozooo.GameStateMachine.StateInfrastructure;

namespace Code.Infrastructure.States.GameStates
{
    public class GameOverState : SimpleState
    {
        public GameOverState()
        {
        }
    
        public override void Enter()
        {
            //base.Enter(); is empty no need to implement
            //GAME OVER LOGIC
            //Then you can revive player or change state on LoadHomeScreenState which will load the home screen on enter
        }
    }
}