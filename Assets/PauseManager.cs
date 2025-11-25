using System;
using Systems;
using Systems.CentralizeEventSystem;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PauseManager : MonoBehaviour
{
    [Header("Panels")] [SerializeField] private GameObject panel;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject tutorial;

    [Header("Buttons")] [SerializeField] private Button continueButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button resetButton;

    private bool _paused;

    private IInputReader _inputReader;

    private GameObject _menuGo;
    private GameObject _tutorialGo;

    private CentralizeEventSystem _eventSystem;

    private void Awake()
    {
        _inputReader = ServiceProvider.GetService<IInputReader>();

        panel.gameObject.SetActive(_paused);

        _eventSystem = ServiceProvider.GetService<CentralizeEventSystem>();

        _eventSystem.AddListener<PausedInput>(PauseHandler);

        SetUpButtons();
    }

    private void SetUpButtons()
    {
        continueButton.onClick.AddListener(Continue);
        settingsButton.onClick.AddListener(Settings);
        tutorialButton.onClick.AddListener(OnTutorial);
        mainMenuButton.onClick.AddListener(GoToMainMenu);
        resetButton.onClick.AddListener(ReloadGameplay);
    }

    private void OnEnable() => Cursor.visible = true;

    private void OnDestroy()
    {
        _eventSystem.RemoveListener<PausedInput>(PauseHandler);

        Time.timeScale = 1.0f;
    }

    public void PauseHandler()
    {
        _paused = !_paused;
        panel.gameObject.SetActive(_paused);

        _inputReader?.SwitchPlayerMapState();
        Time.timeScale = _paused ? 0.0f : 1.0f;

        if (_menuGo)
            Destroy(_menuGo);
    }

    private void Continue()
    {
        Cursor.visible = false;
        PauseHandler();
    }

    public void OnTutorial()
    {
        if (_tutorialGo) 
            return;
        
        _tutorialGo = Instantiate(tutorial);
        _tutorialGo.GetComponent<VideoPlayerManager>().CloseCallback = OnReturnFromTutorial;
        _eventSystem.RemoveListener<PausedInput>(PauseHandler);
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void OnReturnFromTutorial()
    {
        _eventSystem.AddListener<PausedInput>(PauseHandler);
    }

    private void Settings()
    {
        _menuGo = Instantiate(settingsMenu);
        Cursor.visible = true;
    }

    private void GoToMainMenu() => _eventSystem.Get<LoadMainMenu>()?.Invoke();

    private void ReloadGameplay() => _eventSystem.Get<LoadGameplay>()?.Invoke();
}