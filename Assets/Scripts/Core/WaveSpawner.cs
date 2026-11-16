using System;
using System.Collections;
using System.Collections.Generic;
using ArenaBreak.Enemy;
using UnityEngine;
using UnityEngine.Pool;

namespace ArenaBreak.Core
{
    public class WaveSpawner : MonoBehaviour
    {
        [Header("Waves")]
        [SerializeField] private List<WaveData> _waves = new List<WaveData>();
        [SerializeField, Min(0f)] private float _firstWaveDelay = 3f;
        [SerializeField, Min(0f)] private float _nextWaveDelay = 3f;

        [Header("Pool")]
        [SerializeField, Min(0)] private int _prewarmCount = 8;
        [SerializeField, Min(1)] private int _maxPoolSize = 32;

        [Header("References")]
        [SerializeField] private Transform _spawnPointsRoot;
        [SerializeField] private Transform _player;

        public event Action<int> WaveStarted;
        public event Action<int> WaveCleared;
        public event Action AllWavesCleared;

        private readonly List<Transform> _spawnPoints = new List<Transform>();
        private readonly Dictionary<EnemyBase, ObjectPool<EnemyBase>> _pools = new Dictionary<EnemyBase, ObjectPool<EnemyBase>>();
        private readonly List<Health> _pooledHealths = new List<Health>();
        private int _aliveCount;

        // 풀의 콜백(생성, 꺼내기)이 위치를 직접 받을 수 없어서 꺼내기 직전에 여기에 적어 둔다
        private Vector3 _spawnPosition;
        private Quaternion _spawnRotation;

        private void Awake()
        {
            if (_spawnPointsRoot == null || _player == null)
            {
                Debug.LogError("WaveSpawner: SpawnPoints 루트와 Player를 할당해야 합니다.", this);
                enabled = false;
                return;
            }

            // Transform 배열을 인스펙터에 노출하면 스폰 포인트를 늘릴 때마다 다시 연결해야 해서 자식을 직접 모은다
            foreach (Transform child in _spawnPointsRoot)
            {
                _spawnPoints.Add(child);
            }

            if (_spawnPoints.Count == 0)
            {
                Debug.LogError("WaveSpawner: SpawnPoints 아래에 자식이 하나도 없습니다.", this);
                enabled = false;
            }
        }

        private void Start()
        {
            BuildPools();
            StartCoroutine(RunWaves());
        }

        private void OnDisable()
        {
            UnsubscribePooledEnemies();
        }

        private IEnumerator RunWaves()
        {
            // 플레이 시작 직후 바로 적이 나오면 플레이어가 준비할 틈이 없다
            if (_firstWaveDelay > 0f)
            {
                yield return new WaitForSeconds(_firstWaveDelay);
            }

            for (int i = 0; i < _waves.Count; i++)
            {
                int waveNumber = i + 1;
                WaveStarted?.Invoke(waveNumber);

                yield return StartCoroutine(SpawnWave(_waves[i]));

                // 스폰이 끝난 뒤에만 센다. 스폰 도중 앞서 나온 적이 다 죽어도 웨이브가 끝난 것으로 판정하면 안 된다
                yield return new WaitUntil(() => _aliveCount <= 0);

                WaveCleared?.Invoke(waveNumber);

                if (i < _waves.Count - 1)
                {
                    yield return new WaitForSeconds(_nextWaveDelay);
                }
            }

            AllWavesCleared?.Invoke();
        }

        private IEnumerator SpawnWave(WaveData wave)
        {
            List<EnemyBase> order = BuildSpawnOrder(wave);
            if (order.Count == 0)
            {
                Debug.LogError("WaveSpawner: WaveData에 유효한 적 항목이 없어 이 웨이브를 건너뜁니다.", this);
                yield break;
            }

            WaitForSeconds interval = new WaitForSeconds(wave.SpawnInterval);

            for (int spawned = 0; spawned < order.Count; spawned++)
            {
                ObjectPool<EnemyBase> pool = GetPool(order[spawned]);

                // 풀이 상한에 도달했고 쉬는 적도 없으면 한 마리가 반환될 때까지 기다린다.
                // 건너뛰면 웨이브의 적 수가 달라지고, 상한을 넘겨 만들면 풀의 의미가 없다
                while (pool.CountInactive == 0 && pool.CountAll >= _maxPoolSize)
                {
                    yield return null;
                }

                SpawnEnemy(pool);

                if (spawned < order.Count - 1)
                {
                    yield return interval;
                }
            }
        }

