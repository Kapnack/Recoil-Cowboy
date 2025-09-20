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

        private readonly SimpleEvent _attackEvent = new();

        private IEnumerator Start()
        {
            while (!ServiceProvider.TryGetService(out _eventSystem))
            {
                yield return null;
            }

            _eventSystem.Register(PlayerEventKeys.Attack, _attackEvent);
        }

        private void OnEnable()
        {
            attack.action.started += HandleAttack;
        }

        private void OnDisable()
        {
            attack.action.started -= HandleAttack;
        }

        private void HandleAttack(InputAction.CallbackContext _)
        {
            _attackEvent?.Invoke();
        }
    }
}