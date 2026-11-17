using ArenaBreak.Player;
using UnityEngine;

namespace ArenaBreak.UI
{
    // 십자선 위치는 화면 중앙 앵커 기준이라 해상도와 무관하고, 반동으로 카메라가 움직여도 레이는 항상 화면 중앙을 지나므로 따로 보정하지 않는다
    public class CrosshairController : MonoBehaviour
    {
        // 목표 간격에 이만큼 가까워지면 전환이 끝난 것으로 보고 더 계산하지 않는다
        private const float SettleThreshold = 0.01f;

        [Header("Listen")]
        [SerializeField] private AimController _aimController;

        [Header("Lines (anchor/pivot center)")]
        [SerializeField] private RectTransform _top;
        [SerializeField] private RectTransform _bottom;
        [SerializeField] private RectTransform _left;
        [SerializeField] private RectTransform _right;

        [Header("Shape")]
        [SerializeField, Min(0f)] private float _lineLength = 10f;
        [SerializeField, Min(0f)] private float _lineThickness = 2f;

        [Header("Gap (center to line start)")]
        [SerializeField, Min(0f)] private float _normalGap = 12f;
        [Tooltip("일반 상태보다 좁아야 십자선이 모이는 것처럼 보인다")]
        [SerializeField, Min(0f)] private float _aimGap = 3f;

        [Header("Transition")]
        [SerializeField, Min(0f)] private float _transitionSpeed = 14f;

        private float _currentGap;
        private bool _isAiming;

        // 인스펙터에서 값을 바꿨을 때 다음 Update에서 한 번 다시 배치하도록 표시만 한다
        private bool _layoutDirty = true;

        private void Awake()
        {
            if (_aimController == null || _top == null || _bottom == null || _left == null || _right == null)
            {
                Debug.LogError("CrosshairController: AimController와 위·아래·왼쪽·오른쪽 선을 모두 할당해야 합니다.", this);
                enabled = false;
            }
        }

        private void OnEnable()
        {
            _aimController.AimStateChanged += OnAimStateChanged;

            // 구독 전에 이미 정해진 상태를 이벤트로는 받을 수 없어서 한 번 읽고, 전환 없이 바로 그 간격에 둔다
            _isAiming = _aimController.IsAiming;
            _currentGap = TargetGap();
            ApplyLayout(_currentGap);
        }

        private void OnDisable()
        {
            _aimController.AimStateChanged -= OnAimStateChanged;
        }

        private void OnValidate()
        {
            _layoutDirty = true;
        }

        private void Update()
        {
            float target = TargetGap();
            bool settled = Mathf.Abs(_currentGap - target) < SettleThreshold;

            if (settled && !_layoutDirty) return;

            // 지수 보간은 프레임레이트가 달라도 같은 속도로 따라간다
            _currentGap = settled ? target : Mathf.Lerp(_currentGap, target, 1f - Mathf.Exp(-_transitionSpeed * Time.deltaTime));
            _layoutDirty = false;

            ApplyLayout(_currentGap);
        }

        private void OnAimStateChanged(bool aiming)
        {
            _isAiming = aiming;
        }

        private float TargetGap()
        {
            return _isAiming ? _aimGap : _normalGap;
        }

        // 선의 중심을 (간격 + 길이의 절반) 만큼 중앙에서 떨어뜨려서 선의 안쪽 끝이 정확히 간격만큼 떨어지게 한다
        private void ApplyLayout(float gap)
        {
            float offset = gap + _lineLength * 0.5f;

            Place(_top, new Vector2(0f, offset), new Vector2(_lineThickness, _lineLength));
            Place(_bottom, new Vector2(0f, -offset), new Vector2(_lineThickness, _lineLength));
            Place(_left, new Vector2(-offset, 0f), new Vector2(_lineLength, _lineThickness));
            Place(_right, new Vector2(offset, 0f), new Vector2(_lineLength, _lineThickness));
        }

        private static void Place(RectTransform line, Vector2 position, Vector2 size)
        {
            line.anchoredPosition = position;
            line.sizeDelta = size;
        }
    }
}
