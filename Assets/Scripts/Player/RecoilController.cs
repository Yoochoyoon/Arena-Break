using UnityEngine;

namespace ArenaBreak.Player
{
    // 카메라를 직접 돌리지 않고 반동 각도만 계산해 PlayerController에 넘긴다.
    // 카메라 회전을 쓰는 곳이 PlayerController 하나여야 마우스 시점과 반동이 서로 덮어쓰지 않는다.
    // PlayerController(기본 0)보다 먼저 실행되어야 같은 프레임의 값이 카메라에 반영된다
    [DefaultExecutionOrder(-50)]
    public class RecoilController : MonoBehaviour
    {
        // 이 값 아래로 줄어든 반동은 0으로 보고 더 계산하지 않는다
        private const float SettleThreshold = 0.001f;

        [Header("References")]
        [SerializeField] private WeaponSystem _weaponSystem;
        [SerializeField] private AimController _aimController;
        [SerializeField] private PlayerController _playerController;

        [Header("Strength (degrees per shot)")]
        [SerializeField, Min(0f)] private float _hipRecoil = 2f;
        [Tooltip("조준 중에는 일반 사격보다 확실히 작아야 한다")]
        [SerializeField, Min(0f)] private float _aimRecoil = 0.6f;

        [Header("Response")]
        [Tooltip("클수록 발사 직후 카메라가 더 빨리 튄다")]
        [SerializeField, Min(0f)] private float _applySpeed = 40f;
        [Tooltip("클수록 반동이 더 빨리 원래 방향으로 돌아온다")]
        [SerializeField, Min(0f)] private float _recoverySpeed = 5f;
        [Tooltip("연사로 쌓일 수 있는 반동의 최대 각도")]
        [SerializeField, Min(0f)] private float _maxAccumulation = 12f;

        // 쌓아 둔 반동 목표와, 카메라에 실제로 적용 중인 반동. 목표가 줄어드는 것을 적용값이 뒤따라가서 튄 뒤 부드럽게 돌아온다
        private float _targetRecoil;
        private float _currentRecoil;

        private void Awake()
        {
            if (_weaponSystem == null || _aimController == null || _playerController == null)
            {
                Debug.LogError("RecoilController: WeaponSystem, AimController, PlayerController를 할당해야 합니다.", this);
                enabled = false;
            }
        }

        private void OnEnable()
        {
            _weaponSystem.ShotFired += OnShotFired;
        }

        private void OnDisable()
        {
            _weaponSystem.ShotFired -= OnShotFired;

            _targetRecoil = 0f;
            _currentRecoil = 0f;
            _playerController.RecoilPitch = 0f;
        }

        private void Update()
        {
            // 반동이 모두 가라앉은 뒤에는 계산할 것이 없다
            if (_targetRecoil == 0f && _currentRecoil == 0f) return;

            // 지수 보간은 프레임레이트가 달라도 같은 속도로 따라간다
            _targetRecoil = Mathf.Lerp(_targetRecoil, 0f, 1f - Mathf.Exp(-_recoverySpeed * Time.deltaTime));
            _currentRecoil = Mathf.Lerp(_currentRecoil, _targetRecoil, 1f - Mathf.Exp(-_applySpeed * Time.deltaTime));

            if (_targetRecoil < SettleThreshold)
            {
                _targetRecoil = 0f;

                if (_currentRecoil < SettleThreshold)
                {
                    _currentRecoil = 0f;
                }
            }

            _playerController.RecoilPitch = _currentRecoil;
        }

        private void OnShotFired()
        {
            float strength = _aimController.IsAiming ? _aimRecoil : _hipRecoil;
            _targetRecoil = Mathf.Min(_targetRecoil + strength, _maxAccumulation);
        }
    }
}
