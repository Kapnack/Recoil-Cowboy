using Systems;
using UnityEngine;
using UnityEngine.UI;

namespace Shaders
{
    public class ShaderSettings : MonoBehaviour
    {
        private IShaderSettings settings;

        private Toggle toggle;

        private void Awake()
        {
            Cursor.visible = true;

            settings = ServiceProvider.GetService<IShaderSettings>();

            toggle = GetComponentInChildren<Toggle>();

            toggle.isOn = settings.GetFlickerValue() != 0;
            toggle.onValueChanged.AddListener(ActiveShaderFlicker);
        }

        private void OnDestroy() => Cursor.visible = false;

        private void ActiveShaderFlicker(bool active) => settings?.SetFlickerActive(active);

        public void GoBack()
        {
            if (Application.isPlaying)
                Destroy(gameObject);
            else
                DestroyImmediate(gameObject);
        }
    }
}