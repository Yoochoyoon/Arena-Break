using System;
using System.Collections.Generic;
using ArenaBreak.Enemy;
using UnityEngine;

namespace ArenaBreak.Core
{
    [CreateAssetMenu(fileName = "WaveData", menuName = "ArenaBreak/Wave Data")]
    public class WaveData : ScriptableObject
    {
        [Serializable]
        public class SpawnEntry
        {
            // GameObject가 아니라 EnemyBase로 받아야 스폰한 직후 SetPlayer를 부를 때 GetComponent가 필요 없고, 적 종류도 가리지 않는다
            [SerializeField] private EnemyBase _enemyPrefab;
            [SerializeField, Min(1)] private int _count = 1;

            public EnemyBase EnemyPrefab => _enemyPrefab;
            public int Count => _count;
        }

        [SerializeField] private List<SpawnEntry> _entries = new List<SpawnEntry>();
        [Tooltip("웨이브 전체에서 적이 한 마리씩 나오는 간격(초)")]
        [SerializeField, Min(0f)] private float _spawnInterval = 1f;

        public IReadOnlyList<SpawnEntry> Entries => _entries;
        public float SpawnInterval => _spawnInterval;
    }
}
