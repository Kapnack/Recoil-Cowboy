using System.Collections;
using System.Collections.Generic;
using Characters.EnemySRC;
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
        ICentralizeEventSystem eventSystem;

        ServiceProvider.TryGetService(out eventSystem);
        eventSystem.Register(GameplayManagerKeys.ChangeLevel, _winCondition);
        eventSystem.Register(GameplayManagerKeys.GameOverMenu, _loseCondition);
    }

    private IEnumerator Start()
    {
        ICentralizeEventSystem eventSystem;
        ServiceProvider.TryGetService(out eventSystem);

        SimpleEvent simpleEvent;
        
        while(!eventSystem.TryGet(GameplayManagerKeys.WinCondition, out simpleEvent))
            yield return null;
        simpleEvent.AddListener(OnWinConditionMeet);


        while(!eventSystem.TryGet(GameplayManagerKeys.LoseCondition, out simpleEvent))
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

public static class GameplayManagerKeys
{
    public const string WinCondition = "WIN_CONDITION";
    public const string LoseCondition = "LOSE_CONDITION";

    public const string ChangeLevel = "CHANGE_LEVEL";
    public const string GameOverMenu = "GAME_OVER_MENU";
}