using sozooo.GameStateMachine.StateInfrastructure;
using sozooo.GameStateMachine.StateMachine;

namespace Code.Infrastructure.States.GameStates
{
    public class LoadHomeScreenState : SimpleState
    {
        private readonly IGameStateMachine _stateMachine;
        private readonly ISceneLoader _sceneLoader;
        
        public LoadHomeScreenState(IGameStateMachine stateMachine, ISceneLoader sceneLoader)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
        }

        public override void Enter()
        {
            //base.Enter(); is empty no need to implement
            _sceneLoader.Load("SomeHomeScreen", OnLoadHomeScreen);
        }

        private void OnLoadHomeScreen()
        {
            _stateMachine.Enter<HomeScreenState>();
        }
    }
}