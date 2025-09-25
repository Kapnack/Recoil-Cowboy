using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Systems;
using Systems.CentralizeEventSystem;
using Systems.SceneLoader;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SceneLoader))]
public class GameManager : MonoBehaviour
{
    [SerializeField] private SceneRef mainMenuScene;
    [SerializeField] private List<SceneRef> levels = new();
    [SerializeField] private SceneRef winScene;
    [SerializeField] private SceneRef gameOverScene;

    private SceneRef[] scenesToLoad = new SceneRef[2];

    private ISceneLoader _sceneLoader;

    private SimpleEvent _loadingStarted = new();
    private SimpleEvent _loadingEnded = new();

    private int _currentLevel;

    private int CurrentLevel
    {
        get => _currentLevel;
        set
        {
            var newValue = value;

            if (newValue > levels.Count - 1)
                newValue = 0;

            _currentLevel = newValue;
        }
    }

    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _sceneLoader = GetComponent<ISceneLoader>();

        StartCoroutine(SetUpEvents());
    }

    private IEnumerator SetUpEvents()
    {
        ICentralizeEventSystem eventSystem;
        while (!ServiceProvider.TryGetService(out eventSystem))
            yield return null;

        eventSystem.Register(GameManagerKeys.LoadingStarted, _loadingStarted);
        eventSystem.Register(GameManagerKeys.LoadingEnded, _loadingEnded);

        while (!_loadingStarted.HasInvocations())
            yield return null;

        LoadMainMenu();
    }

    private async void LoadMainMenu()
    {
        scenesToLoad[0] = mainMenuScene;
        await TryLoadScenes(scenesToLoad);
        
        FindAfterMatchMenuEvents();
    }

    private async void LoadCurrentLevel()
    {
        try
        {
            scenesToLoad[0] = levels[CurrentLevel];
            await TryLoadScenes(scenesToLoad);

            FindGameplayEvents();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private async void LoadWinScene()
    {
        try
        {
            scenesToLoad[0] = winScene;
            await TryLoadScenes(scenesToLoad);
        
            FindAfterMatchMenuEvents();
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
            scenesToLoad[0] = gameOverScene;
            await TryLoadScenes(scenesToLoad);
        
            FindAfterMatchMenuEvents();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private async Task TryLoadScenes(SceneRef[] sceneRefs)
    {
        _loadingStarted.Invoke();
        
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
        
        _loadingEnded.Invoke();
    }

    private void LevelCleared() => ++CurrentLevel;

    private void FindGameplayEvents()
    {
        ServiceProvider.TryGetService(out ICentralizeEventSystem eventSystem);

        eventSystem.TryGet(GameplayManagerKeys.WinCondition, out var simpleEvent);

        simpleEvent.AddListener(LevelCleared);
        simpleEvent.AddListener(LoadWinScene);

        eventSystem.TryGet(GameplayManagerKeys.LoseCondition, out simpleEvent);

        simpleEvent.AddListener(LoadGameOverMenu);
    }

    private void FindAfterMatchMenuEvents()
    {
        ServiceProvider.TryGetService(out ICentralizeEventSystem eventSystem);

        eventSystem.TryGet(GameManagerKeys.ChangeToLevel, out var simpleEvent);

        simpleEvent.AddListener(LoadCurrentLevel);

        eventSystem.TryGet(GameManagerKeys.MainMenu, out simpleEvent);

        simpleEvent.AddListener(LoadMainMenu);
    }
}