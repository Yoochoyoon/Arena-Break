using UnityEngine;

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

        private float _nextFireTime;

        protected override void Awake()
        {
            base.Awake();

            if (_projectilePrefab == null)
            {
                Debug.LogError("RangedEnemyAI: 투사체 프리팹을 할당해야 합니다.", this);
                enabled = false;
            }
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
            Agent.SetDestination(Player.position + direction * _keepDistance);
        }

        private void Approach()
        {
            Agent.isStopped = false;
            Agent.SetDestination(Player.position);
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

        // 발사마다 생성되고 부서지는 오브젝트라 풀링 대상이다
        private void Fire()
        {
            _nextFireTime = Time.time + _fireInterval;

            Transform origin = _muzzle != null ? _muzzle : transform;
            Vector3 direction = (Player.position - origin.position).normalized;

            EnemyProjectile projectile = Instantiate(_projectilePrefab, origin.position, Quaternion.LookRotation(direction));
            projectile.Launch(direction, transform, Player);
        }
    }
}
