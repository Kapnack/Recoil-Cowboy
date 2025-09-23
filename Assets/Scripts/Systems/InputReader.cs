using System.Collections;
using Systems.CentralizeEventSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Systems
{
    public class InputReader : MonoBehaviour
    {
        private ICentralizeEventSystem _eventSystem;

        [SerializeField] private InputActionReference attack;
        [SerializeField] private InputActionReference reload;
        [SerializeField] private InputActionReference activeInstantReload;

        private readonly SimpleEvent _attackEvent = new();
        private readonly SimpleEvent _reloadEvent = new();
        private readonly SimpleEvent _instantReloadEvent = new();

        private IEnumerator Start()
        {
            while (!ServiceProvider.TryGetService(out _eventSystem))
            {
                yield return null;
            }

            _eventSystem.Register(PlayerEventKeys.Attack, _attackEvent);
            _eventSystem.Register(PlayerEventKeys.ReloadOvertime, _reloadEvent);
            _eventSystem.Register(PlayerEventKeys.InstantReload, _instantReloadEvent);
        }

        private void OnEnable()
        {
            attack.action.started += HandleAttack;
            reload.action.started += HandleReload;
            activeInstantReload.action.started += HandleInstantReload;
        }

        private void OnDisable()
        {
            attack.action.started -= HandleAttack;
            reload.action.started -= HandleReload;
            activeInstantReload.action.started -= HandleInstantReload;
        }

        #region PlayerActionsEvents
        
        private void HandleAttack(InputAction.CallbackContext _) => _attackEvent?.Invoke();
        
        private void HandleReload(InputAction.CallbackContext _) => _reloadEvent?.Invoke();
        
        private void  HandleInstantReload(InputAction.CallbackContext _) =>  _instantReloadEvent?.Invoke();
        
        #endregion
    }
}