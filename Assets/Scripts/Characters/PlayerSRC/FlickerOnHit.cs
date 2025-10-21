using System.Collections;
using System.Collections.Generic;
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

        [FormerlySerializedAs("mat")] [SerializeField]
        private List<Material> materials;

        private readonly List<Color> _originalColors = new();

        private void Start()
        {
            foreach (var material in materials)
                _originalColors.Add(material.color);

            ServiceProvider.TryGetService<ICentralizeEventSystem>(out var eventSystem);

            eventSystem.Get<int, int, int>(PlayerEventKeys.LivesChange).AddListener(Flash);
        }

        private void OnDestroy()
        {
            for (var i = 0; i < materials.Count; i++)
            {
                materials[i].color = _originalColors[i];
            }
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
                foreach (var t in materials)
                {
                    t.color = color;
                }

                yield return new WaitForSeconds(flashDuration);

                for (var j = 0; j < materials.Count; j++)
                {
                    materials[j].color = _originalColors[j];
                }

                yield return new WaitForSeconds(flashDuration);
            }
        }
    }
}