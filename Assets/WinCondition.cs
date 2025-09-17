using Systems;
using Systems.CentralizeEventSystem;
using Systems.TagClassGenerator;
using UnityEngine;
using UnityEngine.Serialization;

public class WinCondition : MonoBehaviour
{
    [SerializeField] private float timeToWait = 2.0f;
    private float _timeUntilWin;
    private bool _isPlayerInside;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.CompareTag(Tags.Player))
            return;

        _isPlayerInside = true;
        _timeUntilWin = Time.time + timeToWait;
    }

    private void OnCollisionStay()
    {
        if (!_isPlayerInside)
            return;

        if (Time.time < _timeUntilWin)
            return;

        if (ServiceProvider.TryGetService(out ICentralizeEventSystem eventSystem))
        {
            if (eventSystem.TryGet(PlayerEventKeys.LevelComplete, out var simpleEvent))
            {
                simpleEvent.Invoke();
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.CompareTag(Tags.Player))
            _isPlayerInside = false;
    }
}