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
        [SerializeField] private int _currentHealth;

        private int _currentBullets;

        private ICentralizeEventSystem _eventSystem;
        private IMousePositionTracker _mouseTracker;

        private readonly ComplexGameEvent<int, int, int> _livesChangeEvent = new();
        private readonly ComplexGameEvent<int, int, int> _bulletsChangeEvent = new();
        private readonly SimpleEvent _dies = new();

        public bool Invincible { get; private set; }

        private int CurrentHealth
        {
            get => _currentHealth;

            set
            {
                var newValue = Mathf.Clamp(value, 0, _config.MaxHealth);

                _livesChangeEvent?.Invoke(_currentHealth, newValue, _config.MaxHealth);
                _currentHealth = newValue;
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

            CurrentHealth = _config.MaxHealth;
            CurrentBullets = _config.MaxBullets;
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

                cam.transform.localPosition = new Vector3(0.0f, 5.0f, -10.0f);
                cam.transform.LookAt(transform);
            }

            while (!ServiceProvider.TryGetService(out _mouseTracker))
                yield return null;

            while (!ServiceProvider.TryGetService(out _eventSystem))
                yield return null;

            RegisterEvents();
            
            SimpleEvent attack;
            while (!_eventSystem.TryGet(PlayerEventKeys.Attack, out attack))
                yield return null;
    
            attack.AddListener(OnAttack);
        }

        private void OnEnable()
        {
            RegisterEvents();
        }

        private void OnDisable()
        {
            UnRegisterEvents();
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

            --CurrentHealth;

            StartCoroutine(InvincibilityFramesCoroutine());

            if (_currentBullets <= 0)
                _dies?.Invoke();
        }

        private IEnumerator InvincibilityFramesCoroutine()
        {
            Invincible = true;

            yield return new WaitForSeconds(_config.InvincibleTime);

            Invincible = false;
        }

        public void InstantDead()
        {
            CurrentHealth = 0;
            _dies?.Invoke();
        }

        private void RegisterEvents()
        {
            _eventSystem?.Register(PlayerEventKeys.LivesChange, _livesChangeEvent);
            _eventSystem?.Register(PlayerEventKeys.BulletsChange, _bulletsChangeEvent);
        }

        private void UnRegisterEvents()
        {
            _eventSystem?.Unregister(PlayerEventKeys.LivesChange);
            _eventSystem?.Unregister(PlayerEventKeys.BulletsChange);
        }
    }
}