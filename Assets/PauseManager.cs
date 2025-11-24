using Systems;
using Systems.CentralizeEventSystem;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject settingsMenu;
    private bool _paused;

    private IInputReader _inputReader;

    private GameObject _menuGo;

    private CentralizeEventSystem _eventSystem;
    
    private void Awake()
    {
        _inputReader = ServiceProvider.GetService<IInputReader>();

        panel.gameObject.SetActive(_paused);

        _eventSystem = ServiceProvider.GetService<CentralizeEventSystem>();

        _eventSystem?.AddListener<PausedInput>(PauseHandler);
    }

    private void OnEnable() => Cursor.visible = true;

    private void OnDestroy()
    {
        _eventSystem.RemoveListener<PausedInput>(PauseHandler);

        Time.timeScale = 1.0f;
    }

    private void PauseHandler()
    {
        _paused = !_paused;
        panel.gameObject.SetActive(_paused);

        _inputReader?.SwitchPlayerMapState();
        Time.timeScale = _paused ? 0.0f : 1.0f;

        if (_menuGo)
            Destroy(_menuGo);
    }

    public void Continue()
    {
        Cursor.visible = false;
        PauseHandler();
    }

    public void Settings()
    {
        _menuGo = Instantiate(settingsMenu);
        Cursor.visible = true;
    }

    public void GoToMainMenu() => _eventSystem.Get<LoadMainMenu>()?.Invoke();

    public void ReloadGameplay() => _eventSystem.Get<LoadGameplay>()?.Invoke();
}