        // 항목 순서대로 내보내면 한 종류가 다 나온 뒤에야 다음 종류가 나와서 섞인 웨이브가 되지 않는다
        private List<EnemyBase> BuildSpawnOrder(WaveData wave)
        {
            List<EnemyBase> order = new List<EnemyBase>();
            if (wave == null) return order;

            foreach (WaveData.SpawnEntry entry in wave.Entries)
            {
                if (entry == null || entry.EnemyPrefab == null) continue;

                for (int i = 0; i < entry.Count; i++)
                {
                    order.Add(entry.EnemyPrefab);
                }
            }

            for (int i = order.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (order[i], order[j]) = (order[j], order[i]);
            }

            return order;
        }

        private void SpawnEnemy(ObjectPool<EnemyBase> pool)
        {
            Transform point = _spawnPoints[UnityEngine.Random.Range(0, _spawnPoints.Count)];
            _spawnPosition = point.position;
            _spawnRotation = point.rotation;

            EnemyBase enemy = pool.Get();
            enemy.SetPlayer(_player);

            _aliveCount++;
        }

        private void HandleEnemyDied()
        {
            _aliveCount--;
        }

        // 웨이브에 쓰이는 프리팹마다 풀을 미리 만들어 두면 웨이브 도중 Instantiate 비용이 생기지 않는다
        private void BuildPools()
        {
            foreach (WaveData wave in _waves)
            {
                if (wave == null) continue;

                foreach (WaveData.SpawnEntry entry in wave.Entries)
                {
                    if (entry != null && entry.EnemyPrefab != null)
                    {
                        GetPool(entry.EnemyPrefab);
                    }
                }
            }
        }

        private ObjectPool<EnemyBase> GetPool(EnemyBase prefab)
        {
            if (_pools.TryGetValue(prefab, out ObjectPool<EnemyBase> existing))
            {
                return existing;
            }

            // 생성 콜백이 풀 자신을 반환 대상으로 넘겨야 해서 변수를 먼저 선언한다
            ObjectPool<EnemyBase> pool = null;
            pool = new ObjectPool<EnemyBase>(
                createFunc: () => CreateEnemy(prefab, pool),
                actionOnGet: OnGetEnemy,
                actionOnRelease: OnReleaseEnemy,
                actionOnDestroy: OnDestroyEnemy,
                collectionCheck: true,
                defaultCapacity: _prewarmCount,
                maxSize: _maxPoolSize);

            _pools.Add(prefab, pool);
            Prewarm(pool);

            return pool;
        }

        private void Prewarm(ObjectPool<EnemyBase> pool)
        {
            _spawnPosition = transform.position;
            _spawnRotation = Quaternion.identity;

            EnemyBase[] warmed = new EnemyBase[Mathf.Min(_prewarmCount, _maxPoolSize)];
            for (int i = 0; i < warmed.Length; i++)
            {
                warmed[i] = pool.Get();
            }

            for (int i = 0; i < warmed.Length; i++)
            {
                pool.Release(warmed[i]);
            }
        }

        // 풀 용량이 부족할 때와 미리 만들 때만 Instantiate가 일어난다
        private EnemyBase CreateEnemy(EnemyBase prefab, ObjectPool<EnemyBase> pool)
        {
            EnemyBase enemy = Instantiate(prefab, _spawnPosition, _spawnRotation, transform);
            enemy.SetReleaseHandler(pool.Release);

            // 풀의 적은 계속 재사용되므로 만들 때 한 번만 구독하고 풀이 사라질 때 해제한다
            Health health = enemy.GetComponent<Health>();
            health.Died += HandleEnemyDied;
            _pooledHealths.Add(health);

            return enemy;
        }

        private void OnGetEnemy(EnemyBase enemy)
        {
            // 비활성 상태에서 위치를 옮겨야 활성화될 때 에이전트가 새 위치에서 NavMesh에 자리를 잡는다
            enemy.transform.SetPositionAndRotation(_spawnPosition, _spawnRotation);
            enemy.gameObject.SetActive(true);
        }

        private void OnReleaseEnemy(EnemyBase enemy)
        {
            enemy.gameObject.SetActive(false);
        }

        // 풀 상한을 넘겨 반환된 적만 여기서 파괴된다
        private void OnDestroyEnemy(EnemyBase enemy)
        {
            Health health = enemy.GetComponent<Health>();
            health.Died -= HandleEnemyDied;
            _pooledHealths.Remove(health);

            Destroy(enemy.gameObject);
        }

        private void UnsubscribePooledEnemies()
        {
            foreach (Health health in _pooledHealths)
            {
                if (health != null)
                {
                    health.Died -= HandleEnemyDied;
                }
            }

            _pooledHealths.Clear();
        }
    }
}
