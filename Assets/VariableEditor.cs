using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VariableEditor : MonoBehaviour
{
    [Header("Scriptable Object")] [SerializeField]
    private PlayerConfig playerConfig;
    
    [Header("Max Ammo")] 
    [SerializeField] private Slider maxAmmoSlider;
    [SerializeField] private TMP_Text maxAmmo;
    private string _maxAmmoFormat;
    
    [Header("Max Lives")] 
    [SerializeField] private Slider maxLivesSlider;
    [SerializeField] private TMP_Text maxLives;
    private string _maxLivesFormat;
    
    [Header("Knockback")] 
    [SerializeField] private Slider knockbackSlider;
    [SerializeField] private TMP_Text knockback;
    private string _knockbackFormat;
    
    [Header("Knockback Distance Modifier")] 
    [SerializeField] private Slider modifierSlider;
    [SerializeField] private TMP_Text modifier;
    private string _modifierFormat;

    private void Awake()
    {
        _maxAmmoFormat = maxAmmo.text;
        maxAmmo.text = string.Format(_maxAmmoFormat, playerConfig.MaxBullets);
        
        maxAmmoSlider.onValueChanged.AddListener(ModifyAmmo);
        
        //--------------------------------------------------------------------------------
        
        _maxLivesFormat = maxLives.text;
        maxLives.text = string.Format(_maxLivesFormat, playerConfig.MaxBullets);
        
        maxLivesSlider.onValueChanged.AddListener(ModifyLives);
        
        //--------------------------------------------------------------------------------
        
        _knockbackFormat = knockback.text;
        knockback.text = string.Format(_knockbackFormat, playerConfig.KnockBack);
        
        knockbackSlider.onValueChanged.AddListener(ModifyKnockBack);
        
        //--------------------------------------------------------------------------------
        
        _modifierFormat = modifier.text;
        modifier.text = string.Format(_modifierFormat, playerConfig.MaxDistance);
        
        modifierSlider.onValueChanged.AddListener(ModifyModifier);
    }

    private void ModifyModifier(float value)
    {
        playerConfig.MaxDistance = value;
        modifier.text = string.Format(_modifierFormat, playerConfig.MaxDistance);
    }

    private void ModifyKnockBack(float value)
    {
       playerConfig.KnockBack = value;
       knockback.text = string.Format(_knockbackFormat, playerConfig.KnockBack);
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