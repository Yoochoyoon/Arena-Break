using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace ArenaBreak.Player
{
    // 총구 화염·연기·탄피를 한곳에서 발사 순간에 재생한다. 명중 여부와 무관해야 하므로 ShotHit이 아니라 ShotFired를 쓴다
    public class ShotEffects : MonoBehaviour
    {
        private struct ActiveShell
        {
            public Rigidbody Body;
            public float ReleaseTime;
        }

        [Header("Listen")]
        [SerializeField] private WeaponSystem _weaponSystem;

        [Header("Muzzle")]
        [Tooltip("화염과 연기가 나오는 위치와 방향. 이 Transform의 앞쪽(+Z)으로 나간다")]
        [SerializeField] private Transform _muzzle;
        [SerializeField] private ParticleSystem _muzzleFlash;
        [SerializeField] private ParticleSystem _muzzleSmoke;

        [Header("Flash")]
        [SerializeField, Min(1)] private int _flashParticleCount = 3;
        [Tooltip("화염이 보이는 시간(초). 길면 카메라 앞을 가린다")]
        [SerializeField, Min(0.01f)] private float _flashDuration = 0.05f;

        [Header("Smoke")]
        [Tooltip("발사 한 번에 나오는 연기 입자 수. 연사하면 쌓이므로 적게 둔다")]
        [SerializeField, Min(0)] private int _smokeParticleCount = 6;
        [Tooltip("연기가 사라지기까지의 시간(초)")]
        [SerializeField, Min(0.01f)] private float _smokeDuration = 0.8f;

        [Header("Shell")]
        [Tooltip("탄피가 튀어나오는 위치. 이 Transform 기준으로 방향을 해석한다")]
        [SerializeField] private Transform _shellEjectPoint;
        [Tooltip("Rigidbody와 콜라이더가 있는 탄피 프리팹")]
        [SerializeField] private Rigidbody _shellPrefab;
        [Tooltip("배출 지점 기준 로컬 방향. 기본은 오른쪽 위 약간 뒤")]
        [SerializeField] private Vector3 _ejectDirection = new Vector3(1f, 1f, -0.2f);
        [SerializeField, Min(0f)] private float _ejectForce = 2.5f;
        [SerializeField, Min(0f)] private float _spinForce = 15f;
        [Tooltip("탄피가 사라지기까지의 시간(초)")]
        [SerializeField, Min(0.1f)] private float _shellLifetime = 3f;
        [Tooltip("동시에 남아 있을 수 있는 탄피 수. 넘으면 가장 오래된 것부터 재사용한다")]
        [SerializeField, Min(1)] private int _maxShells = 32;

        private readonly Queue<ActiveShell> _activeShells = new Queue<ActiveShell>();
        private ObjectPool<Rigidbody> _shellPool;
        private Transform _shellContainer;

        private void Awake()
        {
            if (_weaponSystem == null || _muzzle == null || _muzzleFlash == null || _muzzleSmoke == null
                || _shellEjectPoint == null || _shellPrefab == null)
            {
                Debug.LogError("ShotEffects: WeaponSystem, 총구, 화염, 연기, 탄피 배출 위치, 탄피 프리팹을 모두 할당해야 합니다.", this);
                enabled = false;
                return;
            }

            // 탄피가 총알 레이캐스트를 막으면 탄피 뒤의 적을 맞힐 수 없다. 씬에 저장된 WeaponSystem의 Hit Mask에 기대고 있어서 어긋나면 알린다
            int shellLayer = _shellPrefab.gameObject.layer;
            if ((_weaponSystem.HitMask.value & (1 << shellLayer)) != 0)
            {
                Debug.LogError($"ShotEffects: 탄피 레이어 '{LayerMask.LayerToName(shellLayer)}'가 WeaponSystem의 Hit Mask에 포함되어 있어 탄피가 총알을 막습니다. Hit Mask에서 이 레이어를 빼세요.", this);
            }

            // 지속 시간은 파티클 프리팹을 열지 않고 여기서 조절하려고 수명에 덮어쓴다
            ParticleSystem.MainModule flashMain = _muzzleFlash.main;
            flashMain.startLifetime = _flashDuration;
            ParticleSystem.MainModule smokeMain = _muzzleSmoke.main;
            smokeMain.startLifetime = _smokeDuration;

            // 탄피를 플레이어 자식으로 두면 플레이어가 움직일 때 같이 끌려가서 월드에 따로 모아 둔다
            _shellContainer = new GameObject("Shells").transform;

            _shellPool = new ObjectPool<Rigidbody>(
                createFunc: () => Instantiate(_shellPrefab, _shellContainer),
                actionOnGet: shell => shell.gameObject.SetActive(true),
                actionOnRelease: ReleaseShell,
                actionOnDestroy: DestroyShell,
                collectionCheck: true,
                defaultCapacity: _maxShells,
                maxSize: _maxShells);
        }

        private void OnEnable()
        {
            _weaponSystem.ShotFired += OnShotFired;
        }

        private void OnDisable()
        {
            _weaponSystem.ShotFired -= OnShotFired;

            // 날아가던 탄피가 허공에 멈춘 채 남지 않게 모두 돌려보낸다.
            // 플레이를 멈추거나 씬이 사라질 때는 탄피가 이미 파괴되어 있을 수 있어서 건너뛴다
            while (_activeShells.Count > 0)
            {
                Rigidbody body = _activeShells.Dequeue().Body;
                if (body != null)
                {
                    _shellPool.Release(body);
                }
            }
        }

        private void OnDestroy()
        {
            _shellPool?.Dispose();

            if (_shellContainer != null)
            {
                Destroy(_shellContainer.gameObject);
            }
        }

        private void Update()
        {
            // 수명이 모두 같아서 먼저 나온 탄피가 먼저 사라진다
            while (_activeShells.Count > 0 && Time.time >= _activeShells.Peek().ReleaseTime)
            {
                _shellPool.Release(_activeShells.Dequeue().Body);
            }
        }

        // 씬이 정리되는 중에는 풀이 들고 있던 탄피가 먼저 파괴되어 있을 수 있다
        private static void ReleaseShell(Rigidbody shell)
        {
            if (shell != null)
            {
                shell.gameObject.SetActive(false);
            }
        }

        private static void DestroyShell(Rigidbody shell)
        {
            if (shell != null)
            {
                Destroy(shell.gameObject);
            }
        }

        private void OnShotFired()
        {
            EmitMuzzleEffects();
            EjectShell();
        }

        // Emit은 재생 상태와 상관없이 즉시 내보내서 연사 중에도 매 발 새로 터진다
        private void EmitMuzzleEffects()
        {
            _muzzleFlash.transform.SetPositionAndRotation(_muzzle.position, _muzzle.rotation);
            _muzzleSmoke.transform.SetPositionAndRotation(_muzzle.position, _muzzle.rotation);

            _muzzleFlash.Emit(_flashParticleCount);
            _muzzleSmoke.Emit(_smokeParticleCount);
        }

        private void EjectShell()
        {
            // 상한에 닿으면 가장 오래된 탄피를 먼저 돌려보내서 새로 만들지 않고 재사용한다
            if (_activeShells.Count >= _maxShells)
            {
                _shellPool.Release(_activeShells.Dequeue().Body);
            }

            Rigidbody shell = _shellPool.Get();
            shell.transform.SetPositionAndRotation(_shellEjectPoint.position, _shellEjectPoint.rotation);

            Vector3 direction = _shellEjectPoint.TransformDirection(_ejectDirection.normalized);
            shell.linearVelocity = direction * _ejectForce;
            shell.angularVelocity = Random.onUnitSphere * _spinForce;

            _activeShells.Enqueue(new ActiveShell { Body = shell, ReleaseTime = Time.time + _shellLifetime });
        }
    }
}
