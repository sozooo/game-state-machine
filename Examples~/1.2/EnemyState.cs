using Application.Core.Configs;
using RSG;
using sozooo.GameStateMachine.StateInfrastructure;
using sozooo.GameStateMachine.StateMachine;
using UnityEngine;

namespace Application.Gameplay.Enemy.States
{
    public abstract class EnemyState : EndOfFrameExitState
    {
        protected readonly EnemyBehaviour EnemyBehaviour;
        protected readonly EnemyConfig Config;
        
        private readonly IStateMachine _stateMachine;

        protected EnemyState(EnemyBehaviour enemy, EnemyConfig config, IStateMachine stateMachine)
        {
            EnemyBehaviour = enemy;
            Config = config;
            _stateMachine = stateMachine;
        }

        protected void ChangeState<T>() where T : class, IState
        {
            _stateMachine.Enter<T>(new object[] { EnemyBehaviour, Config, _stateMachine });
        }

        protected bool CheckPlayerDetection()
        {
            Vector3 directionToPlayer = EnemyBehaviour.Player.Transform.position - EnemyBehaviour.transform.position;
            float distance = directionToPlayer.magnitude;

            if (distance > Config.DetectionRadius)
                return false;

            float dot = Vector3.Dot(EnemyBehaviour.transform.forward, directionToPlayer.normalized);
            float cosAngle = Mathf.Cos(Config.DetectionAngle * 0.5f * Mathf.Deg2Rad);
            
            if (dot < cosAngle)
                return false;

            if (Physics.Raycast(EnemyBehaviour.transform.position, directionToPlayer.normalized, out RaycastHit hit, distance))
                return hit.transform == EnemyBehaviour.Player.Transform;

            return false;
        }

        protected bool HasReachedDestination()
        {
            return !EnemyBehaviour.NavAgent.pathPending
                && EnemyBehaviour.NavAgent.remainingDistance <= EnemyBehaviour.NavAgent.stoppingDistance;
        }
    }
}

