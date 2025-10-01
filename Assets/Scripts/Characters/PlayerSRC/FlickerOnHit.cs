using System.Collections;
using Systems;
using Systems.CentralizeEventSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace Characters.PlayerSRC
{
    public class FlickerOnHit : MonoBehaviour
    {
        [SerializeField] private Color hitColor = Color.red;
        [SerializeField] private Color healColor = Color.green;
        [SerializeField] private float flashDuration = 0.1f;
        [SerializeField] private int amountOfFlickers = 3;

        private Material _mat;
        private Color _originalColor;

        private void Start()
        {
            _mat = GetComponentInChildren<Renderer>().material;
            _originalColor = _mat.color;

            ServiceProvider.TryGetService<ICentralizeEventSystem>(out var eventSystem);

            eventSystem.Get<int, int, int>(PlayerEventKeys.LivesChange).AddListener(Flash);
        }

        private void Flash(int previous, int current, int max)
        {
            if (current == previous)
                return;

            StartCoroutine(FlashRoutine(current < previous ? hitColor : healColor));
        }

        private IEnumerator FlashRoutine(Color color)
        {
            for (var i = 0; i < amountOfFlickers; i++)
            {
                _mat.color = color;
                yield return new WaitForSeconds(flashDuration);
                _mat.color = _originalColor;
                yield return new WaitForSeconds(flashDuration);
            }
        }
    }
}