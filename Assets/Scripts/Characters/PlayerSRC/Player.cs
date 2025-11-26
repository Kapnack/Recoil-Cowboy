using System;
using System.Collections;
using Characters.EnemySRC;
using MouseTracker;
using Particle;
using ScriptableObjects;
using Systems;
using Systems.CentralizeEventSystem;
using Systems.Pool;
using UnityEngine;
using UnityEngine.Serialization;

namespace Characters.PlayerSRC
{
    public delegate void Reload360();
    public delegate void InvincibilityOff();
    [RequireComponent(typeof(Rigidbody))]
    public class Player : Character, IPlayerHealthSystem
    {
        [FormerlySerializedAs("_config")] [SerializeField]
        private PlayerConfig config;

        [ContextMenuItem("Instant Kill", nameof(InstantDead))]
        private int _currentLives;

        [SerializeField] private Transform gunPos;
        private Vector3 _initialPos;

        [SerializeField] private GameObject bulletParticlePrefab;
        private IObjectPool<ParticleController> _particlePool;
        
        private int _killPoints;
        private int _distancePoints;

        private bool _isDead;

        private int _currentBullets;

        private Coroutine _reloadingCoroutine;

        private IMousePositionTracker _mouseTracker;

        private ICharacterSpin _spin;

        private CentralizeEventSystem _eventSystem;
        
        public bool Invincible { get; private set; }

        private int CurrentLives
        {
            get => _currentLives;

            set
            {
                if (config.InfinitLives)
                    return;

                int newValue = Mathf.Clamp(value, 0, config.MaxLives);

                _eventSystem.Get<LivesChange>()?.Invoke(_currentLives, newValue, config.MaxLives);
                _currentLives = newValue;

                switch (newValue)
                {
                    case 1:
                        _eventSystem.Get<AlmostDead>()?.Invoke();
                        break;

                    case 0 when !_isDead:
                        OnDead();
                        break;
                }
            }
        }

        private int KillPoints
        {
            get => _killPoints;
            set 
            { 
                _eventSystem.Get<KillsChange>()?.Invoke(_killPoints, value);
                _killPoints = value;
            }
        }

        private int CurrentBullets
        {
            get => _currentBullets;
            set
            {
                if (value < _currentBullets && config.InfinitAmmo)
                    return;

                int newValue = Mathf.Clamp(value, 0, config.MaxBullets);

                _eventSystem.Get<AmmoChange>()?.Invoke(_currentBullets, newValue, config.MaxBullets);
                _currentBullets = newValue;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            _eventSystem = ServiceProvider.GetService<CentralizeEventSystem>();
            
            _spin = GetComponentInChildren<ICharacterSpin>();

            CurrentLives = config.MaxLives;
            CurrentBullets = config.MaxBullets;

            SetUpEvents();
        }

        private IEnumerator Start()
        {
            ServiceProvider.GetService<IFollowTarget>().SetTarget(transform);

            _particlePool = ServiceProvider.GetService<IObjectPool<ParticleController>>();
            
            while (!ServiceProvider.TryGetService(out _mouseTracker))
                yield return null;
            
            _eventSystem.Get<AmmoChange>()?.Invoke(CurrentBullets, CurrentBullets, config.MaxBullets);
            
            _eventSystem.Get<LivesChange>()?.Invoke(CurrentLives, CurrentLives, config.MaxLives);
        }

        private void SetUpEvents()
        {
            _eventSystem.AddListener<AttackInput>(OnAttack);
            _eventSystem.AddListener<AttackInput>(CancelReloadOverTime);
            _eventSystem.AddListener<ReloadInput>(AddBulletsOverTime);
            _eventSystem.AddListener<Reload360>(AddBullet);
        }

        private void Update()
        {
            _distancePoints = (int)Mathf.Abs(Vector3.Distance(new Vector3(0, _initialPos.y, 0),
                new Vector3(0, transform.position.y, 0)));

            _eventSystem.Get<DistanceChange>()?.Invoke(0, _distancePoints);
        }

        private void OnDisable() => UnRegisterEvents();

        private void UnRegisterEvents()
        {
            _eventSystem.RemoveListener<AttackInput>(OnAttack);
            _eventSystem.RemoveListener<AttackInput>(CancelReloadOverTime);
            _eventSystem.RemoveListener<ReloadInput>(AddBulletsOverTime);
            _eventSystem.RemoveListener<Reload360>(AddBullet);
        }

        private async void OnAttack()
        {
            AkUnitySoundEngine.SetRTPCValue("BulletCount", CurrentBullets);
            AkUnitySoundEngine.PostEvent("sfx_Gunshot", gameObject);

            if (CurrentBullets == 0)
                return;

            Vector3 dir = _mouseTracker.GetMouseDir(transform);
            Vector3 mousePos = _mouseTracker.GetMouseWorldPos();

            _spin.SetSpin(dir);

            if (dir.sqrMagnitude < Mathf.Epsilon * Mathf.Epsilon)
                return;

            ApplyKnockBack(dir, mousePos);

            --CurrentBullets;

            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, Mathf.Infinity, Physics.AllLayers,
                    QueryTriggerInteraction.Ignore))
            {
                PoolData<ParticleController> particle = await _particlePool.Get(bulletParticlePrefab);

                particle.Obj.transform.position = hit.point;
                particle.Obj.transform.rotation = Quaternion.LookRotation(transform.position - hit.point);
                particle.Component.SetUp(() => _particlePool.Return(particle));

                if (hit.transform.gameObject.TryGetComponent(out IEnemyHealthSystem healthSystem))
                {
                    healthSystem.ReceiveDamage(OnConfirmKill);
                }
            }
        }

        private void OnConfirmKill()
        {
            AddBullet();
            AddKillPoints();
        }

        private void AddKillPoints() => KillPoints += config.PointsPerKill;

        private void ApplyKnockBack(Vector3 dir, Vector3 mousePos)
        {
            Rb.linearVelocity = Vector3.zero;

            float distance = (mousePos - transform.position).magnitude;

            distance = Mathf.Clamp01(distance / config.AreaOfSight);

            distance = Mathf.Pow(distance, 0.5f);

            Rb.AddForce(dir * (-config.KnockBack * distance), ForceMode.Impulse);

            if (dir.y < -0.5f)
                AkUnitySoundEngine.PostEvent("sfx_Jump", gameObject);
        }

        public void ReceiveDamage(Action action = null)
        {
            if (Invincible)
                return;

            --CurrentLives;

            StartCoroutine(InvincibilityFramesCoroutine());
        }

        private IEnumerator InvincibilityFramesCoroutine()
        {
            Invincible = true;

            yield return new WaitForSeconds(config.InvincibleTime);

            Invincible = false;
            _eventSystem.Get<InvincibilityOff>()?.Invoke();
        }

        private void OnDead()
        {
            _eventSystem.Get<PlayerDied>()?.Invoke(_killPoints, _distancePoints);
            _isDead = true;
        }

        public void InstantDead() => CurrentLives = 0;

        private void AddBullet() => ++CurrentBullets;

        private void AddBulletsOverTime() => _reloadingCoroutine ??= StartCoroutine(ReloadingOverTime());

        private IEnumerator ReloadingOverTime()
        {
            while (_currentBullets < config.MaxBullets)
            {
                yield return new WaitForSeconds(1);

                ++CurrentBullets;
            }

            _reloadingCoroutine = null;
        }

        private void CancelReloadOverTime()
        {
            if (_reloadingCoroutine == null)
                return;

            StopCoroutine(_reloadingCoroutine);
            _reloadingCoroutine = null;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, config.AreaOfSight);
        }
#endif
    }
}