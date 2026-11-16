using UnityEngine;
using UnityEngine.InputSystem;

namespace ArenaBreak.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class DashController : MonoBehaviour
    {
        private const string DashActionName = "Dash";
        private const string DashBinding = "<Keyboard>/leftShift";

        [Header("Dash")]
        [SerializeField, Min(0f)] private float _dashDistance = 4f;
        [SerializeField, Min(0.01f)] private float _dashDuration = 0.2f;
        [SerializeField, Min(0f)] private float _cooldown = 2f;

        private CharacterController _controller;
        private AimController _aimController;
        private InputAction _dashAction;

        private Vector3 _dashDirection;
        private float _dashStartTime;
        private float _lastProgress;
        private float _nextDashTime;
        private bool _isDashing;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();

            // AimController가 없는 오브젝트에서도 대시는 동작해야 해서 선택적으로만 참조한다
            TryGetComponent(out _aimController);

            // Sprint 액션이 이미 Shift에 묶여 있지만 달리기와 대시는 다른 동작이라 별도 액션을 만든다
            _dashAction = new InputAction(DashActionName, InputActionType.Button, DashBinding);
        }

        private void OnEnable()
        {
            _dashAction.Enable();
        }

        private void OnDisable()
        {
            _dashAction.Disable();
            _isDashing = false;
        }

        private void OnDestroy()
        {
            // 코드로 만든 액션은 에셋 소유가 아니라서 직접 해제해야 한다
            _dashAction.Dispose();
        }

        private void Update()
        {
            if (_isDashing)
            {
                ContinueDash();
                return;
            }

            if (_dashAction.WasPressedThisFrame() && Time.time >= _nextDashTime && !IsAimBlocking)
            {
                StartDash();
            }
        }

        // 조준 중에 누른 Shift는 쿨다운을 소모하지 않고 무시한다. 이미 시작한 대시는 중단하지 않는다
        private bool IsAimBlocking => _aimController != null && _aimController.IsAiming;

        private void StartDash()
        {
            // 돌진 중 시점을 돌려도 경로가 휘지 않도록 누른 순간의 방향으로 고정한다
            _dashDirection = transform.forward;

            _isDashing = true;
            _dashStartTime = Time.time;
            _lastProgress = 0f;
        }

        // 프레임마다 속도를 적분하면 프레임레이트에 따라 총 거리가 달라진다.
        // 그래서 시간에 대한 진행도를 계산하고 이전 프레임과의 차이만큼만 이동한다.
        // PlayerController도 같은 프레임에 Move를 호출하므로 두 이동량이 합쳐진다
        private void ContinueDash()
        {
            float t = Mathf.Clamp01((Time.time - _dashStartTime) / _dashDuration);

            // 끝으로 갈수록 느려지는 곡선이라 대시가 끝나도 일반 이동으로 부드럽게 이어진다
            float remaining = 1f - t;
            float progress = 1f - remaining * remaining;

            _controller.Move(_dashDirection * (_dashDistance * (progress - _lastProgress)));
            _lastProgress = progress;

            if (t >= 1f)
            {
                _isDashing = false;

                // 쿨다운은 대시가 끝난 시점부터 센다. 안 그러면 대시가 길수록 쿨다운이 짧아진다
                _nextDashTime = Time.time + _cooldown;
            }
        }
    }
}
