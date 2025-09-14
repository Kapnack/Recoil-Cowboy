using System.Collections;
using MouseTracker;
using ScriptableObjects;
using Systems;
using Systems.CentralizeEventSystem;
using Unity.Collections;
using UnityEngine;

namespace Characters.PlayerSRC
{
    [RequireComponent(typeof(Rigidbody))]
    public class Player : Character, IPlayerHealthSystem
    {
        [SerializeField] private PlayerConfig _config;
        [SerializeField] private int _currentHealth;

        public bool Invincible
        {
            get;
            private set;
        }

        private int _currentBullets;
        
        private ICentralizeEventSystem _eventSystem;
        private IMousePositionTracker _mouseTracker;

        private readonly ComplexGameEvent<int, int, int> _livesChangeEvent = new();
        private readonly ComplexGameEvent<int, int, int> _bulletsChangeEvent = new();
        private readonly SimpleEvent _dies = new();

        protected override void Awake()
        {
            base.Awake();
            
            _currentHealth = _config.MaxHealth;
            _currentBullets = _config.MaxBullets;

            ServiceProvider.TryGetService(out _eventSystem);
            ServiceProvider.TryGetService(out _mouseTracker);
        }

        private void OnEnable()
        {
            _eventSystem?.Get(PlayerEventKeys.Attack).AddListener(OnAttack);
            
            _eventSystem?.Register(PlayerEventKeys.LivesChange, _livesChangeEvent);
            _eventSystem?.Register(PlayerEventKeys.BulletsChange, _bulletsChangeEvent);
        }

        private void OnDisable()
        {
            _eventSystem?.Unregister(PlayerEventKeys.LivesChange);
            _eventSystem?.Unregister(PlayerEventKeys.BulletsChange);
        }
        
        private void OnAttack()
        {
            if (_currentBullets <= 0)
                return;

            var dir = _mouseTracker.GetMouseDir();

            if (dir.sqrMagnitude < Mathf.Epsilon * Mathf.Epsilon)
                return;

            _rb.linearVelocity = Vector3.zero;
            _rb.AddForce(-dir * _config.KnockBack, ForceMode.Impulse);

            _bulletsChangeEvent?.Invoke(_currentBullets, _currentBullets - 1, _config.MaxBullets);
            --_currentBullets;
            
            if (Physics.Raycast(transform.position, dir, out var hit))
            {
                if (hit.transform.gameObject.TryGetComponent<IHealthSystem>(out var healthSystem))
                    healthSystem.ReceiveDamage();
            }
        }

        public void ReceiveDamage()
        {
            if (Invincible)
                return;

            _livesChangeEvent?.Invoke(_currentHealth,  _currentBullets - 1, _config.MaxHealth);
            --_currentHealth;

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
            _currentHealth = 0;
            _dies?.Invoke();
        }
    }
}