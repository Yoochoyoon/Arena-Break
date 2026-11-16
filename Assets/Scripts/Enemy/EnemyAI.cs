using System;
using System.Collections;
using ArenaBreak.Core;
using UnityEngine;
using UnityEngine.AI;

namespace ArenaBreak.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Health))]
    public class EnemyAI : MonoBehaviour
    {
        private enum State
        {
            Idle,
            Chase,
            Attack,
            Dead
        }

        [Header("Target")]
        [SerializeField] private Transform _player;

        [Header("Range")]
        [SerializeField, Min(0f)] private float _chaseRange = 60f;
        [SerializeField, Min(0f)] private float _attackRange = 2f;

        [Header("Attack")]
        [SerializeField, Min(0.1f)] private float _attackInterval = 2f;
        [SerializeField, Min(0)] private int _attackDamage = 10;

        [Header("Death")]
        [SerializeField, Min(0f)] private float _destroyDelay = 1f;

        private NavMeshAgent _agent;
        private Health _health;
        private IDamageable _playerDamageable;

        private State _state = State.Idle;
        private float _nextAttackTime;

        private Action<EnemyAI> _releaseHandler;
        private WaitForSeconds _releaseWait;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _health = GetComponent<Health>();
            _releaseWait = new WaitForSeconds(_destroyDelay);

            CachePlayerDamageable();
        }

        private void OnEnable()
        {
            _health.Died += HandleDied;

            ResetForReuse();
        }

        private void OnDisable()
        {
            _health.Died -= HandleDied;
        }

        // 프리팹은 씬 오브젝트를 참조할 수 없어서 스폰한 쪽이 플레이어를 넣어준다
        public void SetPlayer(Transform player)
        {
            _player = player;
            CachePlayerDamageable();
        }

        // 풀에서 꺼낸 적만 반환 방법을 받는다. 받지 못한 적(씬에 직접 놓은 적)은 예전처럼 파괴한다
        public void SetReleaseHandler(Action<EnemyAI> releaseHandler)
        {
            _releaseHandler = releaseHandler;
        }

        private void Update()
        {
            // 스폰 위치가 NavMesh 밖이면 에이전트 호출이 프레임마다 에러를 쏟아낸다
            if (_state == State.Dead || !_agent.isOnNavMesh) return;

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
            if (_player == null) return;

            if (SqrDistanceToPlayer() <= _chaseRange * _chaseRange)
            {
                ChangeState(State.Chase);
            }
        }

        private void UpdateChase()
        {
            if (_player == null)
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
                _agent.SetDestination(_player.position);
            }
        }

        private void UpdateAttack()
        {
            if (_player == null)
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

            // 플레이어에 Health가 생기기 전까지 공격 여부를 확인하는 임시 로그. 이후 삭제할 것
            Debug.LogFormat("[Enemy] {0} 공격 - 데미지 {1}, 대상 IDamageable {2}", name, _attackDamage, _playerDamageable != null);
        }

        private void ChangeState(State next)
        {
            _state = next;

            // 공격 중에는 제자리에 서 있어야 하고, 추적으로 돌아가면 다시 움직여야 한다
            _agent.isStopped = next != State.Chase;
        }

        private void HandleDied()
        {
            _state = State.Dead;
            _agent.enabled = false;

            StartCoroutine(ReleaseAfterDelay());
        }

        private IEnumerator ReleaseAfterDelay()
        {
            yield return _releaseWait;

            if (_releaseHandler != null)
            {
                _releaseHandler(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // 풀에서 다시 꺼낸 적이 이전 생의 사망 상태, 공격 타이머, 이동 경로를 들고 나오지 않게 한다
        private void ResetForReuse()
        {
            _state = State.Idle;
            _nextAttackTime = 0f;

            _health.Restore();

            // 사망할 때 꺼 둔 에이전트를 다시 켜야 새 위치에서 NavMesh에 자리를 잡는다
            _agent.enabled = true;
            if (_agent.isOnNavMesh)
            {
                _agent.ResetPath();
                _agent.isStopped = false;
            }
        }

        private float SqrDistanceToPlayer()
        {
            return (_player.position - transform.position).sqrMagnitude;
        }

        // 공격할 때마다 조회하지 않도록 플레이어가 정해질 때 한 번만 찾아 둔다
        private void CachePlayerDamageable()
        {
            _playerDamageable = null;

            if (_player != null)
            {
                _player.TryGetComponent(out _playerDamageable);
            }
        }
    }
}
