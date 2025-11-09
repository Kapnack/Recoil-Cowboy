using System;
using System.Collections;
using Systems;
using Systems.CentralizeEventSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class HUD : MonoBehaviour
{
    [SerializeField] private Image livesFill;
    [SerializeField] private TMP_Text pointsText;

    [Header("Ammo Settings")] [SerializeField]
    private RectTransform ammoStarPoint;

    [SerializeField] private Image ammoFill;
    [SerializeField] private Image ammoBase;
    private Image[] _ammoImages;

    private string _pointsTextFormat;
    private string _distanceTextFormat;

    private bool _firstReload = true;

    private void Awake()
    {
        _pointsTextFormat = pointsText.text;

        StartCoroutine(SetUpEvents());
    }

    private IEnumerator SetUpEvents()
    {
        ICentralizeEventSystem eventSystem;

        while (!ServiceProvider.TryGetService(out eventSystem))
            yield return null;

        ComplexGameEvent<int, int, int> complexGameEvent;

        while (!eventSystem.TryGet(PlayerEventKeys.LivesChange, out complexGameEvent))
            yield return null;

        complexGameEvent.AddListener(OnLivesChange);

        DoubleParamEvent<int, int> doubleParamEvent;
        while (!eventSystem.TryGet(PlayerEventKeys.PointsChange, out doubleParamEvent))
            yield return null;

        doubleParamEvent.AddListener(OnPointsChange);

        OnPointsChange(0, 0);

        while (!eventSystem.TryGet(PlayerEventKeys.BulletsChange, out complexGameEvent))
            yield return null;

        complexGameEvent.AddListener(OnAmmoChange);
    }

    private void OnLivesChange(int previous, int current, int max)
    {
        livesFill.fillAmount = (float)current / max;
    }

    public void OnPointsChange(int previous, int current)
    {
        pointsText.text = string.Format(_pointsTextFormat, current);
    }

    private void OnAmmoChange(int previous, int current, int max)
    {
        if (_firstReload)
        {
            _ammoImages = new Image[max];

            for (int i = 0; i < max; ++i)
            {
                _ammoImages[i] = Instantiate(ammoFill, ammoStarPoint.gameObject.transform, false);

                if (i == 0)
                    _ammoImages[i].rectTransform.position = ammoStarPoint.position;
                else
                {
                    RectTransform prevRect = _ammoImages[i - 1].rectTransform;
                    RectTransform currentRect = _ammoImages[i].rectTransform;

                    Vector2 newPos = prevRect.anchoredPosition + new Vector2(prevRect.rect.width, 0);

                    currentRect.anchoredPosition = newPos;
                }
            }
            _firstReload = false;
            
            return;
        }

        if (previous > current)
        {
            _ammoImages[previous - 1].sprite = ammoBase.sprite;
        }
        else if (previous < current)
        {
            _ammoImages[current - 1].sprite = ammoFill.sprite;
        }
    }
}