using System;
using System.Collections;
using ArenaBreak.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace ArenaBreak.Core
{
    public enum GameState
    {
        Ready,
        Playing,
        GameOver,
        Cleared
    }

    public class GameManager : MonoBehaviour
    {
        private const string RestartActionName = "Restart";
        private const string RestartBinding = "<Keyboard>/r";
        private const string QuitActionName = "Quit";
        private const string QuitBinding = "<Keyboard>/escape";

        [Header("Flow")]
        [Tooltip("플레이 시작 후 Playing으로 넘어가기까지 기다리는 시간(초)")]
        [SerializeField, Min(0f)] private float _readyDuration = 2f;

        [Header("Listen")]
        [SerializeField] private Health _playerHealth;
        [SerializeField] private WaveSpawner _waveSpawner;

        // 배열로 묶으면 타입이 넓어져 인스펙터에 엉뚱한 컴포넌트가 들어갈 수 있어서 각각 받는다
        [Header("Stop On End")]
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private WeaponSystem _weaponSystem;

        [Header("Fall")]
        [Tooltip("플레이어가 이 높이보다 아래로 내려가면 사망으로 처리한다. 바닥(y=0)보다 충분히 낮게 둔다")]
        [SerializeField] private float _fallDeathHeight = -10f;

        public event Action<GameState> StateChanged;

        public GameState CurrentState => _state;

        private GameState _state = GameState.Ready;
        private InputAction _restartAction;
        private InputAction _quitAction;
        private Transform _playerTransform;

        private void Awake()
        {
            if (_playerHealth == null || _waveSpawner == null || _playerController == null || _weaponSystem == null)
            {
                Debug.LogError("GameManager: Player Health, WaveSpawner, PlayerController, WeaponSystem을 모두 할당해야 합니다.", this);
                enabled = false;
                return;
            }

            _playerTransform = _playerHealth.transform;

            // 입력 에셋에 R/Esc 액션이 없어서 코드에서 직접 만든다
            _restartAction = new InputAction(RestartActionName, InputActionType.Button, RestartBinding);
            _quitAction = new InputAction(QuitActionName, InputActionType.Button, QuitBinding);
        }

        private void OnEnable()
        {
            if (_restartAction == null) return;

            _playerHealth.Died += HandlePlayerDied;
            _waveSpawner.AllWavesCleared += HandleAllWavesCleared;

            _restartAction.performed += OnRestartPerformed;
            _quitAction.performed += OnQuitPerformed;
            _restartAction.Enable();
            _quitAction.Enable();
        }

        private void OnDisable()
        {
            if (_restartAction == null) return;

            _playerHealth.Died -= HandlePlayerDied;
            _waveSpawner.AllWavesCleared -= HandleAllWavesCleared;

            _restartAction.performed -= OnRestartPerformed;
            _quitAction.performed -= OnQuitPerformed;
            _restartAction.Disable();
            _quitAction.Disable();
        }

        private void OnDestroy()
        {
            // 코드로 만든 액션은 에셋 소유가 아니라서 직접 해제해야 한다
            _restartAction?.Dispose();
            _quitAction?.Dispose();

            // 종료 상태에서 플레이를 멈추거나 씬이 사라져도 timeScale 0이 다음 플레이로 새지 않게 한다
            Time.timeScale = 1f;
        }

        private void Start()
        {
            StartCoroutine(RunReady());
        }

        private void Update()
        {
            if (IsEnded() || _playerTransform.position.y >= _fallDeathHeight) return;

            // 낙사도 체력으로 죽여야 Died 이벤트 하나로 GameOver와 HUD가 같은 경로를 탄다
            _playerHealth.TakeDamage(_playerHealth.MaxHealth);
        }

        private IEnumerator RunReady()
        {
            yield return new WaitForSeconds(_readyDuration);

            // 대기 중에 이미 끝난 게임이 Playing으로 되돌아가면 안 된다
            if (_state == GameState.Ready)
            {
                ChangeState(GameState.Playing);
            }
        }

        private void HandlePlayerDied()
        {
            if (IsEnded()) return;

            ChangeState(GameState.GameOver);
        }

        private void HandleAllWavesCleared()
        {
            // 같은 프레임에 플레이어가 죽었다면 먼저 일어난 쪽을 결과로 둔다
            if (IsEnded()) return;

            ChangeState(GameState.Cleared);
        }

        private void ChangeState(GameState next)
        {
            _state = next;

            switch (next)
            {
                case GameState.Ready:
                case GameState.Playing:
                    break;
                case GameState.GameOver:
                case GameState.Cleared:
                    EnterEndState();
                    break;
            }

            StateChanged?.Invoke(next);
        }

        // timeScale 0은 Update를 멈추지 않아서, 입력 컴포넌트를 꺼야 죽은 뒤에 총이 나가지 않는다
        private void EnterEndState()
        {
            _playerController.enabled = false;
            _weaponSystem.enabled = false;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Time.timeScale = 0f;
        }

        private void OnRestartPerformed(InputAction.CallbackContext context)
        {
            if (!IsEnded()) return;

            // 0인 채로 로드하면 새로 시작한 씬도 멈춰 있다
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void OnQuitPerformed(InputAction.CallbackContext context)
        {
            Application.Quit();
        }

        private bool IsEnded()
        {
            return _state == GameState.GameOver || _state == GameState.Cleared;
        }
    }
}
