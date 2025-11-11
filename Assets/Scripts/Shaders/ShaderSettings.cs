using ScriptableObjects;
using Systems;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Shaders
{
    public class ShaderSettings : MonoBehaviour
    {
        [SerializeField] private VolumeSettings volumeSettings;

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
            SetUpSlider(masterVolumeSlider, OnSetMasterVolume, volumeSettings.MasterVol,
                nameof(volumeSettings.MasterVol));
            SetUpSlider(sfxVolumeSlider, OnSetSFXVolume, volumeSettings.SFXVol, nameof(volumeSettings.SFXVol));
            SetUpSlider(musicVolumeSlider, OnSetMusicVolume, volumeSettings.MusicVol, nameof(volumeSettings.MusicVol));

            SetUpText(masterVolumeSlider.value, masterText);
            SetUpText(sfxVolumeSlider.value, sfxText);
            SetUpText(musicVolumeSlider.value, musicText);
        }

        private void ActiveShaderFlicker(bool active) => _settings?.SetFlickerActive(active);

        private void OnSetMasterVolume(float value)
        {
            AkUnitySoundEngine.SetRTPCValue(nameof(volumeSettings.MasterVol), value);
            volumeSettings.MasterVol = (int)value;

            UpdateText(value, masterText);
        }

        private void OnSetSFXVolume(float value)
        {
            AkUnitySoundEngine.SetRTPCValue(nameof(volumeSettings.SFXVol), value);
            volumeSettings.SFXVol = (int)value;

            UpdateText(value, sfxText);
        }

        private void OnSetMusicVolume(float value)
        {
            AkUnitySoundEngine.SetRTPCValue(nameof(volumeSettings.MusicVol), value);
            volumeSettings.MusicVol = (int)value;

            UpdateText(value, musicText);
        }

        private static void SetUpSlider(Slider slider, UnityAction<float> function, int value, string valueName)
        {
            if (!slider || function == null || valueName == null)
                return;

            slider.onValueChanged.AddListener(function);
            slider.wholeNumbers = true;
            slider.minValue = 0;
            slider.maxValue = 100;

            SetSliderValue(slider, value, valueName);
        }

        private static void SetSliderValue(Slider slider, int value, string valueName)
        {
            if (!slider || valueName == null)
                return;

            slider.value = value;
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