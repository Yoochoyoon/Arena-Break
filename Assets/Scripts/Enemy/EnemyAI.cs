using ArenaBreak.Core;
using UnityEngine;

namespace ArenaBreak.Enemy
{
    public class EnemyAI : EnemyBase
    {
        private enum State
        {
            Idle,
            Chase,
            Attack,
            Dead
        }

        [Header("Range")]
        [SerializeField, Min(0f)] private float _chaseRange = 60f;
        [SerializeField, Min(0f)] private float _attackRange = 2f;

        [Header("Attack")]
        [SerializeField, Min(0.1f)] private float _attackInterval = 2f;
        [SerializeField, Min(0)] private int _attackDamage = 10;

        private IDamageable _playerDamageable;

        private State _state = State.Idle;
        private float _nextAttackTime;

        protected override void Awake()
        {
            base.Awake();

            CachePlayerDamageable();
        }

        protected override void OnPlayerChanged()
        {
            CachePlayerDamageable();
        }

        protected override void OnDied()
        {
            _state = State.Dead;
        }

        protected override void ResetForReuse()
        {
            base.ResetForReuse();

            _state = State.Idle;
            _nextAttackTime = 0f;
        }

        private void Update()
        {
            // 스폰 위치가 NavMesh 밖이면 에이전트 호출이 프레임마다 에러를 쏟아낸다
            if (_state == State.Dead || !Agent.isOnNavMesh) return;

            switch (_state)
            {
                case State.Idle:
                    UpdateIdle();
                    break;
                case State.Chase:
                    UpdateChase();
                    break;
                case State.Attack:
                    UpdateAttack();
                    break;
            }
        }

        private void UpdateIdle()
        {
            if (Player == null) return;

            if (SqrDistanceToPlayer() <= _chaseRange * _chaseRange)
            {
                ChangeState(State.Chase);
            }
        }

        private void UpdateChase()
        {
            if (Player == null)
            {
                ChangeState(State.Idle);
                return;
            }

            float sqrDistance = SqrDistanceToPlayer();
            if (sqrDistance > _chaseRange * _chaseRange)
            {
                ChangeState(State.Idle);
            }
            else if (sqrDistance <= _attackRange * _attackRange)
            {
                ChangeState(State.Attack);
            }
            else
            {
                SetDestinationThrottled(Player.position);
            }
        }

        private void UpdateAttack()
        {
            if (Player == null)
            {
                ChangeState(State.Idle);
                return;
            }

            if (SqrDistanceToPlayer() > _attackRange * _attackRange)
            {
                ChangeState(State.Chase);
                return;
            }

            if (Time.time >= _nextAttackTime)
            {
                Attack();
            }
        }

        private void Attack()
        {
            _nextAttackTime = Time.time + _attackInterval;

            if (_playerDamageable != null)
            {
                _playerDamageable.TakeDamage(_attackDamage);
            }
        }

        private void ChangeState(State next)
        {
            _state = next;

            // 공격 중에는 제자리에 서 있어야 하고, 추적으로 돌아가면 다시 움직여야 한다
            Agent.isStopped = next != State.Chase;

            if (next == State.Chase)
            {
                ResetRepathTimer();
            }
        }

        private float SqrDistanceToPlayer()
        {
            return (Player.position - transform.position).sqrMagnitude;
        }

        // 공격할 때마다 조회하지 않도록 플레이어가 정해질 때 한 번만 찾아 둔다
        private void CachePlayerDamageable()
        {
            _playerDamageable = null;

            if (Player != null)
            {
                Player.TryGetComponent(out _playerDamageable);
            }
        }
    }
}
