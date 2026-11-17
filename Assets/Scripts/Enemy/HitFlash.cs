using System.Collections;
using ArenaBreak.Core;
using UnityEngine;

namespace ArenaBreak.Enemy
{
    [RequireComponent(typeof(Health))]
    public class HitFlash : MonoBehaviour
    {
        // URP Lit의 기본 색 프로퍼티. 다른 셰이더로 바꾸면 이 이름도 맞춰야 한다
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

        [Tooltip("비워 두면 이 오브젝트나 자식에서 첫 Renderer를 찾는다")]
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Color _flashColor = Color.white;
        [SerializeField, Min(0f)] private float _flashDuration = 0.05f;

        private Health _health;
        private MaterialPropertyBlock _block;
        private WaitForSeconds _flashWait;
        private Coroutine _flashRoutine;

        private void Awake()
        {
            _health = GetComponent<Health>();

            if (_renderer == null)
            {
                _renderer = GetComponentInChildren<Renderer>();
            }

            if (_renderer == null)
            {
                Debug.LogError("HitFlash: Renderer를 찾을 수 없습니다.", this);
                enabled = false;
                return;
            }

            // 머티리얼 자체를 바꾸면 같은 머티리얼을 쓰는 다른 적까지 같이 하얘져서 렌더러에만 덮어씌운다
            _block = new MaterialPropertyBlock();
            _flashWait = new WaitForSeconds(_flashDuration);
        }

        private void OnEnable()
        {
            _health.HealthChanged += OnHealthChanged;
        }

        private void OnDisable()
        {
            _health.HealthChanged -= OnHealthChanged;

            // 풀로 돌아간 적이 하얀 채로 다시 나오지 않게 한다
            ClearFlash();
        }

        // 체력이 줄었을 때만 깜빡인다. Restore는 최대 체력으로 이벤트를 보내서 current < max로 걸러낸다
        private void OnHealthChanged(int current, int max)
        {
            if (current >= max) return;

            if (_flashRoutine != null)
            {
                StopCoroutine(_flashRoutine);
            }

            _flashRoutine = StartCoroutine(FlashRoutine());
        }

        private IEnumerator FlashRoutine()
        {
            _renderer.GetPropertyBlock(_block);
            _block.SetColor(BaseColorId, _flashColor);
            _renderer.SetPropertyBlock(_block);

            yield return _flashWait;

            ClearFlash();
            _flashRoutine = null;
        }

        private void ClearFlash()
        {
            if (_renderer == null) return;

            _renderer.SetPropertyBlock(null);
        }
    }
}
