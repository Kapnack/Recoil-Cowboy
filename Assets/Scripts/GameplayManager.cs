using System.Collections;
using System.Collections.Generic;
using Characters.EnemySRC;
using Shaders;
using Systems;
using Systems.CentralizeEventSystem;
using UnityEngine;

public delegate void AlmostDead();
public delegate void PlayerDied(int killPoints, int distance);
public class GameplayManager : MonoBehaviour
{
    private IShaderManager _shaderManager;

    private List<Enemy> _enemies;

    private CentralizeEventSystem _eventSystem;
    
    private void Awake()
    {
        _shaderManager = ServiceProvider.GetService<IShaderManager>();
        _eventSystem =  ServiceProvider.GetService<CentralizeEventSystem>();
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Start()
    {
        _eventSystem.AddListener<AlmostDead>(OnPlayerOneLive);
        _eventSystem.AddListener<PlayerDied>(OnLoseConditionMeet);
    }
    
    private void OnDisable()
    {
        _shaderManager?.StartOffTransition();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void OnDestroy()
    {
        _eventSystem.RemoveListener<AlmostDead>(OnPlayerOneLive);
        _eventSystem.RemoveListener<PlayerDied>(OnLoseConditionMeet);
    }

    private void OnLoseConditionMeet(int killPoints, int distance)
    {
        _eventSystem.Get<UpdateStats>()?.Invoke(killPoints, distance);
        _eventSystem.Get<LoadGameOver>()?.Invoke();
    }

    private void OnPlayerOneLive()
    {
        _shaderManager?.StartOnTransition();
    }
}