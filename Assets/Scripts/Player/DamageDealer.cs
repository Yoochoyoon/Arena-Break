using System.Collections.Generic;
using ArenaBreak.Core;
using UnityEngine;

namespace ArenaBreak.Player
{
    [RequireComponent(typeof(WeaponSystem))]
    public class DamageDealer : MonoBehaviour
    {
        [SerializeField, Min(0)] private int _damage = 10;

        private readonly Dictionary<Collider, IDamageable> _damageableCache = new Dictionary<Collider, IDamageable>();
        private WeaponSystem _weaponSystem;

        private void Awake()
        {
            _weaponSystem = GetComponent<WeaponSystem>();
        }

        private void OnEnable()
        {
            _weaponSystem.ShotHit += HandleShotHit;
        }

        private void OnDisable()
        {
            _weaponSystem.ShotHit -= HandleShotHit;
        }

        // 발사는 Update 경로에서 일어나므로 콜라이더당 한 번만 조회한다.
        // 데미지를 받지 않는 대상(벽)도 null로 기억해서 같은 곳을 쏠 때마다 다시 찾지 않는다
        private void HandleShotHit(RaycastHit hit)
        {
            if (!_damageableCache.TryGetValue(hit.collider, out IDamageable target))
            {
                target = hit.collider.GetComponentInParent<IDamageable>();
                _damageableCache.Add(hit.collider, target);
            }

            target?.TakeDamage(_damage);
        }
    }
}
