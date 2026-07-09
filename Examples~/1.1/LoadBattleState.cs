using sozooo.GameStateMachine.StateInfrastructure;
using sozooo.GameStateMachine.StateMachine;

namespace Code.Infrastructure.States.GameStates
{
    //CALL TO LOAD GAME SCENES
    public class LoadBattleState : SimplePayloadState<string>
    {
        private readonly IGameStateMachine _stateMachine;
        private readonly ISceneLoader _sceneLoader;

        public LoadBattleState(IGameStateMachine stateMachine, ISceneLoader sceneLoader)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
        }
    
        public override void Enter(string sceneName)
        {
            //base.Enter(); is empty no need to implement
            
            _sceneLoader.LoadScene(sceneName, EnterBattleLoopState);
        }

        private void EnterBattleLoopState()
        {
            _stateMachine.Enter<BattleEnterState>();
        }
    }
}