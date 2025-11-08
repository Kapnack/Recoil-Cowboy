using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

public class DebugTools : MonoBehaviour
{
    [SerializeField] public PlayerConfig playerConfig;
    [SerializeField] private Toggle infiniteAmmoToggle;
    [SerializeField] private Toggle infiniteLivesToggle;

    private void Awake()
    {
        infiniteAmmoToggle.isOn = playerConfig.InfinitAmmo;
        infiniteLivesToggle.isOn = playerConfig.InfinitLives;

        infiniteAmmoToggle.onValueChanged.AddListener(ToggleInfinityAmmo);
        infiniteLivesToggle.onValueChanged.AddListener(ToggleInfinityLives);
    }

    private void ToggleInfinityAmmo(bool value) =>
        playerConfig.InfinitAmmo = value;

    private void ToggleInfinityLives(bool value) =>
        playerConfig.InfinitLives = value;
}