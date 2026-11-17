using ArenaBreak.Player;
using UnityEngine;
using UnityEngine.UI;

namespace ArenaBreak.UI
{
    // 조준점(CrosshairController)은 조준 상태에 따라 모양이 변하고, 이 컴포넌트는 데미지가 들어간 순간의 X 표시만 맡는다
    public class HitMarkerController : MonoBehaviour
    {
        public enum RefreshMode
        {
            [Tooltip("명중할 때마다 처음부터 다시 보여 준다")]
            Restart,
            [Tooltip("표시 중에는 새 명중을 무시하고 한 번 보여 준 뒤 사라지면 다음 명중부터 다시 보여 준다")]
            IgnoreWhileVisible
        }

        [Header("Listen")]
        [SerializeField] private DamageDealer _damageDealer;

        [Header("X Marker")]
        [Tooltip("알파로 서서히 투명해지게 하는 그룹. 전체를 감싸는 오브젝트에 붙인다")]
        [SerializeField] private CanvasGroup _group;
        [Tooltip("X의 한 획. 이 Image를 +45도로 회전시켜 둔다")]
        [SerializeField] private Image _barA;
        [Tooltip("X의 다른 획. 이 Image를 -45도로 회전시켜 둔다")]
        [SerializeField] private Image _barB;

        [Header("Look")]
        [SerializeField, Min(0f)] private float _markerLength = 24f;
        [SerializeField, Min(0f)] private float _markerThickness = 3f;
        [SerializeField] private Color _color = Color.white;

        [Header("Timing")]
        [Tooltip("완전히 보이는 상태로 유지되는 시간(초)")]
        [SerializeField, Min(0f)] private float _displayDuration = 0.08f;
        [Tooltip("초당 줄어드는 알파. 클수록 빨리 사라진다")]
        [SerializeField, Min(0.01f)] private float _fadeSpeed = 6f;
        [SerializeField] private RefreshMode _refreshMode = RefreshMode.Restart;

        private float _holdTimer;
        private bool _isShowing;

        // 인스펙터에서 값을 바꿨을 때 다음 Update에서 한 번 다시 적용하도록 표시만 한다
        private bool _styleDirty = true;

        private void Awake()
        {
            if (_damageDealer == null || _group == null || _barA == null || _barB == null)
            {
                Debug.LogError("HitMarkerController: DamageDealer, CanvasGroup, 두 획(Image)을 모두 할당해야 합니다.", this);
                enabled = false;
            }
        }

        private void OnEnable()
        {
            _damageDealer.DamageDealt += OnDamageDealt;

            ApplyStyle();
            Hide();
        }

        private void OnDisable()
        {
            _damageDealer.DamageDealt -= OnDamageDealt;

            Hide();
        }

        private void OnValidate()
        {
            _styleDirty = true;
        }

        private void Update()
        {
            if (_styleDirty)
            {
                ApplyStyle();
            }

            if (!_isShowing) return;

            if (_holdTimer > 0f)
            {
                _holdTimer -= Time.deltaTime;
                return;
            }

            _group.alpha = Mathf.MoveTowards(_group.alpha, 0f, _fadeSpeed * Time.deltaTime);
            if (_group.alpha <= 0f)
            {
                _isShowing = false;
            }
        }

        private void OnDamageDealt(RaycastHit hit)
        {
            if (_refreshMode == RefreshMode.IgnoreWhileVisible && _isShowing) return;

            // 새 오브젝트를 만들지 않고 하나뿐인 표시를 되돌려 쓰므로 연속 명중에도 겹치지 않는다
            _isShowing = true;
            _holdTimer = _displayDuration;
            _group.alpha = 1f;
        }

        private void Hide()
        {
            _isShowing = false;
            _holdTimer = 0f;
            _group.alpha = 0f;
        }

        private void ApplyStyle()
        {
            _styleDirty = false;

            _barA.rectTransform.sizeDelta = new Vector2(_markerLength, _markerThickness);
            _barB.rectTransform.sizeDelta = new Vector2(_markerLength, _markerThickness);
            _barA.color = _color;
            _barB.color = _color;
        }
    }
}
