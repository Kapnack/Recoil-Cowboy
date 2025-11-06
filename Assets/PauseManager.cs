using Systems;
using Systems.CentralizeEventSystem;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    private ICentralizeEventSystem _eventSystem;
    private readonly SimpleEvent _loadMainMenu = new();

    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject settingsMenu;
    private bool _paused;

    private IInputReader _inputReader;

    private GameObject _menuGo;

    private void Awake()
    {
        _inputReader = ServiceProvider.GetService<IInputReader>();

        panel.gameObject.SetActive(_paused);

        _eventSystem = ServiceProvider.GetService<ICentralizeEventSystem>();

        if (_eventSystem == null)
            return;

        _eventSystem.Register(GameManagerKeys.MainMenu, _loadMainMenu);

        _eventSystem.Get(PlayerEventKeys.Paused).AddListener(PauseHandler);
    }

    private void OnEnable() => Cursor.visible = true;

    private void OnDestroy()
    {
        _eventSystem = ServiceProvider.GetService<ICentralizeEventSystem>();

        if (_eventSystem == null)
            return;

        _eventSystem.Unregister(GameManagerKeys.MainMenu);
        _eventSystem.Get(PlayerEventKeys.Paused).RemoveListener(PauseHandler);

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

    public void GoToMainMenu() => _loadMainMenu?.Invoke();

    public void ExitGame()
    {
    }
}