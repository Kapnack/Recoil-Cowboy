using Systems;
using Systems.CentralizeEventSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menus
{
    public class WinMenuManager : MonoBehaviour
    {
        private readonly SimpleEvent _loadNextLevelEvent = new();
        private readonly SimpleEvent _loadMainMenu = new();
        
        [SerializeField] private PointsStats stats;
        
        [SerializeField] private TMP_Text disntanceText;
        private string _distanceTextFormat;

        [SerializeField] private TMP_Text pointsText;
        private string _pointsTextFormat;
        
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button mainMenuButton;

        private void Awake()
        {
            nextLevelButton.onClick.AddListener(OnNextLevel);
            mainMenuButton.onClick.AddListener(OnMainMenu);
            
            if (!ServiceProvider.TryGetService(out ICentralizeEventSystem eventSystem)) 
                return;
            
            eventSystem.Register(GameManagerKeys.ChangeToLevel, _loadNextLevelEvent);
            eventSystem.Register(GameManagerKeys.MainMenu, _loadMainMenu);
            
            _pointsTextFormat =  pointsText.text;
            _distanceTextFormat =  disntanceText.text;
            pointsText.text = string.Format(_pointsTextFormat, stats.CurrentKillPoints, stats.RecordKillPoints);
            disntanceText.text = string.Format(_distanceTextFormat, stats.CurrentDistance, stats.RecordDistance);
        }

        private void OnDestroy()
        {
            if (!ServiceProvider.TryGetService(out ICentralizeEventSystem eventSystem)) 
                return;
            
            eventSystem.Unregister(GameManagerKeys.ChangeToLevel);
            eventSystem.Unregister(GameManagerKeys.MainMenu);
        }

        private void OnNextLevel() => _loadNextLevelEvent?.Invoke();

        private void OnMainMenu() => _loadMainMenu?.Invoke();
    }
}