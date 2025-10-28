using Systems;
using Systems.CentralizeEventSystem;
using Systems.SceneLoader;
using TMPro;
using UnityEngine;

namespace Menus
{
    public class WinMenuManager : MonoBehaviour
    {
        private readonly SimpleEvent _loadNextLevelEvent = new();
        private readonly SimpleEvent _loadMainMenu = new();
        
        private IStatsManager _statsManager;
        
        [SerializeField] private TMP_Text pointsText;
        private string _pointsTextFormat;
        
        private void Awake()
        {
            if (!ServiceProvider.TryGetService(out ICentralizeEventSystem eventSystem)) 
                return;
            
            eventSystem.Register(GameManagerKeys.ChangeToLevel, _loadNextLevelEvent);
            eventSystem.Register(GameManagerKeys.MainMenu, _loadMainMenu);
            
            _statsManager = ServiceProvider.GetService<IStatsManager>();
            
            _pointsTextFormat =  pointsText.text;
            pointsText.text = string.Format(_pointsTextFormat, _statsManager.LastMatchPoints, _statsManager.RecordPoints);
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
    }
}