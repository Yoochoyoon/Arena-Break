using System;
using UnityEngine;

namespace ArenaBreak.Core
{
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField, Min(1)] private int _maxHealth = 100;

        // 플레이 중 Inspector에서 확인하려고 직렬화한다. 외부에는 읽기 전용 프로퍼티로만 노출한다
        [SerializeField] private int _currentHealth;

        public event Action<int, int> HealthChanged;
        public event Action Died;

        public int MaxHealth => _maxHealth;
        public int CurrentHealth => _currentHealth;
        public bool IsDead => _currentHealth <= 0;

        private void Awake()
        {
            _currentHealth = _maxHealth;
        }

        // Inspector에서 값을 직접 고쳐도 범위를 벗어나지 않게 한다
        private void OnValidate()
        {
            _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        }

        public void TakeDamage(int amount)
        {
            // 사망 후 들어온 데미지로 Died가 두 번 발행되는 것을 막는다
            if (IsDead || amount <= 0) return;

            _currentHealth = Mathf.Clamp(_currentHealth - amount, 0, _maxHealth);

            HealthChanged?.Invoke(_currentHealth, _maxHealth);

            if (IsDead)
            {
                Died?.Invoke();
            }
        }

        // 풀링으로 재사용되는 오브젝트가 이전 생의 체력을 들고 나오지 않게 한다
        public void Restore()
        {
            _currentHealth = _maxHealth;

            HealthChanged?.Invoke(_currentHealth, _maxHealth);
        }
    }
}
