using System;
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

        private InputAction _attackAction;
        private InputAction _reloadAction;

        private int _currentAmmo;
        private float _nextFireTime;
        private float _reloadEndTime;
        private bool _isReloading;

        public event Action<RaycastHit> ShotHit;
        public event Action<int, int> AmmoChanged;
        public event Action ShotFired;

        // HUD는 구독이 늦으면 이벤트를 놓치므로 구독하면서 현재값을 한 번 읽을 수 있게 한다
        public int CurrentAmmo => _currentAmmo;
        public int MagazineSize => _magazineSize;

        // 발사 이펙트가 총알에 맞으면 안 되는 오브젝트(탄피)의 레이어가 제외돼 있는지 검증할 수 있게 연다
        public LayerMask HitMask => _hitMask;

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
            AmmoChanged?.Invoke(_currentAmmo, _magazineSize);

            Ray ray = new Ray(_cameraTransform.position, _cameraTransform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, _range, _hitMask, QueryTriggerInteraction.Ignore))
            {
                ShotHit?.Invoke(hit);
            }

            // 맞았는지와 상관없이 발사된 모든 샷을 알린다. 이 샷의 레이가 끝난 뒤에 알려야 반동이 같은 샷의 방향을 바꾸지 않는다
            ShotFired?.Invoke();
        }

        private void StartReload()
        {
            if (_currentAmmo >= _magazineSize) return;

            _isReloading = true;
            _reloadEndTime = Time.time + _reloadDuration;
        }

        private void FinishReload()
        {
            _isReloading = false;
            _currentAmmo = _magazineSize;
            AmmoChanged?.Invoke(_currentAmmo, _magazineSize);
        }
    }
}
