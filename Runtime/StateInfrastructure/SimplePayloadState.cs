using RSG;

namespace sozooo.GameStateMachine.StateInfrastructure
{
    public class SimplePayloadState<TPayload> : IPayloadState<TPayload>
    {
        public virtual void Enter(TPayload payload)
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