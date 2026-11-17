using UnityEngine;
using UnityEngine.Pool;

namespace ArenaBreak.Enemy
{
    public class RangedEnemyAI : EnemyBase
    {
        [Header("Distance")]
        [Tooltip("플레이어가 이 거리보다 가까워지면 거리를 벌린다")]
        [SerializeField, Min(0f)] private float _keepDistance = 6f;
        [Tooltip("플레이어가 이 거리 안에 있으면 발사한다. 더 멀면 다가간다")]
        [SerializeField, Min(0f)] private float _attackRange = 12f;

        [Header("Attack")]
        [SerializeField, Min(0.1f)] private float _fireInterval = 2f;
        [SerializeField] private EnemyProjectile _projectilePrefab;
        [Tooltip("투사체가 나오는 위치. 비워 두면 적의 위치에서 나온다")]
        [SerializeField] private Transform _muzzle;
        [Tooltip("이 적이 동시에 날려 둘 수 있는 투사체 수만큼의 풀 크기. 투사체 수명 ÷ 발사 간격보다 크면 충분하다")]
        [SerializeField, Min(1)] private int _projectilePoolSize = 4;

        private float _nextFireTime;
        private ObjectPool<EnemyProjectile> _projectilePool;

        protected override void Awake()
        {
            base.Awake();

            // 컴포넌트를 끄면 기반 클래스의 OnEnable(사망 구독)도 안 불려서 죽은 적이 풀로 돌아가지 못한다.
            // 그래서 끄지 않고 발사만 막는다
            if (_projectilePrefab == null)
            {
                Debug.LogError("RangedEnemyAI: 투사체 프리팹을 할당해야 합니다. 이 적은 발사하지 않습니다.", this);
            }
        }

        private void OnDestroy()
        {
            _projectilePool?.Dispose();
        }

        // 이전 생의 발사 타이머가 남으면 재사용된 적이 이유 없이 늦게 쏜다
        protected override void ResetForReuse()
        {
            base.ResetForReuse();

            _nextFireTime = 0f;
        }

        private void Update()
        {
            // 스폰 위치가 NavMesh 밖이면 에이전트 호출이 프레임마다 에러를 쏟아낸다
            if (IsDead || Player == null || !Agent.isOnNavMesh) return;

            Vector3 toPlayer = Player.position - transform.position;
            float sqrDistance = toPlayer.sqrMagnitude;

            if (sqrDistance < _keepDistance * _keepDistance)
            {
                Retreat();
            }
            else if (sqrDistance > _attackRange * _attackRange)
            {
                Approach();
            }
            else
            {
                HoldAndFire(toPlayer);
            }
        }

        // 유지 거리 안쪽으로 들어오면 플레이어에서 유지 거리만큼 떨어진 지점으로 물러난다
        private void Retreat()
        {
            Vector3 away = transform.position - Player.position;
            away.y = 0f;
            Vector3 direction = away.sqrMagnitude > Mathf.Epsilon ? away.normalized : -transform.forward;

            Agent.isStopped = false;
            SetDestinationThrottled(Player.position + direction * _keepDistance);
        }

        private void Approach()
        {
            Agent.isStopped = false;
            SetDestinationThrottled(Player.position);
        }

        // 멈춘 에이전트는 회전하지 않으므로 직접 플레이어를 바라봐야 투사체가 앞으로 나간다
        private void HoldAndFire(Vector3 toPlayer)
        {
            Agent.isStopped = true;

            Vector3 flat = toPlayer;
            flat.y = 0f;
            if (flat.sqrMagnitude > Mathf.Epsilon)
            {
                transform.rotation = Quaternion.LookRotation(flat);
            }

            if (Time.time >= _nextFireTime)
            {
                Fire();
            }
        }

        private void Fire()
        {
            _nextFireTime = Time.time + _fireInterval;

            if (_projectilePrefab == null) return;

            // 발사하지 않는 적은 풀을 만들 필요가 없어서 처음 쏠 때 만든다
            if (_projectilePool == null)
            {
                _projectilePool = CreateProjectilePool();
            }

            Transform origin = _muzzle != null ? _muzzle : transform;
            Vector3 direction = (Player.position - origin.position).normalized;

            EnemyProjectile projectile = _projectilePool.Get();
            projectile.transform.SetPositionAndRotation(origin.position, Quaternion.LookRotation(direction));
            projectile.Launch(direction, transform, Player);
        }

        private ObjectPool<EnemyProjectile> CreateProjectilePool()
        {
            // 생성 콜백이 풀 자신을 반환 대상으로 넘겨야 해서 변수를 먼저 선언한다
            ObjectPool<EnemyProjectile> pool = null;
            pool = new ObjectPool<EnemyProjectile>(
                createFunc: () =>
                {
                    EnemyProjectile projectile = Instantiate(_projectilePrefab);
                    projectile.SetReleaseHandler(pool.Release);
                    return projectile;
                },
                actionOnGet: projectile => projectile.gameObject.SetActive(true),
                actionOnRelease: ReleaseProjectile,
                actionOnDestroy: DestroyProjectile,
                collectionCheck: true,
                defaultCapacity: _projectilePoolSize,
                maxSize: _projectilePoolSize);

            return pool;
        }

        // 씬이 정리되는 중에는 투사체가 먼저 파괴되어 있을 수 있다
        private static void ReleaseProjectile(EnemyProjectile projectile)
        {
            if (projectile != null)
            {
                projectile.gameObject.SetActive(false);
            }
        }

        private static void DestroyProjectile(EnemyProjectile projectile)
        {
            if (projectile != null)
            {
                Destroy(projectile.gameObject);
            }
        }
    }
}
