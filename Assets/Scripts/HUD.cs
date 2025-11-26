using System;
using System.Collections;
using Systems;
using Systems.CentralizeEventSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public delegate void LivesChange(int previous, int current, int max);
public delegate void AmmoChange(int previous, int current, int max);
public delegate void DistanceChange(int previous, int current);
public delegate void KillsChange(int previous, int current);

[Serializable]
public class HUD : MonoBehaviour
{
    [SerializeField] private Image livesFill;
    [SerializeField] private TMP_Text killPointsText;
    [SerializeField] private TMP_Text disntanceText;

    [Header("Ammo Settings")] [SerializeField]
    private Image ammoPrefab;

    [SerializeField] private RectTransform ammoStarPoint;
    [SerializeField] private Sprite ammoFill;
    [SerializeField] private Sprite ammoBase;
    private Image[] _ammoImages;

    private string _pointsTextFormat;
    private string _distanceTextFormat;

    private bool _firstReload = true;

    private void Awake()
    {
        _pointsTextFormat = killPointsText.text;

        _distanceTextFormat = disntanceText.text;

        SetUpEvents();
    }

    private void SetUpEvents()
    {
        CentralizeEventSystem eventSystem = ServiceProvider.GetService<CentralizeEventSystem>();

        eventSystem.AddListener<LivesChange>(OnLivesChange);

        eventSystem.AddListener<DistanceChange>(OnDistanceChange);
        OnDistanceChange(0, 0);

        eventSystem.AddListener<KillsChange>(OnKillPointsChange);
        OnKillPointsChange(0, 0);

        eventSystem.AddListener<AmmoChange>(OnAmmoChange);
    }

    private void OnDisable()
    {
        CentralizeEventSystem eventSystem = ServiceProvider.GetService<CentralizeEventSystem>();

        eventSystem.RemoveListener<LivesChange>(OnLivesChange);

        eventSystem.RemoveListener<DistanceChange>(OnDistanceChange);

        eventSystem.RemoveListener<KillsChange>(OnKillPointsChange);

        eventSystem.RemoveListener<AmmoChange>(OnAmmoChange);
    }
    
    private void OnLivesChange(int previous, int current, int max)
    {
        livesFill.fillAmount = (float)current / max;
    }

    private void OnKillPointsChange(int previous, int current)
    {
        killPointsText.text = string.Format(_pointsTextFormat, current);
    }

    private void OnDistanceChange(int previous, int current)
    {
        disntanceText.text = string.Format(_distanceTextFormat, current);
    }

    private void OnAmmoChange(int previous, int current, int max)
    {
        if (_firstReload)
        {
            _ammoImages = new Image[max];

            float scale = 0.1f;
            float padding = 0f;

            Vector2 startPos = Vector2.zero;

            for (int i = 0; i < max; ++i)
            {
                _ammoImages[i] = Instantiate(ammoPrefab, ammoStarPoint, false);

                _ammoImages[i].sprite = ammoFill;

                _ammoImages[i].SetNativeSize();
                _ammoImages[i].rectTransform.localScale = new Vector3(scale, scale, 1f);

                if (i == 0)
                {
                    _ammoImages[i].rectTransform.anchoredPosition = startPos;
                }
                else
                {
                    RectTransform prevRect = _ammoImages[i - 1].rectTransform;

                    float prevWidth = prevRect.rect.width * prevRect.localScale.x;
                    Vector2 newPos = prevRect.anchoredPosition + new Vector2(prevWidth + padding, 0);

                    _ammoImages[i].rectTransform.anchoredPosition = newPos;
                }
            }

            _firstReload = false;
        }

        if (previous > current)
        {
            _ammoImages[previous - 1].sprite = ammoBase;
        }
        else if (previous < current)
        {
            _ammoImages[current - 1].sprite = ammoFill;
        }
    }
}