using InputSystem;
using Systems.CentralizeEventSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Systems
{
    public delegate void AttackInput();
    public delegate void ReloadInput();
    public delegate void InstantReloadInput();
    public delegate void PausedInput();

    public class InputReader : MonoBehaviour, IInputReader
    {
        private CentralizeEventSystem.CentralizeEventSystem _eventSystem;

        private CustomInputSytem _inputSystem;

        private void Awake()
        {
            _inputSystem = new CustomInputSytem();

            ServiceProvider.SetService<IInputReader>(this);
        }

        private void Start()
        {
            _inputSystem.UI.Enable();
            _eventSystem = ServiceProvider.GetService<CentralizeEventSystem.CentralizeEventSystem>();
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

        private void HandlePause(InputAction.CallbackContext _) => _eventSystem.Get<PausedInput>()?.Invoke();

        #region PlayerActionsEvents

        private void HandleAttack(InputAction.CallbackContext _) => _eventSystem.Get<AttackInput>()?.Invoke();

        private void HandleReload(InputAction.CallbackContext _) => _eventSystem.Get<ReloadInput>()?.Invoke();

        private void HandleInstantReload(InputAction.CallbackContext _) =>
            _eventSystem.Get<InstantReloadInput>()?.Invoke();

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