using System.Collections;
using ArenaBreak.Core;
using UnityEngine;

namespace ArenaBreak.Player
{
    public class CameraShake : MonoBehaviour
    {
        [Header("Listen")]
        [SerializeField] private Health _playerHealth;

        [Header("Target")]
        [Tooltip("흔들 카메라. PlayerController가 매 프레임 회전을 덮어쓰므로 위치만 흔든다")]
        [SerializeField] private Transform _cameraTransform;

        [Header("Shake")]
        [SerializeField, Min(0f)] private float _duration = 0.1f;
        [Tooltip("카메라가 원래 위치에서 벗어나는 최대 거리(m)")]
        [SerializeField, Min(0f)] private float _intensity = 0.05f;

        private Vector3 _restPosition;
        private Coroutine _shakeRoutine;

        // 체력이 늘어나는 이벤트에는 흔들리지 않도록 직전 값과 비교한다.
        // 처음 한 번은 무조건 감소로 보이게 최댓값으로 시작한다
        private int _lastHealth = int.MaxValue;

        private void Awake()
        {
            if (_playerHealth == null || _cameraTransform == null)
            {
                Debug.LogError("CameraShake: Player Health와 Camera Transform을 할당해야 합니다.", this);
                enabled = false;
                return;
            }

            _restPosition = _cameraTransform.localPosition;
        }

        private void OnEnable()
        {
            _playerHealth.HealthChanged += OnHealthChanged;
        }

        private void OnDisable()
        {
            _playerHealth.HealthChanged -= OnHealthChanged;

            // 흔드는 도중에 꺼지면 카메라가 어긋난 위치에 남는다
            StopShake();
        }

        private void OnHealthChanged(int current, int max)
        {
            bool tookDamage = current < _lastHealth;
            _lastHealth = current;

            if (!tookDamage) return;

            if (_shakeRoutine != null)
            {
                StopCoroutine(_shakeRoutine);
            }

            _shakeRoutine = StartCoroutine(ShakeRoutine());
        }

        // 사망하면 GameManager가 timeScale을 0으로 만들기 때문에 scaled 시간을 쓰면 흔들린 채로 멈춘다
        private IEnumerator ShakeRoutine()
        {
            float elapsed = 0f;

            while (elapsed < _duration)
            {
                float falloff = 1f - elapsed / _duration;
                _cameraTransform.localPosition = _restPosition + Random.insideUnitSphere * (_intensity * falloff);

                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            StopShake();
        }

        private void StopShake()
        {
            _shakeRoutine = null;

            // 씬이 정리되는 중에는 카메라가 먼저 파괴되어 있을 수 있다
            if (_cameraTransform == null) return;

            _cameraTransform.localPosition = _restPosition;
        }
    }
}
