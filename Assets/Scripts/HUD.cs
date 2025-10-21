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
    [SerializeField] private Image ammoFill;
    [SerializeField] private Image ammoBase;

    private float _ammoImageWidth;

    private bool _firstReload = true;

    private void Awake()
    {
        _ammoImageWidth = ammoBase.rectTransform.sizeDelta.x;
        StartCoroutine(SetUpEvents());
    }

    private IEnumerator SetUpEvents()
    {
        ICentralizeEventSystem eventSystem;

        while (!ServiceProvider.TryGetService(out eventSystem))
            yield return null;

        ComplexGameEvent<int, int, int> complexGameEvent;

        while (!eventSystem.TryGet(PlayerEventKeys.BulletsChange, out complexGameEvent))
            yield return null;

        complexGameEvent.AddListener(OnAmmoChange);

        while (!eventSystem.TryGet(PlayerEventKeys.LivesChange, out complexGameEvent))
            yield return null;

        complexGameEvent.AddListener(OnLivesChange);
    }

    public void OnAmmoChange(int previous, int current, int max)
    {
        if (_firstReload)
        {
            ammoBase.rectTransform.sizeDelta = new Vector2
            (
                _ammoImageWidth * current,
                ammoFill.rectTransform.sizeDelta.y
            );

            _firstReload = false;
        }

        ammoFill.rectTransform.sizeDelta = new Vector2
        (
            _ammoImageWidth * current,
            ammoFill.rectTransform.sizeDelta.y
        );
    }

    public void OnLivesChange(int previous, int current, int max)
    {
        livesFill.fillAmount = (float)current / max;
    }
}