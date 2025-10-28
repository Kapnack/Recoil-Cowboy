using System.Collections;
using System.Collections.Generic;
using Characters.EnemySRC;
using Shaders;
using Systems;
using Systems.CentralizeEventSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameplayManager : MonoBehaviour
{
    private IShaderManager _shaderManager;

    private List<Enemy> _enemies;
    
    private readonly SimpleEvent _loseCondition = new();

    private void Awake()
    {
        SetUpEvents();

        _shaderManager = ServiceProvider.GetService<IShaderManager>();
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }
    
    private void SetUpEvents()
    {
        if (ServiceProvider.TryGetService(out ICentralizeEventSystem eventSystem))
        {
            eventSystem.Register(GameplayManagerKeys.LoseCondition, _loseCondition);
        }
    }

    private void OnDisable()
    {
        _shaderManager?.StartOffTransition();
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void OnDestroy()
    {
        if (ServiceProvider.TryGetService(out ICentralizeEventSystem eventSystem))
        {
            eventSystem.Unregister(GameplayManagerKeys.LoseCondition);
        }
    }

    private IEnumerator Start()
    {
        ServiceProvider.TryGetService(out ICentralizeEventSystem eventSystem);

        SimpleEvent simpleEvent;

        while (!eventSystem.TryGet(PlayerEventKeys.OnOneLive, out simpleEvent))
            yield return null;

        simpleEvent.AddListener(OnPlayerOneLive);

        while (!eventSystem.TryGet(PlayerEventKeys.Dies, out simpleEvent))
            yield return null;

        simpleEvent.AddListener(OnLoseConditionMeet);
    }

    private void OnLoseConditionMeet()
    {
        _loseCondition?.Invoke();
    }

    private void OnPlayerOneLive()
    {
        _shaderManager?.StartOnTransition();
    }
}