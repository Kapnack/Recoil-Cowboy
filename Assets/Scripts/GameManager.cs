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
    [SerializeField] private SceneRef gameplay;
    [SerializeField] private SceneRef winScene;
    [SerializeField] private SceneRef gameOverScene;

    private Camera _mainCamera;

    private readonly SceneRef[] _scenesToLoad = new SceneRef[1];

    private ISceneLoader _sceneLoader;

    private IInputReader _inputReader;

    private readonly SimpleEvent _loadingStarted = new();
    private readonly SimpleEvent _loadingEnded = new();

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

        while (!ServiceProvider.TryGetService(out _inputReader))
            yield return null;

        while (!_loadingStarted.HasInvocations())
            yield return null;

        LoadMainMenu();
    }

    private async void LoadMainMenu()
    {
        try
        {
            _inputReader.DeactivatePlayerMap();

            _scenesToLoad[0] = mainMenuScene;
            await TryLoadScenes(_scenesToLoad);

            FindAfterMatchMenuEvents();
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

            _scenesToLoad[0] = gameplay;
            await TryLoadScenes(_scenesToLoad);

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
            _inputReader.DeactivatePlayerMap();

            _scenesToLoad[0] = winScene;
            await TryLoadScenes(_scenesToLoad);

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
            _inputReader.DeactivatePlayerMap();

            _scenesToLoad[0] = gameOverScene;
            await TryLoadScenes(_scenesToLoad);

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

    private void FindGameplayEvents()
    {
        ServiceProvider.TryGetService(out ICentralizeEventSystem eventSystem);

        eventSystem.Get(GameplayManagerKeys.LoseCondition).AddListener(LoadGameOverMenu);

        eventSystem.Get(GameManagerKeys.MainMenu).AddListener(LoadMainMenu);
    }

    private void FindAfterMatchMenuEvents()
    {
        ServiceProvider.TryGetService(out ICentralizeEventSystem eventSystem);

        eventSystem.Get(GameManagerKeys.ChangeToLevel).AddListener(LoadGameplay);

        eventSystem.Get(GameManagerKeys.MainMenu).AddListener(LoadMainMenu);
    }
}