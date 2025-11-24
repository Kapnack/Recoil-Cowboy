using Systems;
using Systems.CentralizeEventSystem;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject creditsMenu;

    private CentralizeEventSystem _eventSystem;
    
    private void Awake()
    {
        _eventSystem = ServiceProvider.GetService<CentralizeEventSystem>();
    }

    public void OnNextLevel() => _eventSystem.Get<LoadGameplay>()?.Invoke();

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