using System;
using System.Collections;
using System.Collections.Generic;
using Systems;
using Systems.CentralizeEventSystem;
using Systems.SceneLoader;
using UnityEngine;

[RequireComponent(typeof(SceneLoader))]
public class GameManager : MonoBehaviour
{
    [SerializeField] private SceneRef mainMenuScene;
    [SerializeField] private List<SceneRef> levels = new();
    [SerializeField] private SceneRef winScene;
    [SerializeField] private SceneRef gameOverScene;

    private ISceneLoader _sceneLoader;

    private int _currentLevel;

    private void Awake()
    {
        _sceneLoader = GetComponent<ISceneLoader>();
        LoadCurrentLevel();
    }

    private void LoadMainMenu()
    {
        _sceneLoader.UnloadAll();
        _sceneLoader.LoadSceneAsync(mainMenuScene);
    }

    private async void LoadCurrentLevel()
    {
        try
        {
            _sceneLoader.UnloadAll();
            await _sceneLoader.LoadSceneAsync(levels[_currentLevel]);

            FindGameplayConditions();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void LoadWinScene()
    {
        _sceneLoader.UnloadAll();
        _sceneLoader.LoadSceneAsync(winScene);
    }

    private void LoadGameOverMenu()
    {
        _sceneLoader.UnloadAll();
        _sceneLoader.LoadSceneAsync(mainMenuScene);
    }

    private async void LoadNextLevel()
    {
        try
        {
            _sceneLoader.UnloadAll();
            await _sceneLoader.LoadSceneAsync(levels[_currentLevel + 1]);
            ++_currentLevel;

            FindGameplayConditions();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void FindGameplayConditions()
    {
        ICentralizeEventSystem eventSystem;

        ServiceProvider.TryGetService(out eventSystem);

        SimpleEvent simpleEvent;
        
        eventSystem.TryGet(GameplayManagerKeys.ChangeLevel, out simpleEvent);

        simpleEvent.AddListener(LoadWinScene);

        eventSystem.TryGet(GameplayManagerKeys.GameOverMenu, out simpleEvent);

        simpleEvent.AddListener(LoadGameOverMenu);
    }
}