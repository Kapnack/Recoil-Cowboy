using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.U2D;
using UnityEngine.UI;

public class CreditsManager : MonoBehaviour
{
    [Header("Images")] [SerializeField] private List<Sprite> creditsPages;

    [Header("Buttons")] [SerializeField] private Button returnButton;
    [SerializeField] private Button previousPageButton;
    [SerializeField] private Button nextPageButton;

    [Header("Panel")]
    [SerializeField] private Image creditsPanel;

    private int _currentPageIndex;
    private int CurrentPageIndex
    {
        get => _currentPageIndex;
        set
        {
            if (value < 0)
                _currentPageIndex = 0;
            else if (value >= creditsPages.Count)
                _currentPageIndex = creditsPages.Count - 1;
            else
                _currentPageIndex = value;
            
            creditsPanel.sprite = creditsPages[_currentPageIndex];
        }
    }

    private void Awake()
    {
        creditsPanel = GetComponentInChildren<Image>();

        returnButton.onClick.AddListener(OnExitCredits);
        previousPageButton.onClick.AddListener(OnPreviousPage);
        nextPageButton.onClick.AddListener(OnNextPage);

        CurrentPageIndex = 0;
    }

    private void OnPreviousPage() => --CurrentPageIndex;
    private void OnNextPage() => ++CurrentPageIndex;
    private void OnExitCredits() => Destroy(gameObject);
}