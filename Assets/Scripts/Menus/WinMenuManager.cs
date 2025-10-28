using Systems;
using Systems.CentralizeEventSystem;
using UnityEngine;

namespace Menus
{
    public class WinMenuManager : MonoBehaviour
    {
        private readonly SimpleEvent _loadNextLevelEvent = new();
        private readonly SimpleEvent _loadMainMenu = new();

        private void Awake()
        {
            if (!ServiceProvider.TryGetService(out ICentralizeEventSystem eventSystem)) 
                return;
            
            eventSystem.Register(GameManagerKeys.ChangeToLevel, _loadNextLevelEvent);
            eventSystem.Register(GameManagerKeys.MainMenu, _loadMainMenu);
        }

        private void OnDestroy()
        {
            if (!ServiceProvider.TryGetService(out ICentralizeEventSystem eventSystem)) 
                return;
            
            eventSystem.Unregister(GameManagerKeys.ChangeToLevel);
            eventSystem.Unregister(GameManagerKeys.MainMenu);
        }

        public void OnNextLevel() => _loadNextLevelEvent?.Invoke();

        public void OnMainMenu() => _loadMainMenu?.Invoke();

        public void OnExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}