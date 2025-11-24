using Systems;
using Systems.CentralizeEventSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menus
{
    public class WinMenuManager : MonoBehaviour
    {
        [SerializeField] private PointsStats stats;

        [SerializeField] private TMP_Text disntanceText;
        private string _distanceTextFormat;

        [SerializeField] private TMP_Text pointsText;
        private string _pointsTextFormat;

        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button mainMenuButton;

        private CentralizeEventSystem _eventSystem;

        private void Awake()
        {
            _eventSystem = ServiceProvider.GetService<CentralizeEventSystem>();

            nextLevelButton.onClick.AddListener(OnNextLevel);
            mainMenuButton.onClick.AddListener(OnMainMenu);

            _pointsTextFormat = pointsText.text;
            _distanceTextFormat = disntanceText.text;
            pointsText.text = string.Format(_pointsTextFormat, stats.CurrentKillPoints, stats.RecordKillPoints);
            disntanceText.text = string.Format(_distanceTextFormat, stats.CurrentDistance, stats.RecordDistance);
        }

        private void OnNextLevel()
        {
            _eventSystem.Get<LoadGameplay>()?.Invoke();
        }

        private void OnMainMenu()
        {
            _eventSystem.Get<LoadMainMenu>()?.Invoke();
        }
    }
}