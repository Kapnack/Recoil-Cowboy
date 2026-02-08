using System;
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
        private CentralizeEventSystem _eventSystem;

        private void Awake()
        {
            _eventSystem = ServiceProvider.GetService<CentralizeEventSystem>();
            material.SetFloat(SpeedVariable, Speed);
            material.SetFloat(IntensityVariable, 0);
        }

        private void Start()
        {
            _eventSystem.AddListener<LivesChange>(OnLivesChange);
            _eventSystem.AddListener<InvincibilityOff>(OnNoLongerInvincible);
        }

        private void OnDestroy()
        {
            _eventSystem.RemoveListener<LivesChange>(OnLivesChange);
            _eventSystem.RemoveListener<InvincibilityOff>(OnNoLongerInvincible);
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

#if UNITY_EDITOR
        private void OnValidate()
        {
            material.SetFloat(SpeedVariable, Speed);
            material.SetFloat(IntensityVariable, 0);
        }
#endif
    }
}