using ArenaBreak.Core;
using ArenaBreak.Player;
using UnityEngine;
using UnityEngine.UI;

namespace ArenaBreak.UI
{
    public class HUDController : MonoBehaviour
    {
        [Header("Listen")]
        [SerializeField] private Health _playerHealth;
        [SerializeField] private WeaponSystem _weaponSystem;
        [SerializeField] private WaveSpawner _waveSpawner;
        [SerializeField] private GameManager _gameManager;

        // fillAmount 대신 앵커 폭으로 줄이므로 앵커가 (0,0)~(1,1), 오프셋 0인 Image의 RectTransform이어야 한다
        [Header("Health Bar")]
        [SerializeField] private RectTransform _healthFill;

        [Header("Texts")]
        [SerializeField] private Text _ammoText;
        [SerializeField] private Text _waveText;
        [SerializeField] private Text _killText;

        [Header("Result")]
        [SerializeField] private GameObject _resultPanel;
        [SerializeField] private Text _resultTitleText;

        [Header("Formats")]
        [SerializeField] private string _ammoFormat = "{0} / {1}";
        [SerializeField] private string _waveFormat = "WAVE {0}";
        [SerializeField] private string _killFormat = "KILLS {0}";
        [SerializeField] private string _gameOverMessage = "GAME OVER";
        [SerializeField] private string _clearedMessage = "CLEARED";

        // OnEnable이 다시 불려도 웨이브 표시가 0으로 되돌아가지 않도록 마지막 값을 들고 있는다
        private int _wave;

        private void Awake()
        {
            if (_playerHealth == null || _weaponSystem == null || _waveSpawner == null || _gameManager == null
                || _healthFill == null || _ammoText == null || _waveText == null || _killText == null
                || _resultPanel == null || _resultTitleText == null)
            {
                Debug.LogError("HUDController: 모든 참조 칸을 할당해야 합니다.", this);
                enabled = false;
            }
        }

        private void OnEnable()
        {
            _playerHealth.HealthChanged += OnHealthChanged;
            _weaponSystem.AmmoChanged += OnAmmoChanged;
            _waveSpawner.WaveStarted += OnWaveStarted;
            _waveSpawner.EnemyKilled += OnEnemyKilled;
            _gameManager.StateChanged += OnStateChanged;

            // 구독 전에 정해진 값을 이벤트로는 받을 수 없어서 현재 상태를 한 번 읽는다
            UpdateHealth(_playerHealth.CurrentHealth, _playerHealth.MaxHealth);
            UpdateAmmo(_weaponSystem.CurrentAmmo, _weaponSystem.MagazineSize);
            UpdateWave(_wave);
            UpdateKills(_waveSpawner.KillCount);
            ApplyState(_gameManager.CurrentState);
        }

        // Health와 WeaponSystem은 Awake에서 현재값을 채우는데, OnEnable이 그보다 먼저 불릴 수 있어서 한 번 더 읽는다
        private void Start()
        {
            UpdateHealth(_playerHealth.CurrentHealth, _playerHealth.MaxHealth);
            UpdateAmmo(_weaponSystem.CurrentAmmo, _weaponSystem.MagazineSize);
        }

        private void OnDisable()
        {
            _playerHealth.HealthChanged -= OnHealthChanged;
            _weaponSystem.AmmoChanged -= OnAmmoChanged;
            _waveSpawner.WaveStarted -= OnWaveStarted;
            _waveSpawner.EnemyKilled -= OnEnemyKilled;
            _gameManager.StateChanged -= OnStateChanged;
        }

        private void OnHealthChanged(int current, int max)
        {
            UpdateHealth(current, max);
        }

        private void OnAmmoChanged(int current, int max)
        {
            UpdateAmmo(current, max);
        }

        private void OnWaveStarted(int waveNumber)
        {
            _wave = waveNumber;
            UpdateWave(waveNumber);
        }

        private void OnEnemyKilled(int totalKills)
        {
            UpdateKills(totalKills);
        }

        private void OnStateChanged(GameState state)
        {
            ApplyState(state);
        }

        private void UpdateHealth(int current, int max)
        {
            float ratio = max > 0 ? Mathf.Clamp01((float)current / max) : 0f;

            Vector2 anchorMax = _healthFill.anchorMax;
            anchorMax.x = ratio;
            _healthFill.anchorMax = anchorMax;
        }

        private void UpdateAmmo(int current, int max)
        {
            _ammoText.text = string.Format(_ammoFormat, current, max);
        }

        private void UpdateWave(int waveNumber)
        {
            _waveText.text = string.Format(_waveFormat, waveNumber);
        }

        private void UpdateKills(int totalKills)
        {
            _killText.text = string.Format(_killFormat, totalKills);
        }

        private void ApplyState(GameState state)
        {
            switch (state)
            {
                case GameState.GameOver:
                    _resultTitleText.text = _gameOverMessage;
                    _resultPanel.SetActive(true);
                    break;
                case GameState.Cleared:
                    _resultTitleText.text = _clearedMessage;
                    _resultPanel.SetActive(true);
                    break;
                case GameState.Ready:
                case GameState.Playing:
                    _resultPanel.SetActive(false);
                    break;
            }
        }
    }
}
