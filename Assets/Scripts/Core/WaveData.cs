using ArenaBreak.Enemy;
using UnityEngine;

namespace ArenaBreak.Core
{
    [CreateAssetMenu(fileName = "WaveData", menuName = "ArenaBreak/Wave Data")]
    public class WaveData : ScriptableObject
    {
        // GameObject가 아니라 EnemyAI로 받아야 스폰한 직후 SetPlayer를 부를 때 GetComponent가 필요 없다
        [SerializeField] private EnemyAI _enemyPrefab;
        [SerializeField, Min(1)] private int _spawnCount = 5;
        [SerializeField, Min(0f)] private float _spawnInterval = 1f;

        public EnemyAI EnemyPrefab => _enemyPrefab;
        public int SpawnCount => _spawnCount;
        public float SpawnInterval => _spawnInterval;
    }
}
