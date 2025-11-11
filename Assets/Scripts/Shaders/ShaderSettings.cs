using Systems;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Shaders
{
    public class ShaderSettings : MonoBehaviour
    {
        private IShaderSettings _settings;

        private Toggle _toggle;

        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private TMP_Text masterText;

        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private TMP_Text sfxText;

        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private TMP_Text musicText;

        private const string PercentageFormat = "{0}%";
        
        private void Awake()
        {
            _settings = ServiceProvider.GetService<IShaderSettings>();

            _toggle = GetComponentInChildren<Toggle>();

            _toggle.isOn = _settings.GetFlickerValue() != 0;
            _toggle.onValueChanged.AddListener(ActiveShaderFlicker);
        }

        private void Start()
        {
            SetUpSlider(masterVolumeSlider, OnSetMasterVolume, "MasterVol");
            SetUpSlider(sfxVolumeSlider, OnSetSFXVolume, "SFXVol");
            SetUpSlider(musicVolumeSlider, OnSetMusicVolume, "MusicVol");

            SetUpText(masterVolumeSlider.value, masterText);
            SetUpText(sfxVolumeSlider.value, sfxText);
            SetUpText(musicVolumeSlider.value, musicText);
        }

        private void ActiveShaderFlicker(bool active) => _settings?.SetFlickerActive(active);

        private void OnSetMasterVolume(float value)
        {
            AkUnitySoundEngine.SetRTPCValue("MasterVol", value);
            UpdateText(value, masterText);
        }

        private void OnSetSFXVolume(float value)
        {
            AkUnitySoundEngine.SetRTPCValue("SFXVol", value);
            UpdateText(value, sfxText);
        }

        private void OnSetMusicVolume(float value)
        {
            AkUnitySoundEngine.SetRTPCValue("MusicVol", value);
            UpdateText(value, musicText);
        }

        private void SetUpSlider(Slider slider, UnityAction<float> function, string valueName)
        {
            if (!slider || function == null || valueName == null)
                return;
            
            slider.onValueChanged.AddListener(function);
            slider.wholeNumbers = true;
            slider.minValue = 0;
            slider.maxValue = 100;

            SetSliderValue(slider, valueName);
        }

        private void SetSliderValue(Slider slider, string valueName)
        {
            if (!slider || valueName == null)
                return;

            int valueType = 0;

            AKRESULT result = AkUnitySoundEngine.GetRTPCValue
            (
                valueName,
                null,
                0,
                out float value,
                ref valueType
            );

            if (result == AKRESULT.AK_Success)
            {
                slider.value = value;
            }
            else
            {
                Debug.LogWarning($"The Wwise Value {value} is not a valid value.");
            }
        }

        private static void SetUpText(float value, TMP_Text actualText)
        {
            UpdateText(value, actualText);
        }

        private static void UpdateText(float value, TMP_Text actualText) =>
            actualText.text = string.Format(PercentageFormat, value);

        public void GoBack()
        {
            if (Application.isPlaying)
                Destroy(gameObject);
            else
                DestroyImmediate(gameObject);
        }
    }
}