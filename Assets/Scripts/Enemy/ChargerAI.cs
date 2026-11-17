using ArenaBreak.Core;
using UnityEngine;

namespace ArenaBreak.Enemy
{
    public class ChargerAI : EnemyBase
    {
        private enum State
        {
            Chase,
            Windup,
            Charge,
            Recovery
        }

        [Header("Chase")]
        [SerializeField, Min(0f)] private float _chaseSpeed = 2f;

        [Header("Charge")]
        [SerializeField, Min(0f)] private float _chargeTriggerDistance = 8f;
        [SerializeField, Min(0f)] private float _chargeSpeed = 12f;
        [Tooltip("돌진이 끝나는 최대 시간(초). 최대 돌진 거리에 먼저 닿으면 거기서 끝난다")]
        [SerializeField, Min(0.1f)] private float _chargeDuration = 1f;
        [Tooltip("돌진이 끝나는 최대 거리(m). 최대 돌진 시간이 먼저 지나면 거기서 끝난다")]
        [SerializeField, Min(0f)] private float _maxChargeDistance = 12f;
        // 쿨다운이 없으면 후딜레이가 끝나자마자 거리 조건이 다시 맞아서 천천히 추적하는 구간이 사라진다
        [SerializeField, Min(0f)] private float _chargeCooldown = 3f;

        [Header("Windup / Recovery")]
        [SerializeField, Min(0f)] private float _windupDuration = 0.6f;
        [SerializeField, Min(0f)] private float _recoveryDuration = 0.8f;

        [Header("Contact Damage")]
        [SerializeField, Min(0)] private int _contactDamage = 10;
        [Tooltip("플레이어와의 거리가 이 값 이하이면 닿은 것으로 본다")]
        [SerializeField, Min(0f)] private float _contactRange = 1.5f;
        // 간격이 없으면 붙어 있는 동안 매 프레임 데미지가 들어가 순식간에 죽는다
        [SerializeField, Min(0.1f)] private float _contactInterval = 1f;

        private IDamageable _playerDamageable;
        private float _nextContactTime;
        private State _state = State.Chase;
        private Vector3 _chargeDirection;
        private Vector3 _chargeStartPosition;
        private float _windupEndTime;
        private float _chargeEndTime;
        private float _recoveryEndTime;
        private float _nextChargeTime;

        protected override void Awake()
        {
            base.Awake();

            CachePlayerDamageable();
        }

        protected override void OnPlayerChanged()
        {
            CachePlayerDamageable();
        }

        // 돌진이 끝난 채 반환되면 빠른 속도, 후딜레이 상태, 쿨다운이 다음 생으로 넘어가므로 전부 되돌린다
        protected override void ResetForReuse()
        {
            base.ResetForReuse();

            _state = State.Chase;
            _nextChargeTime = 0f;
            _nextContactTime = 0f;
            Agent.speed = _chaseSpeed;
        }

        private void Update()
        {
            // 스폰 위치가 NavMesh 밖이면 에이전트 호출이 프레임마다 에러를 쏟아낸다
            if (IsDead || Player == null || !Agent.isOnNavMesh) return;

            TryContactDamage();

            switch (_state)
            {
                case State.Chase:
                    UpdateChase();
                    break;
                case State.Windup:
                    UpdateWindup();
                    break;
                case State.Charge:
                    UpdateCharge();
                    break;
                case State.Recovery:
                    UpdateRecovery();
                    break;
            }
        }

        private void UpdateChase()
        {
            SetDestinationThrottled(Player.position);

            bool cooledDown = Time.time >= _nextChargeTime;
            float sqrDistance = (Player.position - transform.position).sqrMagnitude;

            if (cooledDown && sqrDistance <= _chargeTriggerDistance * _chargeTriggerDistance)
            {
                StartWindup();
            }
        }

        // 준비 동안 플레이어를 계속 조준하다가 준비가 끝나는 순간의 방향으로 고정한다.
        // 시작 순간에 고정하면 준비가 긴 설정에서 플레이어가 움직여도 엉뚱한 곳으로 돌진한다
        private void UpdateWindup()
        {
            _chargeDirection = DirectionToPlayer();
            transform.rotation = Quaternion.LookRotation(_chargeDirection);

            if (Time.time >= _windupEndTime)
            {
                StartCharge();
            }
        }

        private void UpdateCharge()
        {
            Vector3 traveled = transform.position - _chargeStartPosition;
            traveled.y = 0f;

            bool timeUp = Time.time >= _chargeEndTime;
            bool distanceReached = traveled.sqrMagnitude >= _maxChargeDistance * _maxChargeDistance;

            if (timeUp || distanceReached)
            {
                StartRecovery();
            }
        }

        private void UpdateRecovery()
        {
            if (Time.time < _recoveryEndTime) return;

            _state = State.Chase;
            Agent.isStopped = false;
            Agent.speed = _chaseSpeed;

            // 돌진 목적지가 남아 있어서 간격을 기다리면 그쪽으로 계속 간다
            ResetRepathTimer();
        }

        private void StartWindup()
        {
            _state = State.Windup;
            _windupEndTime = Time.time + _windupDuration;

            StopMoving();
        }

        private void StartCharge()
        {
            _state = State.Charge;
            _chargeStartPosition = transform.position;
            _chargeEndTime = Time.time + _chargeDuration;

            Agent.isStopped = false;
            Agent.speed = _chargeSpeed;

            // 정한 방향으로 끝까지 간다. 플레이어를 따라 꺾으면 돌진이 아니라 빠른 추적이 된다
            Agent.SetDestination(_chargeStartPosition + _chargeDirection * _maxChargeDistance);

            // 가속도 때문에 최고 속도에 닿기 전에 돌진이 끝나는 것을 막는다
            Agent.velocity = _chargeDirection * _chargeSpeed;
        }

        private void StartRecovery()
        {
            _state = State.Recovery;
            _recoveryEndTime = Time.time + _recoveryDuration;
            _nextChargeTime = Time.time + _chargeCooldown;

            StopMoving();
        }

        // 속도를 0으로 만들지 않으면 isStopped만으로는 남은 관성으로 미끄러진다
        private void StopMoving()
        {
            Agent.isStopped = true;
            Agent.velocity = Vector3.zero;
        }

        // 에이전트는 물리 충돌 이벤트를 주지 않아서 거리로 접촉을 판정한다
        private void TryContactDamage()
        {
            if (_playerDamageable == null || Time.time < _nextContactTime) return;

            Vector3 offset = Player.position - transform.position;
            offset.y = 0f;
            if (offset.sqrMagnitude > _contactRange * _contactRange) return;

            _nextContactTime = Time.time + _contactInterval;
            _playerDamageable.TakeDamage(_contactDamage);
        }

        // 접촉할 때마다 조회하지 않도록 플레이어가 정해질 때 한 번만 찾아 둔다
        private void CachePlayerDamageable()
        {
            _playerDamageable = null;

            if (Player != null)
            {
                Player.TryGetComponent(out _playerDamageable);
            }
        }

        private Vector3 DirectionToPlayer()
        {
            Vector3 toPlayer = Player.position - transform.position;
            toPlayer.y = 0f;

            return toPlayer.sqrMagnitude > Mathf.Epsilon ? toPlayer.normalized : transform.forward;
        }
    }
}
