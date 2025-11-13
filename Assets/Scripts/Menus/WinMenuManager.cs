using Systems;
using Systems.CentralizeEventSystem;
using TMPro;
using UnityEngine;

namespace Menus
{
    public class WinMenuManager : MonoBehaviour
    {
        private readonly SimpleEvent _loadNextLevelEvent = new();
        private readonly SimpleEvent _loadMainMenu = new();
        
        private PointsStats _stats;
        
        [SerializeField] private TMP_Text disntanceText;
        private string _distanceTextFormat;

        [SerializeField] private TMP_Text pointsText;
        private string _pointsTextFormat;

        private void Awake()
        {
            if (!ServiceProvider.TryGetService(out ICentralizeEventSystem eventSystem)) 
                return;
            
            eventSystem.Register(GameManagerKeys.ChangeToLevel, _loadNextLevelEvent);
            eventSystem.Register(GameManagerKeys.MainMenu, _loadMainMenu);
            
            _pointsTextFormat =  pointsText.text;
            pointsText.text = string.Format(_pointsTextFormat, _stats.CurrentKillPoints, _stats.RecordKillPoints);
            disntanceText.text = string.Format(_pointsTextFormat, _stats.CurrentDistance, _stats.RecordKillPoints);
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