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
    [SerializeField] private Animator livesFill;
    [SerializeField] private TMP_Text killPointsText;
    [SerializeField] private TMP_Text disntanceText;

    [Header("Ammo Settings")] [SerializeField]
    private GameObject ammoPrefab;

    [SerializeField] private RectTransform ammoStarPoint;
    private Animator[] _bulletsAnimator;

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
        livesFill.SetInteger("hitsRecived", -(current - max) );
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
            _bulletsAnimator = new Animator[max];

            float padding = 0f;

            Vector2 currentPos = ammoStarPoint.GetComponent<RectTransform>().anchoredPosition;

            for (int i = 0; i < max; ++i)
            {
                GameObject gameObject = Instantiate(ammoPrefab, ammoStarPoint.transform.parent, false);
                RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
                _bulletsAnimator[i] = gameObject.GetComponent<Animator>();

                _bulletsAnimator[i].SetBool("loaded", true);

                rectTransform.anchoredPosition = currentPos;

                float width = rectTransform.rect.width * rectTransform.localScale.x;
                currentPos += new Vector2(width + padding, 0);
            }

            _firstReload = false;
        }

        if (previous > current)
        {
            _bulletsAnimator[previous - 1].SetBool("loaded", false);
        }
        else if (previous < current)
        {
            _bulletsAnimator[current - 1].SetBool("loaded", true);
        }
    }
}