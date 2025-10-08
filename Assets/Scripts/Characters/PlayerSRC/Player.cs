using System.Collections;
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

        private bool _isDead;

        private int _currentBullets;

        private bool _instantReload;

        private Coroutine _reloadingCoroutine;

        private IMousePositionTracker _mouseTracker;

        private readonly ComplexGameEvent<int, int, int> _livesChangeEvent = new();
        private readonly ComplexGameEvent<int, int, int> _bulletsChangeEvent = new();
        private readonly SimpleEvent _dies = new();

        public bool Invincible { get; private set; }

        private int CurrentLives
        {
            get => _currentLives;

            set
            {
                var newValue = Mathf.Clamp(value, 0, _config.MaxLives);

                _livesChangeEvent?.Invoke(_currentLives, newValue, _config.MaxLives);
                _currentLives = newValue;

                if (newValue == 0 && !_isDead)
                {
                    _dies?.Invoke();
                    _isDead = true;
                }
            }
        }

        private int CurrentBullets
        {
            get => _currentBullets;
            set
            {
                var newValue = Mathf.Clamp(value, 0, _config.MaxBullets);

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
            var cam = Camera.main;

            while (cam == null)
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
            eventSystem.Register(PlayerEventKeys.BulletsChange, _bulletsChangeEvent);
            eventSystem.Register(PlayerEventKeys.Dies, _dies);

            eventSystem.TryGet(PlayerEventKeys.Attack, out var simpleEvent);

            simpleEvent.AddListener(OnAttack);
            simpleEvent.AddListener(CancelReloadOverTime);

            eventSystem.TryGet(PlayerEventKeys.ReloadOvertime, out simpleEvent);

            simpleEvent.AddListener(AddBulletsOverTime);

            eventSystem.TryGet(PlayerEventKeys.InstantReload, out simpleEvent);

            simpleEvent.AddListener(ChangeInstantReloadMode);

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
            var eventSystem = ServiceProvider.GetService<ICentralizeEventSystem>();

            eventSystem.Unregister(PlayerEventKeys.LivesChange);
            eventSystem.Unregister(PlayerEventKeys.BulletsChange);
            eventSystem.Unregister(PlayerEventKeys.Dies);

            eventSystem.Get(PlayerEventKeys.Attack).RemoveListener(OnAttack);

            eventSystem.Get(PlayerEventKeys.ReloadOvertime).RemoveListener(AddBulletsOverTime);
            eventSystem.Get(PlayerEventKeys.InstantReload).RemoveListener(ChangeInstantReloadMode);
        }

        private void OnAttack()
        {
            if (CurrentBullets == 0)
                return;

            var dir = _mouseTracker.GetMouseDir(transform);
            var mousePos = _mouseTracker.GetMouseWorldPos();

            if (dir.sqrMagnitude < Mathf.Epsilon * Mathf.Epsilon)
                return;

            ApplyKnockBack(dir, mousePos);

            --CurrentBullets;

            if (Physics.Raycast(transform.position, dir, out var hit))
            {
                if (hit.transform.gameObject.TryGetComponent<IHealthSystem>(out var healthSystem))
                    healthSystem.ReceiveDamage();
            }
        }

        private void ApplyKnockBack(Vector3 dir, Vector3 mousePos)
        {
            _rb.linearVelocity = Vector3.zero;

            var distance = (mousePos - transform.position).magnitude;

            distance = Mathf.Clamp01(distance / _config.MaxDistance);

            distance = Mathf.Pow(distance, 0.5f);
            
            _rb.AddForce(dir * (-_config.KnockBack * distance), ForceMode.Impulse);
        }

        public void ReceiveDamage()
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

        private void AddBullet()
        {
            if (_instantReload)
                CurrentBullets = _config.MaxBullets;
            else
                ++CurrentBullets;
        }


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

        private void ChangeInstantReloadMode() => _instantReload = !_instantReload;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _config.MaxDistance);
        }
    }
}