using System;
using System.Collections;
using Characters.EnemySRC;
using MouseTracker;
using ScriptableObjects;
using Systems;
using Systems.CentralizeEventSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace Characters.PlayerSRC
{
    [RequireComponent(typeof(Rigidbody))]
    public class Player : Character, IPlayerHealthSystem
    {
        [FormerlySerializedAs("_config")] [SerializeField]
        private PlayerConfig config;

        [ContextMenuItem("Instant Kill", nameof(InstantDead))]
        private int _currentLives;

        private Vector3 _initialPos;

        private int _points;

        private bool _isDead;
        private bool _canDied;

        private int _currentBullets;

        private Camera _camera;

        private Coroutine _reloadingCoroutine;

        private IMousePositionTracker _mouseTracker;

        private readonly ComplexGameEvent<int, int, int> _livesChangeEvent = new();
        private readonly DoubleParamEvent<int, int> _pointsChangeEvent = new();
        private readonly ComplexGameEvent<int, int, int> _bulletsChangeEvent = new();
        private readonly SimpleEvent _oneLiveRemains = new();
        private readonly SingleParamEvent<int> _dies = new();

        public bool Invincible { get; private set; }

        private int CurrentLives
        {
            get => _currentLives;

            set
            {
                int newValue = Mathf.Clamp(value, 0, config.MaxLives);

                _livesChangeEvent?.Invoke(_currentLives, newValue, config.MaxLives);
                _currentLives = newValue;

                switch (newValue)
                {
                    case 1:
                        _oneLiveRemains?.Invoke();
                        break;

                    case 0 when !_isDead:
                        _dies?.Invoke(Points);
                        _isDead = true;
                        break;
                }
            }
        }

        private int Points
        {
            get => _points;
            set
            {
                _pointsChangeEvent?.Invoke(_points, value);
                _points = value;
            }
        }

        private int CurrentBullets
        {
            get => _currentBullets;
            set
            {
                int newValue = Mathf.Clamp(value, 0, config.MaxBullets);

                _bulletsChangeEvent?.Invoke(_currentBullets, newValue, config.MaxBullets);
                _currentBullets = newValue;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            _camera = Camera.main;

            CurrentLives = config.MaxLives;
            CurrentBullets = config.MaxBullets;

            StartCoroutine(SetUpEvents());
            StartCoroutine(SetUpCanDied());
        }


        private IEnumerator SetUpCanDied()
        {
            yield return new WaitForSeconds(0.5f);
            _canDied = true;
        }

        private IEnumerator Start()
        {
            _initialPos = transform.position;

            ServiceProvider.GetService<IFollowTarget>().SetTarget(transform);

            while (!ServiceProvider.TryGetService(out _mouseTracker))
                yield return null;

            while (!_bulletsChangeEvent.HasInvocations())
                yield return null;
            _bulletsChangeEvent.Invoke(CurrentBullets, CurrentBullets, config.MaxBullets);

            while (!_livesChangeEvent.HasInvocations())
                yield return null;
            _livesChangeEvent.Invoke(CurrentLives, CurrentLives, config.MaxLives);
        }

        private IEnumerator SetUpEvents()
        {
            ServiceProvider.TryGetService(out ICentralizeEventSystem eventSystem);

            eventSystem.Register(PlayerEventKeys.LivesChange, _livesChangeEvent);
            eventSystem.Register(PlayerEventKeys.PointsChange, _pointsChangeEvent);
            eventSystem.Register(PlayerEventKeys.BulletsChange, _bulletsChangeEvent);
            eventSystem.Register(PlayerEventKeys.OnOneLive, _oneLiveRemains);
            eventSystem.Register(PlayerEventKeys.Dies, _dies);

            eventSystem.TryGet(PlayerEventKeys.Attack, out SimpleEvent simpleEvent);

            simpleEvent.AddListener(OnAttack);
            simpleEvent.AddListener(CancelReloadOverTime);

            eventSystem.TryGet(PlayerEventKeys.ReloadOvertime, out simpleEvent);

            simpleEvent.AddListener(AddBulletsOverTime);

            while (!eventSystem.TryGet(PlayerEventKeys.Reload, out simpleEvent))
                yield return null;

            simpleEvent.AddListener(AddBullet);
        }

        private void Update()
        {
            if (_isDead || !_canDied)
                return;

            Vector3 viewportPos = _camera.WorldToViewportPoint(transform.position);

            bool isVisible =
                viewportPos.x is >= 0f and <= 1f &&
                viewportPos.y is >= 0f and <= 1f &&
                viewportPos.z > 0f;

            if (isVisible)
                return;

            _dies.Invoke(Points);
            _isDead = true;
        }

        private void OnDisable()
        {
            UnRegisterEvents();
        }

        private void UnRegisterEvents()
        {
            ICentralizeEventSystem eventSystem = ServiceProvider.GetService<ICentralizeEventSystem>();

            eventSystem.Unregister(PlayerEventKeys.LivesChange);
            eventSystem.Unregister(PlayerEventKeys.PointsChange);
            eventSystem.Unregister(PlayerEventKeys.BulletsChange);
            eventSystem.Unregister(PlayerEventKeys.OnOneLive);
            eventSystem.Unregister(PlayerEventKeys.Dies);

            eventSystem.Get(PlayerEventKeys.Attack).RemoveListener(OnAttack);

            eventSystem.Get(PlayerEventKeys.ReloadOvertime).RemoveListener(AddBulletsOverTime);
        }

        private void OnAttack()
        {
            AkUnitySoundEngine.SetRTPCValue("BulletCount", CurrentBullets);
            AkUnitySoundEngine.PostEvent("sfx_Gunshot", gameObject);

            if (CurrentBullets == 0)
                return;

            Vector3 dir = _mouseTracker.GetMouseDir(transform);
            Vector3 mousePos = _mouseTracker.GetMouseWorldPos();

            if (dir.sqrMagnitude < Mathf.Epsilon * Mathf.Epsilon)
                return;

            ApplyKnockBack(dir, mousePos);

            --CurrentBullets;

            if (Physics.Raycast(transform.position, dir, out RaycastHit hit))
            {
                if (hit.transform.gameObject.TryGetComponent(out IEnemyHealthSystem healthSystem))
                {
                    healthSystem.ReceiveDamage(OnConfirmKill);
                }
            }
        }

        private void OnConfirmKill()
        {
            AddBullet();
            AddPoints();
        }

        private void AddPoints() => Points += config.PointsPerKill;

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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, config.AreaOfSight);
        }
    }
}