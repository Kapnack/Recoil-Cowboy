using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Systems;
using Systems.CentralizeEventSystem;
using Systems.SceneLoader;
using UnityEngine;
using UnityEngine.SceneManagement;

public delegate void LoadingStarted();
public delegate void LoadingEnded();
public delegate void LoadGameplay();
public delegate void LoadMainMenu();
public delegate void LoadGameOver();
public delegate void UpdateStats(int killPoints, int distance);

[RequireComponent(typeof(SceneLoader))]
public class GameManager : MonoBehaviour, IGameStats
{
    [SerializeField] private SceneRef mainMenuScene;
    [SerializeField] private SceneRef gameplay;
    [SerializeField] private SceneRef winScene;
    [SerializeField] private SceneRef gameOverScene;

    [SerializeField] private PointsStats stats;

    private readonly CentralizeEventSystem _eventSystem = new();

    private Camera _mainCamera;

    private readonly SceneRef[] _scenesToLoad = new SceneRef[1];

    private ISceneLoader _sceneLoader;

    private IInputReader _inputReader;

    public int TimesPlayed { get; private set; }
    
    private void Awake()
    {
        _mainCamera = Camera.main;
        _sceneLoader = GetComponent<ISceneLoader>();
        
        _inputReader = GetComponent<IInputReader>();
        
        ServiceProvider.SetService(_eventSystem);
        ServiceProvider.SetService<IGameStats>(this);
        
        _eventSystem.AddListener<LoadGameplay>(LoadGameplay);
        _eventSystem.AddListener<LoadMainMenu>(LoadMainMenu);
        _eventSystem.AddListener<LoadGameOver>(LoadGameOverMenu);
        _eventSystem.AddListener<UpdateStats>(SetNewStats);
    }

    private void Start() => LoadMainMenu();

    private async void LoadMainMenu()
    {
        try
        {
            _inputReader.DeactivatePlayerMap();

            _scenesToLoad[0] = mainMenuScene;
            await TryLoadScenes(_scenesToLoad);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private async void LoadGameplay()
    {
        try
        {
            _inputReader.ActivePlayerMap();
            ++TimesPlayed;
            _scenesToLoad[0] = gameplay;
            await TryLoadScenes(_scenesToLoad);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private async void LoadGameOverMenu()
    {
        try
        {
            _inputReader.DeactivatePlayerMap();

            _scenesToLoad[0] = gameOverScene;
            await TryLoadScenes(_scenesToLoad);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void SetNewStats(int killPoints, int distance)
    {
        stats.CurrentDistance = distance;
        if (distance > stats.RecordDistance)
        {
            stats.RecordDistance = distance;
            stats.NewDistanceRecord = true;
        }

        stats.CurrentKillPoints = killPoints;
        if (killPoints > stats.RecordKillPoints)
        {
            stats.RecordKillPoints = killPoints;
            stats.NewKillPointsRecord = true;
        }
    }

    private async Task TryLoadScenes(SceneRef[] sceneRefs)
    {
        _eventSystem.Get<LoadingStarted>()?.Invoke();

        try
        {
            _mainCamera.transform.SetParent(null);
            SceneManager.MoveGameObjectToScene(_mainCamera.gameObject, gameObject.scene);

            await _sceneLoader.UnloadAll();
            await _sceneLoader.LoadSceneAsync(sceneRefs);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        _eventSystem.Get<LoadingEnded>()?.Invoke();
    }
}

public interface IGameStats
{
    public int TimesPlayed { get; }
}