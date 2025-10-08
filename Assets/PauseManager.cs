using Systems;
using Systems.CentralizeEventSystem;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseManager : MonoBehaviour
{
    private ICentralizeEventSystem _eventSystem;
    private SimpleEvent _loadMainMenu = new();

    [SerializeField] private GameObject panel;
    private bool _paused = false;

    private IInputReader _inputReader;

    private void Awake()
    {
        _inputReader = ServiceProvider.GetService<IInputReader>();
        
        panel.gameObject.SetActive(_paused);

        _eventSystem = ServiceProvider.GetService<ICentralizeEventSystem>();

        _eventSystem.Register(GameManagerKeys.MainMenu, _loadMainMenu);

        _eventSystem.Get(PlayerEventKeys.Paused).AddListener(PauseHandler);
    }

    private void OnDestroy()
    {
        _eventSystem.Get(PlayerEventKeys.Paused).RemoveListener(PauseHandler);
        _eventSystem.Unregister(GameManagerKeys.MainMenu);
        Time.timeScale = 1.0f;
    }

    private void PauseHandler()
    {
        _paused = !_paused;
        panel.gameObject.SetActive(_paused);
        _inputReader.SwitchPlayerMapState();
        Time.timeScale = _paused ? 0.0f : 1.0f;
    }

    public void GoToMainMenu() => _loadMainMenu?.Invoke();

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}