using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class VariableEditor : MonoBehaviour
{
    [Header("Scriptable Object")] [SerializeField]
    private PlayerConfig playerConfig;
    
    [Header("Max Ammo")] 
    [SerializeField] private Slider maxAmmoSlider;
    [SerializeField] private TMP_Text maxAmmo;
    private string _maxAmmoFormat;
    
    [Header("Max Ammo")] 
    [SerializeField] private Slider maxLivesSlider;
    [SerializeField] private TMP_Text maxLives;
    private string _maxLivesFormat;

    private void Awake()
    {
        _maxAmmoFormat = maxAmmo.text;
        maxAmmo.text = string.Format(_maxAmmoFormat, playerConfig.MaxBullets);
        
        maxAmmoSlider.onValueChanged.AddListener(ModifyAmmo);
        //--------------------------------------------------------------------------------
        _maxLivesFormat = maxLives.text;
        maxLives.text = string.Format(_maxLivesFormat, playerConfig.MaxBullets);
        
        maxLivesSlider.onValueChanged.AddListener(ModifyLives);
    }

    private void ModifyAmmo(float value)
    {
        playerConfig.MaxBullets = (int)value;
        maxAmmo.text = string.Format(_maxAmmoFormat, playerConfig.MaxBullets);
    }

    private void ModifyLives(float value)
    {
        playerConfig.MaxLives = (int)value;
        maxLives.text = string.Format(_maxLivesFormat, playerConfig.MaxLives);
    }
    
}