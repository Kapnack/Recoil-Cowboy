using System.Collections;
using System.Collections.Generic;
using Characters.EnemySRC;
using Shaders;
using Systems;
using Systems.CentralizeEventSystem;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    private IShaderManager _shaderManager;

    private List<Enemy> _enemies;

    private readonly DoubleParamEvent<int, int> _loseCondition = new();

    private void Awake()
    {
        SetUpEvents();

        _shaderManager = ServiceProvider.GetService<IShaderManager>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void SetUpEvents()
    {
        ICentralizeEventSystem eventSystem = ServiceProvider.GetService<ICentralizeEventSystem>();

        eventSystem.Register(GameplayManagerKeys.LoseCondition, _loseCondition);
    }

    private void OnDisable()
    {
        _shaderManager?.StartOffTransition();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void OnDestroy()
    {
        ICentralizeEventSystem eventSystem = ServiceProvider.GetService<ICentralizeEventSystem>();

        eventSystem.Unregister(GameplayManagerKeys.LoseCondition);
    }

    private IEnumerator Start()
    {
        ServiceProvider.TryGetService(out ICentralizeEventSystem eventSystem);

        SimpleEvent simpleEvent;

        while (!eventSystem.TryGet(PlayerEventKeys.OnOneLive, out simpleEvent))
            yield return null;

        simpleEvent.AddListener(OnPlayerOneLive);

        DoubleParamEvent<int, int> singleParamEvent;
        while (!eventSystem.TryGet(PlayerEventKeys.Dies, out singleParamEvent))
            yield return null;

        singleParamEvent.AddListener(OnLoseConditionMeet);
    }

    private void OnLoseConditionMeet(int killPoints, int distance)
    {
        _loseCondition?.Invoke(killPoints, distance);
    }

    private void OnPlayerOneLive()
    {
        _shaderManager?.StartOnTransition();
    }
}