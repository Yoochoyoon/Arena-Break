using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace ArenaBreak.Player
{
    public class HitParticleSpawner : MonoBehaviour
    {
        [SerializeField] private WeaponSystem _weaponSystem;
        [Tooltip("재생이 끝나도 파괴되지 않는(Stop Action: None) 파티클 프리팹. 풀에서 꺼내 다시 재생한다")]
        [SerializeField] private ParticleSystem _particlePrefab;
        [Tooltip("동시에 재생 중일 수 있는 최대 개수. 넘으면 그 발사의 파티클은 건너뛴다")]
        [SerializeField, Min(1)] private int _maxParticles = 16;

        private readonly List<ParticleSystem> _activeParticles = new List<ParticleSystem>();
        private ObjectPool<ParticleSystem> _pool;
        private Transform _container;

        private void Awake()
        {
            if (_weaponSystem == null || _particlePrefab == null)
            {
                Debug.LogError("HitParticleSpawner: WeaponSystem과 파티클 프리팹을 할당해야 합니다.", this);
                enabled = false;
                return;
            }

            // 플레이어 자식으로 두면 플레이어가 움직일 때 같이 끌려가서 월드에 따로 모아 둔다
            _container = new GameObject("HitSparks").transform;

            _pool = new ObjectPool<ParticleSystem>(
                createFunc: () => Instantiate(_particlePrefab, _container),
                actionOnGet: particle => particle.gameObject.SetActive(true),
                actionOnRelease: ReleaseParticle,
                actionOnDestroy: DestroyParticle,
                collectionCheck: true,
                defaultCapacity: _maxParticles,
                maxSize: _maxParticles);
        }

        private void OnEnable()
        {
            _weaponSystem.ShotHit += OnShotHit;
        }

        private void OnDisable()
        {
            _weaponSystem.ShotHit -= OnShotHit;

            // 플레이를 멈추거나 씬이 사라질 때는 파티클이 이미 파괴되어 있을 수 있어서 건너뛴다
            foreach (ParticleSystem particle in _activeParticles)
            {
                if (particle != null)
                {
                    _pool.Release(particle);
                }
            }

            _activeParticles.Clear();
        }

        private void OnDestroy()
        {
            _pool?.Dispose();

            if (_container != null)
            {
                Destroy(_container.gameObject);
            }
        }

        private void Update()
        {
            // 재생이 끝난 것만 풀로 돌려보낸다. 중간에 지우므로 뒤에서부터 돈다
            for (int i = _activeParticles.Count - 1; i >= 0; i--)
            {
                ParticleSystem particle = _activeParticles[i];

                if (particle == null)
                {
                    _activeParticles.RemoveAt(i);
                }
                else if (!particle.IsAlive(true))
                {
                    _pool.Release(particle);
                    _activeParticles.RemoveAt(i);
                }
            }
        }

        // 씬이 정리되는 중에는 풀이 들고 있던 파티클이 먼저 파괴되어 있을 수 있다
        private static void ReleaseParticle(ParticleSystem particle)
        {
            if (particle != null)
            {
                particle.gameObject.SetActive(false);
            }
        }

        private static void DestroyParticle(ParticleSystem particle)
        {
            if (particle != null)
            {
                Destroy(particle.gameObject);
            }
        }

        // 법선 방향으로 향하게 해서 벽에 맞으면 벽 바깥쪽으로, 적에 맞으면 적 표면 바깥쪽으로 튄다
        private void OnShotHit(RaycastHit hit)
        {
            if (_pool.CountActive >= _maxParticles) return;

            ParticleSystem particle = _pool.Get();
            particle.transform.SetPositionAndRotation(hit.point, Quaternion.LookRotation(hit.normal));

            // 꺼내자마자 이전 재생의 입자가 남아 있지 않게 비우고 처음부터 다시 재생한다
            particle.Clear(true);
            particle.Play(true);

            _activeParticles.Add(particle);
        }
    }
}
