using System;
using System.Collections;
using UnityEngine;

namespace Particle
{
    public class ParticleController : MonoBehaviour
    {
        private ParticleSystem _particleSystem;

        private Action _action;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        public void SetUp(Action action = null)
        {
            _particleSystem.Play();
            _action = action;

            StartCoroutine(ReturnToPool());
        }

        private IEnumerator ReturnToPool()
        {
            yield return new WaitForSeconds(_particleSystem.main.duration);

            _action?.Invoke();
        }
    }
}