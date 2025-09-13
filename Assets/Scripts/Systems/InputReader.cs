using Systems.CentralizeEventSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Systems
{
    public class InputReader : MonoBehaviour
    {
        private ICentralizeEventSystem _eventSystem;

        [SerializeField] private InputActionReference _attack;
        
        private readonly SimpleEvent _attackEvent = new();
        
        private void Awake()
        {
            if (!ServiceProvider.TryGetService(out _eventSystem))
            {
                enabled = false;
            }

            _eventSystem.Register(PlayerEventKeys.Attack, _attackEvent);
        }

        private void OnEnable()
        {
            _attack.action.started += HandleAttack;
        }

        private void OnDisable()
        {
            _attack.action.started -= HandleAttack;
        }
        
        private void HandleAttack(InputAction.CallbackContext _)
        {
            _attackEvent?.Invoke();
        }
    }
}