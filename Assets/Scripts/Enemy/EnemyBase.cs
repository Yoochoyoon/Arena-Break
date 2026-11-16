using System;
using System.Collections;
using ArenaBreak.Core;
using UnityEngine;
using UnityEngine.AI;

namespace ArenaBreak.Enemy
{
    // 웨이브가 적 종류를 가리지 않고 다루려면 플레이어 지정, 사망 후 반환, 재사용 초기화가 한곳에 있어야 한다
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Health))]
    public abstract class EnemyBase : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform _player;

        [Header("Death")]
        [SerializeField, Min(0f)] private float _destroyDelay = 1f;

        private Health _health;
        private Action<EnemyBase> _releaseHandler;
        private WaitForSeconds _releaseWait;

        protected NavMeshAgent Agent { get; private set; }
        protected Transform Player => _player;
        protected bool IsDead { get; private set; }

        protected virtual void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            _health = GetComponent<Health>();
            _releaseWait = new WaitForSeconds(_destroyDelay);
        }

        protected virtual void OnEnable()
        {
            _health.Died += HandleDied;

            ResetForReuse();
        }

        protected virtual void OnDisable()
        {
            _health.Died -= HandleDied;
        }

        // 프리팹은 씬 오브젝트를 참조할 수 없어서 스폰한 쪽이 플레이어를 넣어준다
        public void SetPlayer(Transform player)
        {
            _player = player;
            OnPlayerChanged();
        }

        // 풀에서 꺼낸 적만 반환 방법을 받는다. 받지 못한 적(씬에 직접 놓은 적)은 예전처럼 파괴한다
        public void SetReleaseHandler(Action<EnemyBase> releaseHandler)
        {
            _releaseHandler = releaseHandler;
        }

        protected virtual void OnPlayerChanged() { }

        protected virtual void OnDied() { }

        // 풀에서 다시 꺼낸 적이 이전 생의 사망 상태, 타이머, 이동 경로를 들고 나오지 않게 한다.
        // 하위 클래스는 base를 먼저 호출하고 자기 상태를 되돌린다
        protected virtual void ResetForReuse()
        {
            IsDead = false;
            _health.Restore();

            // 사망할 때 꺼 둔 에이전트를 다시 켜야 새 위치에서 NavMesh에 자리를 잡는다
            Agent.enabled = true;
            if (Agent.isOnNavMesh)
            {
                Agent.ResetPath();
                Agent.isStopped = false;
            }
        }

        private void HandleDied()
        {
            IsDead = true;
            Agent.enabled = false;
            OnDied();

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
    }
}
