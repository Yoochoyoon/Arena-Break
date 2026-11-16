using UnityEngine;
using UnityEngine.InputSystem;

namespace ArenaBreak.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        private const string PlayerMapName = "Player";
        private const string MoveActionName = "Move";
        private const string LookActionName = "Look";
        private const string JumpActionName = "Jump";

        [Header("Input")]
        [SerializeField] private InputActionAsset _inputActions;

        [Header("Camera")]
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private float _minPitch = -80f;
        [SerializeField] private float _maxPitch = 80f;

        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 6f;
        [SerializeField] private float _jumpHeight = 1.5f;
        [SerializeField] private float _gravity = -20f;
        // isGrounded는 매 프레임 바닥에 살짝 눌려 있어야 안정적으로 true가 된다
        [SerializeField] private float _groundedStickVelocity = -2f;

        [Header("Look")]
        [SerializeField] private float _lookSensitivity = 0.1f;
        // 커서 잠금 첫 프레임에 delta가 크게 튀는 현상을 막는다
        [SerializeField] private float _maxLookDeltaPerFrame = 50f;

        [Header("Cursor")]
        [SerializeField] private bool _lockCursor = true;

        private CharacterController _controller;
        private InputActionMap _playerMap;
        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _jumpAction;

        private float _pitch;
        private float _verticalVelocity;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();

            if (_inputActions == null || _cameraTransform == null)
            {
                Debug.LogError("PlayerController: InputActionAsset과 Camera Transform을 할당해야 합니다.", this);
                enabled = false;
                return;
            }

            _playerMap = _inputActions.FindActionMap(PlayerMapName);
            if (_playerMap == null)
            {
                Debug.LogError($"PlayerController: '{PlayerMapName}' 액션 맵을 찾을 수 없습니다.", this);
                enabled = false;
                return;
            }

            _moveAction = _playerMap.FindAction(MoveActionName);
            _lookAction = _playerMap.FindAction(LookActionName);
            _jumpAction = _playerMap.FindAction(JumpActionName);

            if (_moveAction == null || _lookAction == null || _jumpAction == null)
            {
                Debug.LogError($"PlayerController: '{PlayerMapName}' 맵에 Move/Look/Jump 액션이 모두 필요합니다.", this);
                enabled = false;
            }
        }

        private void OnEnable()
        {
            if (_playerMap == null) return;

            _playerMap.Enable();

            if (_lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void OnDisable()
        {
            if (_playerMap != null)
            {
                _playerMap.Disable();
            }

            if (_lockCursor)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        private void Update()
        {
            HandleLook();
            HandleMovement();
        }

        private void HandleLook()
        {
            Vector2 delta = Vector2.ClampMagnitude(_lookAction.ReadValue<Vector2>(), _maxLookDeltaPerFrame);

            // 마우스 delta는 이미 프레임 단위 이동량이라 deltaTime을 곱하면 프레임레이트에 따라 감도가 달라진다
            transform.Rotate(0f, delta.x * _lookSensitivity, 0f);

            _pitch = Mathf.Clamp(_pitch - delta.y * _lookSensitivity, _minPitch, _maxPitch);
            _cameraTransform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }

        private void HandleMovement()
        {
            Vector2 moveInput = _moveAction.ReadValue<Vector2>();
            Vector3 horizontal = (transform.right * moveInput.x + transform.forward * moveInput.y) * _moveSpeed;

            if (_controller.isGrounded && _verticalVelocity < 0f)
            {
                _verticalVelocity = _groundedStickVelocity;
            }

            if (_controller.isGrounded && _jumpAction.WasPressedThisFrame())
            {
                _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
            }

            _verticalVelocity += _gravity * Time.deltaTime;

            Vector3 velocity = horizontal + Vector3.up * _verticalVelocity;
            _controller.Move(velocity * Time.deltaTime);
        }
    }
}
