using ArenaBreak.Core;
using UnityEngine;

namespace ArenaBreak.Enemy
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    public class EnemyProjectile : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _speed = 12f;
        [SerializeField, Min(0)] private int _damage = 10;
        [SerializeField, Min(0.1f)] private float _lifetime = 5f;

        private Rigidbody _rigidbody;
        private Transform _owner;
        private Transform _target;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();

            // 프리팹을 만들 때 설정을 빠뜨려도 동작하도록 코드에서 강제한다
            _rigidbody.useGravity = false;
            GetComponent<SphereCollider>().isTrigger = true;
        }

        // 발사한 적과 맞출 대상을 받아서, 발사한 적에게 맞거나 다른 적을 다치게 하는 일을 막는다
        public void Launch(Vector3 direction, Transform owner, Transform target)
        {
            _owner = owner;
            _target = target;

            transform.rotation = Quaternion.LookRotation(direction);
            _rigidbody.linearVelocity = direction * _speed;

            Destroy(gameObject, _lifetime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_owner != null && other.transform.IsChildOf(_owner)) return;

            if (_target != null && other.transform.IsChildOf(_target))
            {
                IDamageable damageable = other.GetComponentInParent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(_damage);
                }
            }

            // 맞은 것이 플레이어든 벽이든 투사체는 여기서 끝난다
            Destroy(gameObject);
        }
    }
}
