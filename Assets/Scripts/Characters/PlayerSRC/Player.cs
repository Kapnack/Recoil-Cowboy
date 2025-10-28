using System;
using System.Collections;
using Characters.EnemySRC;
using MouseTracker;
using ScriptableObjects;
using Systems;
using Systems.CentralizeEventSystem;
using UnityEngine;

namespace Characters.PlayerSRC
{
    [RequireComponent(typeof(Rigidbody))]
    public class Player : Character, IPlayerHealthSystem
    {
        [SerializeField] private PlayerConfig _config;

        [ContextMenuItem("Instant Kill", nameof(InstantDead))]
        private int _currentLives;

        private int _points;
        
        private bool _isDead;

        private int _currentBullets;

        private Coroutine _reloadingCoroutine;

        private IMousePositionTracker _mouseTracker;

        private readonly ComplexGameEvent<int, int, int> _livesChangeEvent = new();
        private readonly DoubleParamEvent<int, int> _pointsChangeEvent = new();
        private readonly ComplexGameEvent<int, int, int> _bulletsChangeEvent = new();
        private readonly SimpleEvent _oneLiveRemains = new();
        private readonly SimpleEvent _dies = new();

        public bool Invincible { get; private set; }

        private int CurrentLives
        {
            get => _currentLives;

            set
            {
                int newValue = Mathf.Clamp(value, 0, _config.MaxLives);

                _livesChangeEvent?.Invoke(_currentLives, newValue, _config.MaxLives);
                _currentLives = newValue;

                switch (newValue)
                {
                    case 1:
                        _oneLiveRemains?.Invoke();
                        break;

                    case 0 when !_isDead:
                        _dies?.Invoke();
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
                int newValue = Mathf.Clamp(value, 0, _config.MaxBullets);

                _bulletsChangeEvent?.Invoke(_currentBullets, newValue, _config.MaxBullets);
                _currentBullets = newValue;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            CurrentLives = _config.MaxLives;
            CurrentBullets = _config.MaxBullets;

            StartCoroutine(SetUpEvents());
        }

        private IEnumerator Start()
        {
            Camera cam = Camera.main;

            while (!cam)
            {
                cam = Camera.main;
                yield return null;
            }

            if (cam)
            {
                cam.transform.SetParent(transform);

                cam.transform.localPosition = new Vector3(0.0f, 5.0f, -20.0f);
                cam.transform.LookAt(transform);
            }

            while (!ServiceProvider.TryGetService(out _mouseTracker))
                yield return null;

            while (!_bulletsChangeEvent.HasInvocations())
                yield return null;
            _bulletsChangeEvent.Invoke(CurrentBullets, CurrentBullets, _config.MaxBullets);

            while (!_livesChangeEvent.HasInvocations())
                yield return null;
            _livesChangeEvent.Invoke(CurrentLives, CurrentLives, _config.MaxLives);
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

        private void AddPoints() => Points += _config.PointsPerKill;
        
        private void ApplyKnockBack(Vector3 dir, Vector3 mousePos)
        {
            Rb.linearVelocity = Vector3.zero;

            float distance = (mousePos - transform.position).magnitude;

            distance = Mathf.Clamp01(distance / _config.AreaOfSight);

            distance = Mathf.Pow(distance, 0.5f);

            Rb.AddForce(dir * (-_config.KnockBack * distance), ForceMode.Impulse);

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

            yield return new WaitForSeconds(_config.InvincibleTime);

            Invincible = false;
        }

        public void InstantDead() => CurrentLives = 0;

        private void AddBullet() => ++CurrentBullets;

        private void AddBulletsOverTime() => _reloadingCoroutine ??= StartCoroutine(ReloadingOverTime());

        private IEnumerator ReloadingOverTime()
        {
            while (_currentBullets < _config.MaxBullets)
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
            Gizmos.DrawWireSphere(transform.position, _config.AreaOfSight);
        }
    }
}