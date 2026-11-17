using UnityEngine;

namespace ArenaBreak.Player
{
    public class HitParticleSpawner : MonoBehaviour
    {
        [SerializeField] private WeaponSystem _weaponSystem;
        [Tooltip("재생이 끝나면 스스로 파괴되도록(Stop Action: Destroy) 설정된 파티클 프리팹")]
        [SerializeField] private ParticleSystem _particlePrefab;

        private void Awake()
        {
            if (_weaponSystem == null || _particlePrefab == null)
            {
                Debug.LogError("HitParticleSpawner: WeaponSystem과 파티클 프리팹을 할당해야 합니다.", this);
                enabled = false;
            }
        }

        private void OnEnable()
        {
            _weaponSystem.ShotHit += OnShotHit;
        }

        private void OnDisable()
        {
            _weaponSystem.ShotHit -= OnShotHit;
        }

        // 연사하면 초당 여러 개가 생성되고 사라지는 오브젝트라 풀링 대상이다.
        // 법선 방향으로 향하게 해서 벽에 맞으면 벽 바깥쪽으로, 적에 맞으면 적 표면 바깥쪽으로 튄다
        private void OnShotHit(RaycastHit hit)
        {
            Instantiate(_particlePrefab, hit.point, Quaternion.LookRotation(hit.normal));
        }
    }
}
