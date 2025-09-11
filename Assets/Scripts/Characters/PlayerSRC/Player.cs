using System;
using System.Collections;
using MouseTracker;
using ScriptableObjects;
using Systems;
using Systems.CentralizeEventSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace Characters.PlayerSRC
{
    [RequireComponent(typeof(Rigidbody))]
    public class Player : MonoBehaviour, IPlayerHealthSystem
    {
        [SerializeField] private PlayerConfig _config;
        [SerializeField] private int _currentHealth;
        private bool _invincible;

        private int _currentBullets;

        private ICentralizeEventSystem _serviceProvider;
        private ICentralizeEventSystem _eventSystem;
        private IMousePositionTracker _mouseTracker;
        
        private Rigidbody _rb;

        private readonly ComplexGameEvent<int, int, int> _livesChangeEvent = new();
        private readonly ComplexGameEvent<int, int, int> _bulletsChangeEvent = new();
        private readonly SimpleEvent _dies = new();
        
        private void Awake()
        {
            _currentHealth = _config.MaxHealth;
            _currentBullets = _config.MaxBullets;

            _rb = GetComponent<Rigidbody>();

            ServiceProvider.TryGetService(out _eventSystem);
            ServiceProvider.TryGetService(out _mouseTracker);
            ServiceProvider.TryGetService(out _serviceProvider);

            _serviceProvider?.Register(PlayerEventKeys.LivesChange, _livesChangeEvent);
        }

        private void OnEnable()
        {
            _eventSystem?.Get(PlayerEventKeys.Attack).AddListener(OnAttack);
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
            if (_invincible)
                return;

            _livesChangeEvent?.Invoke(_currentHealth,  _currentBullets - 1, _config.MaxHealth);
            --_currentHealth;

            StartCoroutine(InvincibilityFramesCoroutine());
            
            if (_currentBullets <= 0)
                _dies?.Invoke();
        }

        private IEnumerator InvincibilityFramesCoroutine()
        {
            _invincible = true;

            yield return new WaitForSeconds(_config.InvincibleTime);

            _invincible = false;
        }

        public void InstantDead()
        {
            _currentHealth = 0;
            _dies?.Invoke();
        }
    }
}