using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace ArenaBreak.Player
{
    // 판정은 WeaponSystem의 레이캐스트가 즉시 끝내고, 이 컴포넌트는 총구에서 명중 지점까지 날아가는 공만 보여 준다
    public class BulletTracer : MonoBehaviour
    {
        private struct Ball
        {
            public Transform Transform;
            public Vector3 Start;
            public Vector3 End;
            public float Duration;
            public float Elapsed;
        }

        [Header("References")]
        [SerializeField] private WeaponSystem _weaponSystem;
        [SerializeField] private Transform _cameraTransform;
        [Tooltip("콜라이더가 없는 공 프리팹. 콜라이더가 있으면 다음 발사가 공에 맞는다")]
        [SerializeField] private Transform _ballPrefab;

        [Header("Ball")]
        [Tooltip("카메라 기준 로컬 좌표. 카메라 정중앙에서 출발하면 공이 시야에 점으로만 보인다")]
        [SerializeField] private Vector3 _muzzleLocalOffset = new Vector3(0.3f, -0.25f, 0.5f);
        [SerializeField, Min(1f)] private float _ballSpeed = 80f;

        [Header("Pool")]
        [SerializeField, Min(1)] private int _maxPoolSize = 32;

        private readonly List<Ball> _activeBalls = new List<Ball>();
        private ObjectPool<Transform> _pool;

        private void Awake()
        {
            if (_weaponSystem == null || _cameraTransform == null || _ballPrefab == null)
            {
                Debug.LogError("BulletTracer: WeaponSystem, Camera Transform, 공 프리팹을 할당해야 합니다.", this);
                enabled = false;
                return;
            }

            _pool = new ObjectPool<Transform>(
                createFunc: () => Instantiate(_ballPrefab),
                actionOnGet: ball => ball.gameObject.SetActive(true),
                actionOnRelease: ReleaseBall,
                actionOnDestroy: DestroyBall,
                collectionCheck: true,
                defaultCapacity: _maxPoolSize,
                maxSize: _maxPoolSize);
        }

        private void OnEnable()
        {
            _weaponSystem.ShotHit += OnShotHit;
        }

        private void OnDisable()
        {
            _weaponSystem.ShotHit -= OnShotHit;

            // 날아가던 공이 허공에 멈춘 채 남지 않게 모두 돌려보낸다.
            // 플레이를 멈추거나 씬이 사라질 때는 공이 이미 파괴되어 있을 수 있어서 건너뛴다
            for (int i = 0; i < _activeBalls.Count; i++)
            {
                Transform ball = _activeBalls[i].Transform;
                if (ball != null)
                {
                    _pool.Release(ball);
                }
            }

            _activeBalls.Clear();
        }

        private void OnDestroy()
        {
            _pool?.Dispose();
        }

        private void Update()
        {
            // 중간에 제거하므로 뒤에서부터 돈다
            for (int i = _activeBalls.Count - 1; i >= 0; i--)
            {
                Ball ball = _activeBalls[i];
                ball.Elapsed += Time.deltaTime;

                float progress = ball.Duration > 0f ? Mathf.Clamp01(ball.Elapsed / ball.Duration) : 1f;
                ball.Transform.position = Vector3.Lerp(ball.Start, ball.End, progress);

                if (progress >= 1f)
                {
                    _pool.Release(ball.Transform);
                    _activeBalls.RemoveAt(i);
                }
                else
                {
                    _activeBalls[i] = ball;
                }
            }
        }

        // 씬이 정리되는 중에는 풀이 들고 있던 공이 먼저 파괴되어 있을 수 있다
        private static void ReleaseBall(Transform ball)
        {
            if (ball != null)
            {
                ball.gameObject.SetActive(false);
            }
        }

        private static void DestroyBall(Transform ball)
        {
            if (ball != null)
            {
                Destroy(ball.gameObject);
            }
        }

        private void OnShotHit(RaycastHit hit)
        {
            // 연사가 풀 상한을 넘으면 새 공을 만들지 않고 그 발사는 연출을 건너뛴다
            if (_pool.CountActive >= _maxPoolSize) return;

            Vector3 start = _cameraTransform.TransformPoint(_muzzleLocalOffset);
            Transform ballTransform = _pool.Get();
            ballTransform.position = start;

            _activeBalls.Add(new Ball
            {
                Transform = ballTransform,
                Start = start,
                End = hit.point,
                Duration = Vector3.Distance(start, hit.point) / _ballSpeed,
                Elapsed = 0f
            });
        }
    }
}
