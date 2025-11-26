using System.Collections;
using System.Collections.Generic;
using Characters.EnemySRC;
using Shaders;
using Systems;
using Systems.CentralizeEventSystem;
using UnityEngine;
using UnityEngine.Serialization;

public delegate void AlmostDead();

public delegate void PlayerDied(int killPoints, int distance);

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseHUD;

    private IShaderManager _shaderManager;

    private List<Enemy> _enemies;

    private CentralizeEventSystem _eventSystem;


    private void Awake()
    {
        _shaderManager = ServiceProvider.GetService<IShaderManager>();
        _eventSystem = ServiceProvider.GetService<CentralizeEventSystem>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Start()
    {
        _eventSystem.AddListener<AlmostDead>(OnPlayerOneLive);
        _eventSystem.AddListener<PlayerDied>(OnLoseConditionMeet);

        GameObject pauseHUDGo = Instantiate(pauseHUD, transform);
        PauseManager pauseManager = pauseHUDGo.GetComponent<PauseManager>();

        if (ServiceProvider.GetService<IGameStats>().TimesPlayed == 1)
        {
            pauseManager.PauseHandler();
            pauseManager.OnTutorial();
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