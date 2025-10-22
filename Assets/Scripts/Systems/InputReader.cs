using System.Collections;
using Systems.CentralizeEventSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Systems
{
    public class InputReader : MonoBehaviour, IInputReader
    {
        private ICentralizeEventSystem _eventSystem;

        [SerializeField] private InputActionAsset gameplayActionMap;

        private InputActionMap _gameplayActionMap;

        private InputAction _attack;
        private InputAction _reload;
        private InputAction _activeInstantReload;
        [SerializeField] private InputActionReference paused;

        private readonly SimpleEvent _attackEvent = new();
        private readonly SimpleEvent _reloadEvent = new();
        private readonly SimpleEvent _instantReloadEvent = new();
        private readonly SimpleEvent _paused = new();

        private void Awake()
        {
            ServiceProvider.SetService<IInputReader>(this);
            
            _gameplayActionMap = gameplayActionMap.FindActionMap("Player");
            _attack = _gameplayActionMap.FindAction("Attack");
            _reload = _gameplayActionMap.FindAction("Reload");
            _activeInstantReload = _gameplayActionMap.FindAction("ChangeInstantReload");
        }

        private void Start()
        {
            _eventSystem = ServiceProvider.GetService<ICentralizeEventSystem>();

            _eventSystem.Register(PlayerEventKeys.Attack, _attackEvent);
            _eventSystem.Register(PlayerEventKeys.ReloadOvertime, _reloadEvent);
            _eventSystem.Register(PlayerEventKeys.Paused, _paused);
        }

        private void OnEnable()
        {
            _attack.started += HandleAttack;
            _reload.started += HandleReload;
            _activeInstantReload.started += HandleInstantReload;
            paused.action.started += HandlePause;
        }


        private void OnDisable()
        {
            _attack.started -= HandleAttack;
            _reload.started -= HandleReload;
            _activeInstantReload.started -= HandleInstantReload;
            paused.action.started -= HandlePause;
        }

        private void HandlePause(InputAction.CallbackContext _) => _paused?.Invoke();

        #region PlayerActionsEvents

        private void HandleAttack(InputAction.CallbackContext _) => _attackEvent?.Invoke();

        private void HandleReload(InputAction.CallbackContext _) => _reloadEvent?.Invoke();

        private void HandleInstantReload(InputAction.CallbackContext _) => _instantReloadEvent?.Invoke();

        #endregion

        public void ActivePlayerMap() => _gameplayActionMap.Enable();
        public void DeactivatePlayerMap() => _gameplayActionMap.Disable();

        public void SwitchPlayerMapState()
        {
            if (_gameplayActionMap.enabled)
                _gameplayActionMap.Disable();
            else
                _gameplayActionMap.Enable();
        }
    }
}