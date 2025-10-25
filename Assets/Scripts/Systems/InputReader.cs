using InputSystem;
using Systems.CentralizeEventSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Systems
{
    public class InputReader : MonoBehaviour, IInputReader
    {
        private ICentralizeEventSystem _eventSystem;

        private CustomInputSytem _inputSystem;

        private readonly SimpleEvent _attackEvent = new();
        private readonly SimpleEvent _reloadEvent = new();
        private readonly SimpleEvent _instantReloadEvent = new();
        private readonly SimpleEvent _paused = new();

        private void Awake()
        {
            _inputSystem = new CustomInputSytem();
            
            ServiceProvider.SetService<IInputReader>(this);
        }

        private void Start()
        {
            _inputSystem.UI.Enable();
            
            _eventSystem = ServiceProvider.GetService<ICentralizeEventSystem>();

            _eventSystem.Register(PlayerEventKeys.Attack, _attackEvent);
            _eventSystem.Register(PlayerEventKeys.ReloadOvertime, _reloadEvent);
            _eventSystem.Register(PlayerEventKeys.Paused, _paused);
        }

        private void OnEnable()
        {
            _inputSystem.Player.Attack.started += HandleAttack;
            _inputSystem.Player.Reload.started += HandleReload;
            _inputSystem.Player.ChangeInstantReload.started += HandleInstantReload;
            _inputSystem.UI.Pause.started += HandlePause;
        }


        private void OnDisable()
        {
            _inputSystem.Player.Attack.started -= HandleAttack;
            _inputSystem.Player.Reload.started -= HandleReload;
            _inputSystem.Player.ChangeInstantReload.started -= HandleInstantReload;
            _inputSystem.UI.Pause.started -= HandlePause;
        }

        private void HandlePause(InputAction.CallbackContext _) => _paused?.Invoke();

        #region PlayerActionsEvents

        private void HandleAttack(InputAction.CallbackContext _) => _attackEvent?.Invoke();

        private void HandleReload(InputAction.CallbackContext _) => _reloadEvent?.Invoke();

        private void HandleInstantReload(InputAction.CallbackContext _) => _instantReloadEvent?.Invoke();

        #endregion

        public void ActivePlayerMap() => _inputSystem.Player.Enable();
        public void DeactivatePlayerMap() => _inputSystem.Player.Disable();

        public void SwitchPlayerMapState()
        {
            if (_inputSystem.Player.enabled)
                _inputSystem.Player.Disable();
            else
                _inputSystem.Player.Enable();
        }
    }
}