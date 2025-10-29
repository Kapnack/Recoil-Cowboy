using System.Collections;
using Systems;
using UnityEngine;

namespace Shaders
{
    public class ShaderManager : MonoBehaviour, IShaderManager, IShaderSettings
    {
        [SerializeField] private Material material;

        private const string IntensityVariable = "_Intensity";
        private const string FlickerIntensityVariable = "_FlickerIntencity";

        private Coroutine _currentRoutine;

        private void Awake()
        {
            ServiceProvider.SetService<IShaderManager>(this);
            ServiceProvider.SetService<IShaderSettings>(this);

            material.SetFloat(IntensityVariable, 0);
        }

        private IEnumerator OnTransition()
        {
            var intensity = material.GetFloat(IntensityVariable);

            while (intensity < 1)
            {
                intensity += Time.deltaTime;
                material.SetFloat(IntensityVariable, intensity);
                yield return null;
            }

            intensity = Mathf.Clamp(intensity, 0, 1);
        }

        private IEnumerator OffTransition()
        {
            var intensity = material.GetFloat(IntensityVariable);

            while (intensity > 0)
            {
                intensity -= Time.deltaTime;
                material.SetFloat(IntensityVariable, intensity);
                yield return null;
            }

            intensity = Mathf.Clamp(intensity, 0, 1);
        }

        private void StopRoutine()
        {
            if (_currentRoutine != null)
                StopCoroutine(_currentRoutine);
        }

        private void OnDisable()
        {
            StopRoutine();
        }

        public void StartOnTransition()
        {
            StopRoutine();
            _currentRoutine = StartCoroutine(OnTransition());
        }

        public void StartOffTransition()
        {
            StopRoutine();
            _currentRoutine = StartCoroutine(OffTransition());
        }

        public void SetFlickerActive(bool active) => material.SetFloat(FlickerIntensityVariable, active ? 1f : 0f);
        public float GetFlickerValue() => material.GetFloat(FlickerIntensityVariable);
    }
}