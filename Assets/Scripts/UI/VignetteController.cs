using ArenaBreak.Player;
using UnityEngine;
using UnityEngine.UI;

namespace ArenaBreak.UI
{
    // 어두워지는 모양은 이미지(스프라이트)가 정하고, 이 컴포넌트는 전체 농도(알파)만 조준 상태에 맞춰 바꾼다
    public class VignetteController : MonoBehaviour
    {
        // 목표 농도에 이만큼 가까워지면 전환이 끝난 것으로 보고 더 계산하지 않는다
        private const float SettleThreshold = 0.001f;

        [Header("Listen")]
        [SerializeField] private AimController _aimController;

        [Header("Target")]
        [Tooltip("화면 전체를 덮는 Image. 색(RGB)은 그대로 두고 알파만 바꾼다")]
        [SerializeField] private Image _vignetteImage;

        [Header("Intensity (0~1)")]
        [SerializeField, Range(0f, 1f)] private float _normalIntensity = 0.35f;
        [Tooltip("일반 상태보다 조금 높게 둔다")]
        [SerializeField, Range(0f, 1f)] private float _aimIntensity = 0.55f;

        [Header("Transition")]
        [SerializeField, Min(0f)] private float _transitionSpeed = 8f;

        private float _currentIntensity;
        private bool _isAiming;

        // 인스펙터에서 값을 바꿨을 때 다음 Update에서 한 번 다시 적용하도록 표시만 한다
        private bool _intensityDirty = true;

        private void Awake()
        {
            if (_aimController == null || _vignetteImage == null)
            {
                Debug.LogError("VignetteController: AimController와 Vignette Image를 할당해야 합니다.", this);
                enabled = false;
            }
        }

        private void OnEnable()
        {
            _aimController.AimStateChanged += OnAimStateChanged;

            // 구독 전에 이미 정해진 상태를 이벤트로는 받을 수 없어서 한 번 읽고, 전환 없이 바로 그 농도로 시작한다
            _isAiming = _aimController.IsAiming;
            _currentIntensity = TargetIntensity();
            ApplyIntensity(_currentIntensity);
        }

        private void OnDisable()
        {
            _aimController.AimStateChanged -= OnAimStateChanged;
        }

        private void OnValidate()
        {
            _intensityDirty = true;
        }

        private void Update()
        {
            float target = TargetIntensity();
            bool settled = Mathf.Abs(_currentIntensity - target) < SettleThreshold;

            if (settled && !_intensityDirty) return;

            // 지수 보간은 프레임레이트가 달라도 같은 속도로 따라간다
            _currentIntensity = settled ? target : Mathf.Lerp(_currentIntensity, target, 1f - Mathf.Exp(-_transitionSpeed * Time.deltaTime));
            _intensityDirty = false;

            ApplyIntensity(_currentIntensity);
        }

        private void OnAimStateChanged(bool aiming)
        {
            _isAiming = aiming;
        }

        private float TargetIntensity()
        {
            return _isAiming ? _aimIntensity : _normalIntensity;
        }

        private void ApplyIntensity(float intensity)
        {
            Color color = _vignetteImage.color;
            color.a = intensity;
            _vignetteImage.color = color;
        }
    }
}
