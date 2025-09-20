using Characters.PlayerSRC;
using Systems;
using Systems.CentralizeEventSystem;
using Systems.TagClassGenerator;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class WinCondition : MonoBehaviour
{
    [SerializeField] private float timeToWait = 2.0f;
    private float _timeUntilWin;
    private bool _isPlayerInside;
    private bool _eventCalled;

    private readonly SimpleEvent _event = new();

    private void Awake()
    {
        var boxCollider = GetComponent<BoxCollider>();

        boxCollider.isTrigger = true;

        if (ServiceProvider.TryGetService(out ICentralizeEventSystem eventSystem))
            eventSystem.Register(PlayerEventKeys.Wins, _event);
    }

    private void OnDestroy()
    {
        if (ServiceProvider.TryGetService(out ICentralizeEventSystem eventSystem))
            eventSystem.Unregister(PlayerEventKeys.Wins);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.CompareTag(Tags.Player))
            return;

        _isPlayerInside = true;
        _timeUntilWin = Time.time + timeToWait;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!_isPlayerInside)
            return;

        if (Time.time < _timeUntilWin)
            return;

        if (!_eventCalled)
        {
            _event?.Invoke();
            _eventCalled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag(Tags.Player))
            _isPlayerInside = false;
    }
}