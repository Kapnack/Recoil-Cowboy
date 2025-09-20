using System.Collections;
using System.Collections.Generic;
using Characters.EnemySRC;
using Characters.PlayerSRC;
using Systems;
using Systems.CentralizeEventSystem;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    private List<Enemy> _enemies;

    private readonly SimpleEvent _winCondition = new();
    private readonly SimpleEvent _loseCondition = new();

    private void Awake()
    {
        SetUpEvents();
    }

    private void SetUpEvents()
    {
        if (ServiceProvider.TryGetService(out ICentralizeEventSystem eventSystem))
        {
            eventSystem.Register(GameplayManagerKeys.WinCondition, _winCondition);
            eventSystem.Register(GameplayManagerKeys.LoseCondition, _loseCondition);
        }
    }

    private void OnDestroy()
    {
        if (ServiceProvider.TryGetService(out ICentralizeEventSystem eventSystem))
        {
            eventSystem.Unregister(GameplayManagerKeys.WinCondition);
            eventSystem.Unregister(GameplayManagerKeys.LoseCondition);
        }
    }

    private IEnumerator Start()
    {
        ServiceProvider.TryGetService(out ICentralizeEventSystem eventSystem);

        SimpleEvent simpleEvent;

        while (!eventSystem.TryGet(PlayerEventKeys.Wins, out simpleEvent))
            yield return null;
        simpleEvent.AddListener(OnWinConditionMeet);


        while (!eventSystem.TryGet(PlayerEventKeys.Dies, out simpleEvent))
            yield return null;

        simpleEvent.AddListener(OnLoseConditionMeet);
    }

    private void OnWinConditionMeet()
    {
        _winCondition?.Invoke();
    }

    private void OnLoseConditionMeet()
    {
        _loseCondition?.Invoke();
    }
}