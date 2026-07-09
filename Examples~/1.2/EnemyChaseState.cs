using Application.Core.Configs;
using sozooo.GameStateMachine.StateMachine;
using UnityEngine;

namespace Application.Gameplay.Enemy.States
{
    public class EnemyChaseState : EnemyState
    {
        private readonly Collider[] _overlapResults = new Collider[8];
        
        private Vector3 _lastKnownPosition;
        private float _lostTimer;

        public EnemyChaseState(EnemyBehaviour enemy, EnemyConfig config, IStateMachine stateMachine) 
            : base(enemy, config, stateMachine)
        {
        }

        public override void Enter()
        {
            EnemyBehaviour.NavAgent.speed = Config.ChaseSpeed;
            _lastKnownPosition = EnemyBehaviour.Player.Transform.position;
            _lostTimer = 0;
        }

        protected override void OnUpdate()
        {
            EnemyBehaviour.NavAgent.SetDestination(EnemyBehaviour.Player.Transform.position);

            float distanceToPlayer = Vector3.Distance(
                EnemyBehaviour.transform.position, EnemyBehaviour.Player.Transform.position);

            if (distanceToPlayer <= Config.AttackDistance)
            {
                EnemyBehaviour.LoseGame();
                return;
            }

            if (CheckPlayerDetection())
            {
                _lostTimer = 0;
                _lastKnownPosition = EnemyBehaviour.Player.Transform.position;
            }
            else
            {
                _lostTimer += Time.deltaTime;
                if (_lostTimer >= Config.LostTargetTime)
                {
                    EnemyBehaviour.SearchPosition = _lastKnownPosition;
                    ChangeState<EnemySearchState>();
                }
            }

            PushAwayFromOthers();
        }

        protected override void ExitOnEndOfFrame() => 
            _lostTimer = 0;

        private void PushAwayFromOthers()
        {
            int count = Physics.OverlapSphereNonAlloc(
                EnemyBehaviour.transform.position, 1.5f, _overlapResults);
            Vector3 pushDir = Vector3.zero;

            for (int i = 0; i < count; i++)
            {
                if (_overlapResults[i].TryGetComponent<EnemyBehaviour>(out var other) && other != EnemyBehaviour)
                {
                    Vector3 away = EnemyBehaviour.transform.position - other.transform.position;
                    float dist = away.magnitude;
                    if (dist < 0.01f) continue;
                    pushDir += away.normalized * (1.5f - dist) / 1.5f;
                }
            }

            if (pushDir != Vector3.zero)
                EnemyBehaviour.NavAgent.Move(pushDir * (Time.deltaTime * 2f));
        }
    }
}

