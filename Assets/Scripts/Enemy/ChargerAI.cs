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

        private State _state = State.Chase;
        private Vector3 _chargeDirection;
        private Vector3 _chargeStartPosition;
        private float _windupEndTime;
        private float _chargeEndTime;
        private float _recoveryEndTime;
        private float _nextChargeTime;

        // 돌진이 끝난 채 반환되면 빠른 속도, 후딜레이 상태, 쿨다운이 다음 생으로 넘어가므로 전부 되돌린다
        protected override void ResetForReuse()
        {
            base.ResetForReuse();

            _state = State.Chase;
            _nextChargeTime = 0f;
            Agent.speed = _chaseSpeed;
        }

        private void Update()
        {
            // 스폰 위치가 NavMesh 밖이면 에이전트 호출이 프레임마다 에러를 쏟아낸다
            if (IsDead || Player == null || !Agent.isOnNavMesh) return;

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
            Agent.SetDestination(Player.position);

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

        private Vector3 DirectionToPlayer()
        {
            Vector3 toPlayer = Player.position - transform.position;
            toPlayer.y = 0f;

            return toPlayer.sqrMagnitude > Mathf.Epsilon ? toPlayer.normalized : transform.forward;
        }
    }
}
