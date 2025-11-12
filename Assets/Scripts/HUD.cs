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

    [Header("Ammo Settings")] 
    [SerializeField] private Image ammoPrefab;
    [SerializeField] private RectTransform ammoStarPoint;
    [SerializeField] private Sprite ammoFill;
    [SerializeField] private Sprite ammoBase;
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