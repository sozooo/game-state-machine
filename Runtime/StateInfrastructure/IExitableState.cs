using RSG;

namespace sozooo.GameStateMachine.StateInfrastructure
{
    public interface IExitableState
    {
        IPromise BeginExit();
        void EndExit();
    }
}