using Application.Core.Configs;
using sozooo.GameStateMachine.StateMachine;
using UnityEngine;

namespace Application.Gameplay.Enemy.States
{
    public class EnemySearchState : EnemyState
    {
        private float _waitTimer;

        public EnemySearchState(EnemyBehaviour enemy, EnemyConfig config, IStateMachine stateMachine) 
            : base(enemy, config, stateMachine)
        {
        }

        public override void Enter()
        {
            EnemyBehaviour.NavAgent.SetDestination(EnemyBehaviour.SearchPosition);
            _waitTimer = 0;
        }

        protected override void OnUpdate()
        {
            if (CheckPlayerDetection())
            {
                ChangeState<EnemyChaseState>();
                return;
            }

            if (HasReachedDestination())
            {
                _waitTimer += Time.deltaTime;
                if (_waitTimer >= Config.PatrolWaitTime)
                    ChangeState<EnemyPatrolState>();
            }
        }

        protected override void ExitOnEndOfFrame() => 
            _waitTimer = 0;
    }
}

