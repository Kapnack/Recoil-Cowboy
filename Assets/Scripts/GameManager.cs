using System;
using System.Collections.Generic;
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

    private ISceneLoader _sceneLoader;

    private int _currentLevel;

    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _sceneLoader = GetComponent<ISceneLoader>();
        LoadMainMenu();
    }

    private async void LoadMainMenu()
    {
        try
        {
            _mainCamera.transform.SetParent(null);
            SceneManager.MoveGameObjectToScene(_mainCamera.gameObject, gameObject.scene);
            _sceneLoader.UnloadAll();
            await _sceneLoader.LoadSceneAsync(mainMenuScene);
            FindAfterMatchMenuEvents();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private async void LoadCurrentLevel()
    {
        try
        {
            _mainCamera.transform.SetParent(null);
            SceneManager.MoveGameObjectToScene(_mainCamera.gameObject, gameObject.scene);
            _sceneLoader.UnloadAll();
            await _sceneLoader.LoadSceneAsync(levels[_currentLevel]);

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
            _mainCamera.transform.SetParent(null);
            SceneManager.MoveGameObjectToScene(_mainCamera.gameObject, gameObject.scene);
            _sceneLoader.UnloadAll();
            await _sceneLoader.LoadSceneAsync(winScene);

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
            _mainCamera.transform.SetParent(null);
            SceneManager.MoveGameObjectToScene(_mainCamera.gameObject, gameObject.scene);
            _sceneLoader.UnloadAll();
            await _sceneLoader.LoadSceneAsync(gameOverScene);

            FindAfterMatchMenuEvents();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void LevelCleared() => ++_currentLevel;
    
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