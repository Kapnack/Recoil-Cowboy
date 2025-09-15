using System;
using System.Collections;
using Systems;
using Systems.CentralizeEventSystem;
using TMPro;
using UnityEngine;

[Serializable]
public class HUD : MonoBehaviour
{
    [SerializeField] private GameObject ammoCanvas;
    [SerializeField] private GameObject livesCanvas;

    private TMP_Text _ammo;
    private string _ammoFormat;

    private TMP_Text _lives;
    private string _livesFormat;


    private void Awake()
    {
        _ammo = ammoCanvas.GetComponent<TMP_Text>();
        _ammoFormat = _ammo.text;
        _ammo.text = string.Format(_ammoFormat, 0, 0);

        _lives = livesCanvas.GetComponent<TMP_Text>();
        _livesFormat = _lives.text;
        _lives.text = string.Format(_livesFormat, 0, 0);

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
        _ammo.text = string.Format(_ammoFormat, current, max);
    }

    public void OnLivesChange(int previous, int current, int max)
    {
        _lives.text = string.Format(_livesFormat, current, max);
    }
}