using System.Collections;
using Systems;
using Systems.CentralizeEventSystem;
using Systems.TagClassGenerator;
using UnityEngine;

public class WinCondition : MonoBehaviour
{
    [SerializeField] private float timeToWait = 2.0f;
    private float _timeUntilWin;
    private bool _isPlayerInside;

    private readonly SimpleEvent _event = new();

    private void Awake()
    {
        StartCoroutine(StartUpEvent());
    }

    private IEnumerator StartUpEvent()
    {
        ICentralizeEventSystem eventSystem;
        while(!ServiceProvider.TryGetService(out eventSystem))
            yield return null;
        
        eventSystem.Register(GameplayManagerKeys.WinCondition, _event);
    }
    
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

        _event?.Invoke();
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.CompareTag(Tags.Player))
            _isPlayerInside = false;
    }
}