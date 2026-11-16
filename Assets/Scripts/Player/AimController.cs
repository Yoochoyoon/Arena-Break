using System.Globalization;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ArenaBreak.Player
{
    public class AimController : MonoBehaviour
    {
        private const string PlayerMapName = "Player";
        private const string LookActionName = "Look";
        private const string MoveActionName = "Move";
        private const string AimActionName = "Aim";
        private const string AimBinding = "<Mouse>/rightButton";
        private const string ScaleProcessorFormat = "ScaleVector2(x={0},y={0})";

        [Header("Input")]
        [SerializeField] private InputActionAsset _inputActions;

        [Header("Camera")]
        [SerializeField] private Camera _camera;

        [Header("Aim")]
        [SerializeField] private float _aimFieldOfView = 40f;
        [SerializeField, Range(0.05f, 1f)] private float _aimSensitivityMultiplier = 0.5f;
        [SerializeField, Range(0.05f, 1f)] private float _aimMoveSpeedMultiplier = 0.5f;
        [SerializeField] private float _fovTransitionSpeed = 12f;

        private InputAction _aimAction;
        private InputScaler _lookScaler;
        private InputScaler _moveScaler;

        private float _defaultFieldOfView;
        private bool _isAiming;

        public bool IsAiming => _isAiming;

        private void Awake()
        {
            if (_inputActions == null || _camera == null)
            {
                Debug.LogError("AimController: InputActionAsset과 Camera를 할당해야 합니다.", this);
                enabled = false;
                return;
            }

            InputActionMap playerMap = _inputActions.FindActionMap(PlayerMapName);
            InputAction lookAction = playerMap?.FindAction(LookActionName);
            InputAction moveAction = playerMap?.FindAction(MoveActionName);
            if (lookAction == null || moveAction == null)
            {
                Debug.LogError($"AimController: '{PlayerMapName}' 맵에 '{LookActionName}'와 '{MoveActionName}' 액션이 필요합니다.", this);
                enabled = false;
                return;
            }

            // 에셋에 우클릭 액션이 없어서 코드에서 직접 만든다
            _aimAction = new InputAction(AimActionName, InputActionType.Button, AimBinding);

            // 원래 FOV를 하드코딩하지 않고 시작 시점의 카메라 값을 기준으로 삼는다
            _defaultFieldOfView = _camera.fieldOfView;

            _lookScaler = new InputScaler(lookAction, _aimSensitivityMultiplier);
            _moveScaler = new InputScaler(moveAction, _aimMoveSpeedMultiplier);
        }

        private void OnEnable()
        {
            if (_aimAction == null) return;

            _aimAction.Enable();
        }

        private void OnDisable()
        {
            if (_aimAction == null) return;

            if (_isAiming)
            {
                SetAiming(false);
            }

            if (_camera != null)
            {
                _camera.fieldOfView = _defaultFieldOfView;
            }

            _aimAction.Disable();
        }

        private void OnDestroy()
        {
            // 코드로 만든 액션은 에셋 소유가 아니라서 직접 해제해야 한다
            _aimAction?.Dispose();
        }

        private void Update()
        {
            bool wantsAim = _aimAction.IsPressed();
            if (wantsAim != _isAiming)
            {
                SetAiming(wantsAim);
            }

            UpdateFieldOfView();
        }

        // 감도와 이동 속도는 PlayerController의 private 필드라 직접 못 바꾼다.
        // 대신 PlayerController가 읽는 입력값 자체를 줄여서, 같은 입력을 읽는 쪽이 낮아진 값을 받게 한다
        private void SetAiming(bool aiming)
        {
            _isAiming = aiming;

            _lookScaler.SetScaled(aiming);
            _moveScaler.SetScaled(aiming);
        }

        private void UpdateFieldOfView()
        {
            float target = _isAiming ? _aimFieldOfView : _defaultFieldOfView;

            // 지수 보간은 프레임레이트가 달라도 같은 속도로 따라간다
            float blend = 1f - Mathf.Exp(-_fovTransitionSpeed * Time.deltaTime);
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, target, blend);
        }

        // 액션의 바인딩마다 ScaleVector2 프로세서를 덧씌우거나 걷어낸다.
        // 문자열은 조준을 시작할 때마다 만들지 않도록 생성 시점에 미리 계산한다
        private sealed class InputScaler
        {
            private readonly InputAction _action;
            private readonly string[] _scaledProcessors;

            public InputScaler(InputAction action, float multiplier)
            {
                _action = action;

                string scale = string.Format(CultureInfo.InvariantCulture, ScaleProcessorFormat, multiplier);

                int bindingCount = action.bindings.Count;
                _scaledProcessors = new string[bindingCount];

                for (int i = 0; i < bindingCount; i++)
                {
                    InputBinding binding = action.bindings[i];

                    // 컴포지트의 구성 요소에는 프로세서가 적용되지 않고 컴포지트 자체에 적용된다
                    if (binding.isPartOfComposite) continue;

                    _scaledProcessors[i] = string.IsNullOrEmpty(binding.processors)
                        ? scale
                        : binding.processors + "," + scale;
                }
            }

            public void SetScaled(bool scaled)
            {
                for (int i = 0; i < _scaledProcessors.Length; i++)
                {
                    if (_scaledProcessors[i] == null) continue;

                    if (scaled)
                    {
                        _action.ApplyBindingOverride(i, new InputBinding { overrideProcessors = _scaledProcessors[i] });
                    }
                    else
                    {
                        _action.RemoveBindingOverride(i);
                    }
                }
            }
        }
    }
}
