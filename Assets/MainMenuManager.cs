using Systems;
using Systems.CentralizeEventSystem;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject creditsMenu;
    
    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button exitButton;
    
    private CentralizeEventSystem _eventSystem;
    
    private void Awake()
    {
        _eventSystem = ServiceProvider.GetService<CentralizeEventSystem>();
        
        playButton.onClick.AddListener(OnGame);
        settingsButton.onClick.AddListener(OnSettings);
        creditsButton.onClick.AddListener(OnCredits);
        exitButton.onClick.AddListener(OnExitGame);
    }

    private void OnGame() => _eventSystem.Get<LoadGameplay>()?.Invoke();

    private void OnSettings() => Instantiate(settingsMenu);
    
    private void OnCredits() => Instantiate(creditsMenu);

    private void OnExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}