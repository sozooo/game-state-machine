using RSG;

namespace sozooo.GameStateMachine.StateInfrastructure
{
    public class SimpleState : IState
    {
        public virtual void Enter()
        {
        }

        protected virtual void Exit()
        {
        }

        IPromise IExitableState.BeginExit()
        {
            Exit();
            return Promise.Resolved();
        }

        void IExitableState.EndExit()
        {
        }
    }
}