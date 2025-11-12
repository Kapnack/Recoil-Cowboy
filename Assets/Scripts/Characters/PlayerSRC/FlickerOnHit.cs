using Systems;
using Systems.CentralizeEventSystem;
using UnityEngine;

namespace Characters.PlayerSRC
{
    public class FlickerOnHit : MonoBehaviour
    {
        [SerializeField] private Material material;

        [SerializeField] private float Speed;
        [SerializeField] private float Intencity;

        private const string SpeedVariable = "_Speed";
        private const string IntensityVariable = "_Intensity";

        private Coroutine _currentRoutine;

        private void Awake()
        {
            material.SetFloat(SpeedVariable, Speed);
            material.SetFloat(IntensityVariable, 0);
        }

        private void Start()
        {
            ICentralizeEventSystem eventSystem = ServiceProvider.GetService<ICentralizeEventSystem>();

            eventSystem.Get<int, int, int>(PlayerEventKeys.LivesChange).AddListener(OnLivesChange);
            eventSystem.Get(PlayerEventKeys.NoLongerInvincible).AddListener(OnNoLongerInvincible);
        }

        private void OnLivesChange(int previous, int actual, int max)
        {
            if (actual == previous)
                return;

            material.SetFloat(IntensityVariable, 1);
        }

        private void OnNoLongerInvincible()
        {
            material.SetFloat(IntensityVariable, 0);
        }
    }
}