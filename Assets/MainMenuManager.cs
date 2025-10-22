using Systems;
using Systems.CentralizeEventSystem;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    private readonly SimpleEvent _loadNextLevelEvent = new();
    private readonly SimpleEvent _loadMainMenu = new();
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject creditsMenu;

    private void Awake()
    {
        if (ServiceProvider.TryGetService<ICentralizeEventSystem>(out var eventSystem))
        {
            eventSystem.Register(GameManagerKeys.ChangeToLevel, _loadNextLevelEvent);
            eventSystem.Register(GameManagerKeys.MainMenu, _loadMainMenu);
        }
    }

    private void OnDestroy()
    {
        if (ServiceProvider.TryGetService<ICentralizeEventSystem>(out var eventSystem))
        {
            eventSystem.Unregister(GameManagerKeys.ChangeToLevel);
            eventSystem.Unregister(GameManagerKeys.MainMenu);
        }
    }

    public void OnNextLevel() => _loadNextLevelEvent?.Invoke();

    public void OnSettings() => Instantiate(settingsMenu);
    
    public void OnCredits() => Instantiate(creditsMenu);

    public void OnExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}