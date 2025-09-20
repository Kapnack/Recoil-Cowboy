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

            while (!eventSystem.TryGet(PlayerEventKeys.Reload, out simpleEvent))
                yield return null;

            simpleEvent.AddListener(AddAmmo);
        }

        private void OnDisable()
        {
            UnRegisterEvents();
        }

        private void UnRegisterEvents()
        {
            if (!ServiceProvider.TryGetService<ICentralizeEventSystem>(out var eventSystem))
                return;

            eventSystem?.Unregister(PlayerEventKeys.LivesChange);
            eventSystem?.Unregister(PlayerEventKeys.BulletsChange);
            eventSystem?.Unregister(PlayerEventKeys.Dies);

            eventSystem?.Get(PlayerEventKeys.Attack).RemoveListener(OnAttack);
        }
        
        private void OnAttack()
        {
            if (CurrentBullets == 0)
                return;

            var dir = _mouseTracker.GetMouseDir(transform);

            if (dir.sqrMagnitude < Mathf.Epsilon * Mathf.Epsilon)
                return;

            ApplyKnockBack(dir);

            --CurrentBullets;

            if (Physics.Raycast(transform.position, dir, out var hit))
            {
                if (hit.transform.gameObject.TryGetComponent<IHealthSystem>(out var healthSystem))
                    healthSystem.ReceiveDamage();
            }
        }

        private void ApplyKnockBack(Vector3 dir)
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.AddForce(dir * -_config.KnockBack, ForceMode.Impulse);
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

        public void AddAmmo() => CurrentBullets++;

        public void AddAmmo() => CurrentBullets++;
    }
}