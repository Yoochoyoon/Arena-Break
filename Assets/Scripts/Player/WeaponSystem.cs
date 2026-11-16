using UnityEngine;
using UnityEngine.InputSystem;

namespace ArenaBreak.Player
{
    public class WeaponSystem : MonoBehaviour
    {
        private const string PlayerMapName = "Player";
        private const string AttackActionName = "Attack";
        private const string ReloadActionName = "Reload";
        private const string ReloadBinding = "<Keyboard>/r";

        [Header("Input")]
        [SerializeField] private InputActionAsset _inputActions;

        [Header("Aim")]
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private LayerMask _hitMask = ~0;

        [Header("Fire")]
        [SerializeField] private float _fireInterval = 0.15f;
        [SerializeField] private float _range = 50f;

        [Header("Ammo")]
        [SerializeField] private int _magazineSize = 12;
        [SerializeField] private float _reloadDuration = 1.5f;

        [Header("Hit Marker (Debug)")]
        [SerializeField] private Color _hitMarkerColor = Color.yellow;
        [SerializeField] private float _hitMarkerSize = 0.2f;
        [SerializeField] private float _hitMarkerLifetime = 0.5f;

        private InputAction _attackAction;
        private InputAction _reloadAction;

        private Mesh _hitMarkerMesh;
        private Material _hitMarkerMaterial;

        private int _currentAmmo;
        private float _nextFireTime;
        private float _reloadEndTime;
        private bool _isReloading;

        private void Awake()
        {
            if (_inputActions == null || _cameraTransform == null)
            {
                Debug.LogError("WeaponSystem: InputActionAsset과 Camera Transform을 할당해야 합니다.", this);
                enabled = false;
                return;
            }

            InputActionMap playerMap = _inputActions.FindActionMap(PlayerMapName);
            _attackAction = playerMap?.FindAction(AttackActionName);
            if (_attackAction == null)
            {
                Debug.LogError($"WeaponSystem: '{PlayerMapName}' 맵에 '{AttackActionName}' 액션이 필요합니다.", this);
                enabled = false;
                return;
            }

            // 에셋에 R 액션이 없어서 코드에서 직접 만든다
            _reloadAction = new InputAction(ReloadActionName, InputActionType.Button, ReloadBinding);

            CacheHitMarkerResources();
            _currentAmmo = _magazineSize;
        }

        private void OnEnable()
        {
            if (_attackAction == null) return;

            // 맵 전체를 끄면 PlayerController의 입력까지 죽으므로 이 컴포넌트가 쓰는 액션만 다룬다
            _attackAction.Enable();
            _reloadAction.Enable();
        }

        private void OnDisable()
        {
            if (_attackAction == null) return;

            _attackAction.Disable();
            _reloadAction.Disable();
        }

        private void OnDestroy()
        {
            // 코드로 만든 액션은 에셋 소유가 아니라서 직접 해제해야 한다
            _reloadAction?.Dispose();

            if (_hitMarkerMaterial != null)
            {
                Destroy(_hitMarkerMaterial);
            }
        }

        private void Update()
        {
            if (_isReloading)
            {
                if (Time.time >= _reloadEndTime)
                {
                    FinishReload();
                }
                return;
            }

            if (_reloadAction.WasPressedThisFrame())
            {
                StartReload();
                return;
            }

            if (_attackAction.IsPressed() && Time.time >= _nextFireTime && _currentAmmo > 0)
            {
                Fire();
            }
        }

        private void Fire()
        {
            _nextFireTime = Time.time + _fireInterval;
            _currentAmmo--;

            Ray ray = new Ray(_cameraTransform.position, _cameraTransform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, _range, _hitMask, QueryTriggerInteraction.Ignore))
            {
                SpawnHitMarker(hit.point);
            }

            // HUD(3장)로 옮기기 전까지만 쓰는 임시 로그. 이관 후 삭제할 것
            Debug.LogFormat("[Weapon] 발사 - 남은 탄약 {0}/{1}", _currentAmmo, _magazineSize);
        }

        private void StartReload()
        {
            if (_currentAmmo >= _magazineSize) return;

            _isReloading = true;
            _reloadEndTime = Time.time + _reloadDuration;

            // 임시 로그. HUD 이관 후 삭제할 것
            Debug.LogFormat("[Weapon] 재장전 시작 - 남은 탄약 {0}/{1}", _currentAmmo, _magazineSize);
        }

        private void FinishReload()
        {
            _isReloading = false;
            _currentAmmo = _magazineSize;

            // 임시 로그. HUD 이관 후 삭제할 것
            Debug.LogFormat("[Weapon] 재장전 완료 - 남은 탄약 {0}/{1}", _currentAmmo, _magazineSize);
        }

        // 구체에 Collider가 있으면 다음 발사가 구체에 맞는다.
        // 프리미티브를 복제하는 대신 메시/머티리얼만 꺼내 쓰고 Collider 없는 오브젝트를 직접 만든다
        private void CacheHitMarkerResources()
        {
            GameObject template = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _hitMarkerMesh = template.GetComponent<MeshFilter>().sharedMesh;

            // 렌더 파이프라인 기본 머티리얼을 복제해야 URP에서 셰이더가 빌드에 빠지지 않는다
            _hitMarkerMaterial = new Material(template.GetComponent<MeshRenderer>().sharedMaterial)
            {
                color = _hitMarkerColor
            };

            Destroy(template);
        }

        // 풀링 대상: 연사 중 0.15초마다 생성되고 0.5초 뒤 파괴된다
        private void SpawnHitMarker(Vector3 position)
        {
            GameObject marker = new GameObject("HitMarker");
            marker.transform.position = position;
            marker.transform.localScale = Vector3.one * _hitMarkerSize;

            marker.AddComponent<MeshFilter>().sharedMesh = _hitMarkerMesh;
            marker.AddComponent<MeshRenderer>().sharedMaterial = _hitMarkerMaterial;

            Destroy(marker, _hitMarkerLifetime);
        }
    }
}
